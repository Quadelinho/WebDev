using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestApiTest.Core.DTO;
using RestApiTest.Core.Interfaces.Repositories;
using RestApiTest.Core.Models;
using RestApiTest.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApiTest.Controllers
{
    [Route("api/[controller]")] //[note] gdzie można podejrzeć dostępne tokeny, takie jak ten? - w dokumentacji na MSDN
    [ApiController]
    public class BlogController : ControllerBase
    {
        private IBlogPostRepository repository;
        private ILogger<BlogController> logger;
        private IMapper mappingProvider;
        private IConfiguration configuration; 

        public BlogController(ILogger<BlogController> log, IBlogPostRepository repository, IHostingEnvironment environment, IMapper mapper, IConfiguration configuration)
        {
            logger = log;
            //logger.LogError("Sample error {0}, {@1}", environment, new { value = 1, value2 ="test" }); //Przykład możliwości automatycznego serializowania obiektów przez Serilog'a
            this.repository = repository;
            mappingProvider = mapper;
            this.configuration = configuration;
            //var posts = repository.GetAllBlogPostsAsync();
            //if (posts == null || posts.Count() == 0)
            //{
            //    var sampleData = CreateSampleData(5);
            //    foreach (var post in sampleData)
            //    {
            //        repository.AddAsync(post);
            //    }
            //}
        }

        //GET api/blog
        //[HttpGet]
        //[ProducesResponseType(typeof(IEnumerable<BlogPost>), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //public async Task<ActionResult<IEnumerable<BlogPost>>> Get()
        //{
        //    logger.LogInformation("Calling get for all objects");
        //    var allPosts = await repository.GetAllBlogPostsAsync();
        //    if (allPosts != null)
        //    {
        //        return Ok(allPosts); 
        //    }
        //    else
        //    {
        //        logger.LogWarning("There is no blog post element to be returned");
        //        return NoContent();
        //    }
        //}

        //GET api/blog/5
        [HttpGet("{id}", Name = "GetBlog")] //[Note] Co daje ta nazwa? Odwołanie się przez nią mi nie przechodzi - nie do url, tylko alias na potrzeby kodu
                                            //[Note] Czy to można zdefiniować, żeby podawać wartość w parametrze typu "...?id=5"? //[??]Jak wywołać to z użyciem tego ActionName? ODP: tak, ale trzeba by zmienić routing
        [ProducesResponseType(typeof(BlogPostDTO), StatusCodes.Status200OK)] //[Note - DTO] Czy dla dokumentacji Swagger'a podaje się jako typ zwracany obiekty DTO, czy rzeczywistej klasy?
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BlogPostDTO>> Get(long id)
        {
            logger.LogInformation("Calling get for object with id =" + id);
            BlogPost post = await repository.GetAsync(id);
            if (post != null)
            {
                BlogPostDTO postToReturn = mappingProvider.Map<BlogPost, BlogPostDTO>(post); //[Note] - konfiguracja początkowa jest niezbędna, bo bez niej automapper nie będzie w stanie niczego rozwiązać. Dodatkowo tutaj nie trzeba podawać obu typów, wystarczy docelowy, jeśli mapowanie jest unikalne (np. nie ma w konfiguracji mapowania jednego źródła na kilka docelowych) - Czy skoro tutaj się tego używa, to ta konfiguracja nie jest zbędna, bo to trochę wygląda na redundancję? A może ja coś źle robię?
                return Ok(postToReturn);
            }
            else
            {
                logger.LogWarning("No blog post with given id exists");
                return NotFound(id);
            }
        }

        //Done: Szukanie po tytułach (adres endpointa: /posts/?title contains) 
        //TODO: pageing -> response next page (link do następnej strony) (adres endpointa: /posts/?page=)

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BlogPostDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<PageDTO<BlogPostDTO>>> GetAll()
        {
            //[Note] - powinny być osobne metody, które będą procesowały typy z hierarchii zgodnie z założeniami klienta / biznesu - Czy to powinno też zwracać obiekty klasy pochodnej (QuestionPost), bo skoro są pochodnymi to są też postami (domenowo tak samo - pytanie też jest postem) //TODO: rozdzielić na osobne metody
            logger.LogInformation("Calling get for all posts");
            var posts = /*await*/ repository.GetAllBlogPostsAsync()/*.ToAsyncEnumerable()*/; //?? Jak to tutaj powinno być zwracane, żeby było asynchroniczne (czy da radę bez IAsyncEnumerablle)?
            long? count = /*await*/ posts?.Count();
            if (count.HasValue && count.Value > 0)
            {
                return Ok(mappingProvider.ProjectTo<BlogPostDTO>(posts)); //[Note] sygnatura metody powinna być bez queryable, ale zwracać można bez problemu - Czy tutaj mogę po prostu zwracać IQueryable skoro ten interface dziedziczy po IEnumerable, czy jednak mam użyć np. ToList i zwracać listę dla pełnej zgodności z IEnumerable?
            }
            else
            {
                logger.LogWarning("No posts to return");
                return NoContent();
            }
        }

        //[HttpGet("pages/{pageNo}")]
        [HttpGet("pages")]
        [ProducesResponseType(typeof(IEnumerable<BlogPostDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<ActionResult<IEnumerable</*BlogPostDTO*/PageDTO<BlogPostDTO>>>> GetNextChunk([FromQuery]int pageNo, int postsPerPage)
        public async Task<ActionResult<PageDTO<BlogPostDTO>>> GetNextChunk([FromQuery]int pageNo, int postsPerPage)
        {
            int maxPostsPerPage = configuration.GetValue<int>("MaxPostsPerPage"); //?? Czy w przypadku plików appsettings to zachowuje się tak jak z config'ami xml - że plik konfiguracji jest ładowany raz przy starcie aplikacji nie może zostać podmieniony w locie?
            if(!Request.Query.ContainsKey("pageNo")
                || (Request.Query.ContainsKey("postsPerPage") && postsPerPage > maxPostsPerPage)) //Done: Górny limit postów odczytywać z config'a //?? 
            {
                logger.LogWarning("Posts paging bad request");
                //?? Jakie podejście stosuje się w praktyce przy zbyt dużej liczbie podanej do zwrotu - bad request, czy nadpisać tą wartość maksymalną dopuszczalną i zwrócić (chyba to lepsze ze względu na płynność, ale może być niejasne dla użytkownika i być może ujawniać limity systemu potencjalnym atakującym?)
                return BadRequest();
            }

            logger.LogInformation("Calling get for next chunk");
            var posts = /*await*/ repository.GetBlogPostsChunkAsync(pageNo, postsPerPage);
            long? count = posts?.Count();
            if (count.HasValue && count.Value > 0)
            { //TODO: Done - Przygotować do zwrotu obiekt PageDTO
                decimal totalPages = Math.Ceiling(repository.GetTotalPostsCount() / postsPerPage);
                StringBuilder urlBuilder = new StringBuilder("https://").Append(Request.Host);
                urlBuilder.Append(Request.Path).AppendFormat("?pageNo={0}&postsPerPage={1}", pageNo + 1 >= totalPages ? pageNo : ++pageNo, postsPerPage); //?? Jak wygląda prawilny sposób generowania url'a dl następnej strony?
                
                PageDTO<BlogPostDTO> valueToReturn = new PageDTO<BlogPostDTO>(mappingProvider.ProjectTo<BlogPostDTO>(posts).ToList(),
                    (int)totalPages,
                    urlBuilder.ToString());

                return Ok(valueToReturn); //?? Gdzie jeszcze powinny być zwracane url'e, żeby była pełna zgodność z HATEOAS'em?
            }
            else
            {
                logger.LogWarning("No posts to return");
                return NoContent();
            }
        }

        //[HttpGet("/api/blogposts/find/{titlePartToFind?}")] //[Note] - tak jak w pytaniu - Jak określić, żeby podawać parametry po "?" (np. find?title=test)? //TODO: wyszukać przykład użycia i konfiguracji (http query parameter)
        //[HttpGet("find/{titlePartToFind}")]
        [HttpGet("find")]
        [ProducesResponseType(typeof(IEnumerable<BlogPostDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<BlogPostDTO>>> GetByTitle([FromQuery]string titlePartToFind) //[Note] - przeglądarka powinna to zdekodować prawidłowo i przesłać już odpowiednio sformatowanego URL'a - Jak zapewnić możliwość odpytania o znaki specjalne (np. w tytule Entry #)? Podanie w URL formy zakodowanej z %23 przekazuje wartość "#" w parametrze, ale w bazie nie jest to znajdywane. Czy to może być wina comparator'a stringów?
        { //?? Czy można jakoś wymusić, żeby znaki specjalne były odpowiednio parsowane? Jak wpiszę w przeglądarce ?... entry # to nie koduje i nie przesyła tego #, dopiero go przekazuje jak sam podam entry%20%23 (chociaż spacje koduje prawidłowo od razu)?

            //if(string.IsNullOrWhiteSpace(Request[""].QueryString("titlePartToFind")))
            if(!Request.Query.ContainsKey("titlePartToFind")) //[Note] - można w parametrze robić binding do obiektu: np. GetByTitle([FromQuery]ModelObject paramName) i potem sprawdzać if(ModelState.IsValid) -> to sprawdzi, czy parametr podany w query da radę być zmapowany na właściwość obiektu
            {
                logger.LogWarning("The find request contained invalid parameter");
                return BadRequest(); //?? Czy w BadRequest zwraca się coś informacyjnego (np. treść/url requestu)?
            }

            logger.LogDebug("Calling get for all posts containing in title: {0}", titlePartToFind); //[Note] - Done - takie rzeczy tylko jako debug
            var posts = /*await*/ repository.GetPostsContaingInTitle(titlePartToFind);
            long? count = posts?.Count();
            if (count.HasValue && count.Value > 0)
            {
                return Ok(mappingProvider.ProjectTo<BlogPostDTO>(posts));
            }
            else
            {
                logger.LogWarning("No posts to return");
                return NoContent();
            }
        }
        
        // POST api/blog
        [HttpPost]
        [ProducesResponseType(typeof(BlogPostDTO), StatusCodes.Status201Created)] //[Note] Co definiuje się w takich przypadkach jako typ zwracany? Muszę podawać zawsze typ rzeczywisty, bo interface nie może być obiektem typeof? ODP: tak, podaje się typ rzeczywisty
        public async Task<IActionResult> Post([FromBody] BlogPostDTO postToAdd) //[note] Czy tutaj to FromBody jest konieczne? Czy domyślnie typy złożone nie powinny być odczytywane z body? ODP: nie jest konieczne, bo domyślnie są odczytywane z body, ale poprawia czytelność
        {
            logger.LogInformation("Calling post for the following object: {@0} ", postToAdd); //?? Czy przy tym nie ma tej automatycznej weryfikacji modelu? W body post'a miałem więcej pól i wszystko przeszło. Czy da się wymusić kontrolę 1:1 (żeby body było w 100% zgodne z modelem?
//           postToAdd.Modified = DateTime.Now.ToLongDateString();
            BlogPost addedPost = await repository.AddAsync(mappingProvider.Map<BlogPostDTO, BlogPost>(postToAdd));
            var addedPostDTO = mappingProvider.Map<BlogPostDTO>(addedPost); //[Note] - w tego typu aplikacjach narzut wynikający z mapowania jest powszechnym i akceptowanym minusem, bo mapowanie jest konieczne - Czy to podwójne mapowanie nie jest już za dużym narzutem na taką akcję?
            return CreatedAtRoute("GetBlog", new { id = addedPost.Id }, addedPostDTO); //[note] W jaki sposób przerobić to na pojedynczy punkt wyjścia? Czy jest jakiś typ wspólny dla tych helpersów i czy tak się w ogóle robie w web dev'ie? ODP: nie stosuje się tego podejścia w aplikacjach web'owych
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(BlogPostDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BlogPostDTO), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Patch(long id, [FromBody] List<PatchDTO> propertiesToUpdate)
        {//[Note] definiuje się np. specjalne usługi walidacyjne. Można użyć FluentValidator'a - W jaki sposób prawidłowo zapewnia się walidację obiektu wejściowego, żeby dla POST'a obiekt DTO nie zawierał żadnych pustych pól, a patch pozwalał ustawić np. tylko jedno pole? Na necie znalazłem podejście z osobnym obiektem PatchDTO, ale to wygląda jak mocny nadmiar

            //[Note] - osobne DTO dla response i request - Czy jest jakiś sprytny sposób zablokowania pól przed edycją, oprócz private set i DTO, np. jeśli chcemy, żeby jakieś pola nie mogły być edytowane patch'em?
            logger.LogInformation("Calling patch for the following object with id = {0} ", id);
            try
            {
                var postToModify = await repository.GetAsync(id);
                if (postToModify != null)
                {
                    var modifiedObject = await repository.ApplyPatchAsync(postToModify, propertiesToUpdate);
                    return Ok(mappingProvider.Map<BlogPostDTO>(modifiedObject));
                }
                else
                {
                    return NotFound(id);
                }
            }
            catch (Exception)
            {
                return NotFound(id); //[Note] - notFound, nie ma w bieżącej wersji middlewere'a o nazwie notModified - W tym przypadku powinno być zwracane NotFound, czy NotModified?
            }
        }

        // PUT api/blog/5
        [HttpPut("{id}")]
        [ActionName("UpdatePostTitle")]
        [ProducesResponseType(typeof(BlogPost), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BlogPostDTO), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Put(long id, [FromBody] BlogPostDTO updatedPost)
        {
            //if(!ModelState.IsValid) //[Note] - nie wystarczy, bo model traktowany jest jako prawidłowy chyba, że jakieś właściwości byłyby ustawione jako required - Czy to wystarczy do sprawdzenia poprawności (np. zamiast service'u walidującego)
            //{
            //    return BadRequest();
            //}

            //[Note] zwraca się zawsze stan aktualny z bazy, a nie obiekt z parametrów
            logger.LogInformation("Calling put for object: {@0}", updatedPost);
            try
            {
                updatedPost.Id = id;
                var modifiedPost = await repository.UpdateAsync(mappingProvider.Map<BlogPost>(updatedPost));
                return Ok(mappingProvider.Map<BlogPostDTO>(modifiedPost));
            }
            catch (InvalidOperationException) //?? Czy muszę osobno przechwytywać wyjątek z duplikacją tytułu i zwracać wtedy 409, czy coś innego?
            {
                logger.LogWarning("There was nothing to update");
                return NotFound(id);
            }

            //try //[Note] raczej nie ma potrzeby używać tu try-catch -> zazwyczaj w web dev'ie stosuje się podejście global exception handler'a
             //Użyć Update, żeby nie zmieniać całego kontekstu
            
            //[Note] - jeśli jest tworzony nowy zasób - zwracać tak jak w Post, jeśli jest modyfikowany, nie trzeba zwracać ścieżki, bo jest w wywołaniu URL - Co tu ma być zwrócone, żeby było zgodne z HATEOS'em (żeby zwróciło w responsie url'a?) - to co w Post?
            //return Ok(updatedPost);//Może być NoContent
        }

        // DELETE api/blog/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Delete(int id)
        {
            var post = await repository.GetAsync(id); //[Note] - potencjalnie zawsze można usuwać od razu - czy w praktyce tak się robi, czy raczej od razu próbować usunąć dla lepszej wydajności?
            if (post == null)
            {
                logger.LogWarning("Element with given ID doesn't exist - nothing is deleted");
                return NoContent();
            }

            await repository.DeleteAsync(id);
            logger.LogInformation("Element with given ID has been successfully removed");
            return NoContent(); //[Note] Jak zwrócić element usunięty? Ma być wtedy zwracany status OK z obiektem? [OK + obiekt]
        }
        
        private IEnumerable<QuestionPost> CreateSampleData(int requiredNumberOfSamples)
        {
            List<QuestionPost> posts = new List<QuestionPost>();
            for (int postIndex = 1; postIndex <= requiredNumberOfSamples; ++postIndex)
            {
                posts.Add(new QuestionPost()
                {
                    //Author = "User" + postIndex,
                    Content = DateTime.Now.ToShortDateString(),
 //                   Modified = DateTime.Now.ToLongDateString(),
                    Title = "Entry #" + postIndex
                });
            }
            return posts;
        }
    }
}

//Map:
//[note]
//??

//Zadanie 12.06
//Done: usunąć nullable z encji w Core.Models i oznaczyć jako required
//Done: Implementacja DTO dla kontrolerów, mają mieć pola nullowalne. DTO ma nie mieć pola 'ModifiedDate' //[Note] - DTO i encje są modelami danych, ale DTO jest uproszczony, na poziomie tylko kontrolera, a encja jest modelem pełnym, domenowym
//Done: dodać w kontrolerach implementację akcji patch dla aktualizacji tylko określonych pól, jeśli nie są nullami
//Done: pageowanie rezultatów zwracanych przez getAll posts //[Note] - możliwe 2 podejścia a) podawać do backend'u rozmiar paczki do zwrotu i wtedy fronend odpowiada za wyznaczanie stron (bardziej elastyczne rozwiązanie), b) podawać do backendu numer strony do zwrotu, a backend wylicza strony (lepsze w naszym przypadku, bo nie mamy frontendu)

//Zadanie z 24.07
//TODO: tworzenie danych tymczasowych / początkowych przenieść do startup'u (wzorzec inicjalizacji bazy danych)
//TODO: po inicjalizacji bazy danych w startup'ie wymuszać programistycznie utworzenie migracji + przed rozpoczęciem jakichkolwiek akcji - wywoływać programistycznie aktualizację bazy do właściwego stanu
//TODO: z poziomu kontrolerów wywoływać service walidujący //?? czy ten projekt ma w praktyce rzeczywiście być service'm cały czas działającym w tle, czy wystarczy zwykły obiekt powoływany czasowo tylko na potrzeby walidacji?
//TODO: jeśli property przekazane do put / patch jest błędne, zwracać bad request z poziomu kontrolera
//TODO: ApplyPatch przenieść do klasy bazowego repo i zmienić nazwę na Update tylko z przeładowaniem (żeby patch i put korzystały z tej samej metody, ale osiąganej z różnych adresów)
//TODO: do walidacji kontrolerów używać FluentValidator
//Done: paging - poprawić routing dla spójności: api/blogposts/pages/
//TODO: paging z HATEOS'a - zwracać url do następnej strony - obiekt PageDTO (kolekcja encji dla danej strony, nextPage, totalPages)
//TODO: paging - przeładowany routing -> api/blogpost/{id} do zwracania konkretnego posta i api/blogpost/pages/{pageNo} do zwracania strony
//TODO: ilość postów na page przekazywać jako parametr do metody w kontrolerze i z kontrolera do repo [Note] - tak się robi w praktyce, nawet jeśli np. frontend limituje to do określonej liczby, to nie blokuje się użytkownikowi zazwyczaj możliwości jawnego podania dowolnej liczby (co najwyżej nakłada się ograniczenie na maksymalną liczbę, jaką może podać)
//?? Jak działa niejawne przekazywanie parametrów, jeśli z formularza html określi się coś, jako niewidoczne w request'cie, bo nie przypominam sobie, żeby tam się definiowało jakieś szyfrowanie itp. domyślnie
//TODO: [future] - generyczne filtorwanie - dać możliwość definiowania większej ilości filtrów w parametrze URL, np. rozdzielanych przez | i potem odpowiednio parsowanych i aplikowanych