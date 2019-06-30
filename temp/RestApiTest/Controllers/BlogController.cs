using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestApiTest.Core.DTO;
using RestApiTest.Core.Interfaces.Repositories;
using RestApiTest.Core.Models;
using RestApiTest.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public BlogController(ILogger<BlogController> log, IBlogPostRepository repository, IHostingEnvironment environment, IMapper mapper)
        {
            logger = log;
            //logger.LogError("Sample error {0}, {@1}", environment, new { value = 1, value2 ="test" }); //Przykład możliwości automatycznego serializowania obiektów przez Serilog'a
            this.repository = repository;
            mappingProvider = mapper;
            var posts = repository.GetAllBlogPostsAsync();
            if (posts == null || posts.Count() == 0)
            {
                var sampleData = CreateSampleData(5);
                foreach (var post in sampleData)
                {
                    repository.AddAsync(post);
                }
            }
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<BlogPostDTO>> Get(long id)
        {
            logger.LogInformation("Calling get for object with id =" + id);

           // IMapper mapper = mappingProvider.ConfigurationProvider.CreateMapper();

            BlogPost post = await repository.GetAsync(id);
            if (post != null)
            {
                var destination = mappingProvider.Map<BlogPost, BlogPostDTO>(post); //??  Czy skoro tutaj się tego używa, to ta konfiguracja nie jest zbędna, bo to trochę wygląda na redundancję? A może ja coś źle robię?
                return Ok(destination);
            }
            else
            {
                logger.LogWarning("No blog post with given id exists");
                return NotFound(id);
            }
        }

        //TODO: Szukanie po tytułach (adres endpointa: /posts/?title contains) 
        //TODO: pageing -> response next page (link do następnej strony) (adres endpointa: /posts/?page=)

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BlogPostDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<IEnumerable<BlogPostDTO>>> GetAll()
        {
            //?? Czy to powinno też zwracać obiekty klasy pochodnej (QuestionPost), bo skoro są pochodnymi to są też postami (domenowo tak samo - pytanie też jest postem) //TODO: rozdzielić na osobne metody
            logger.LogInformation("Calling get for all posts");
            var posts = /*await*/ repository.GetAllBlogPostsAsync(); //?? Jak tutaj się robi jakieś "porcjowanie", albo coś w rodzaju yield'a, żeby nie pchać dużej paczki w response'ie/ //TODO: AsyncEnumerable
            long? count = posts?.Count();
            if (count.HasValue && count.Value > 0)
            {
                List<BlogPostDTO> postsToReturn = mappingProvider.ProjectTo<BlogPostDTO>(posts).ToList();
                return Ok(postsToReturn);
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
            var addedPost = await repository.AddAsync(mappingProvider.Map<BlogPostDTO, BlogPost>(postToAdd));
            var addedPostDTO = mappingProvider.Map<BlogPostDTO>(addedPost); //?? Czy to podwójne mapowanie nie jest już za dużym narzutem na taką akcję?
            return CreatedAtRoute("GetBlog", new { id = addedPost.Id }, addedPostDTO); //[note] W jaki sposób przerobić to na pojedynczy punkt wyjścia? Czy jest jakiś typ wspólny dla tych helpersów i czy tak się w ogóle robie w web dev'ie? ODP: nie stosuje się tego podejścia w aplikacjach web'owych
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(BlogPostDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BlogPostDTO), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Patch(long id, [FromBody] List<PatchDTO> propertiesToUpdate)
        {//?? W jaki sposób prawidłowo zapewnia się walidację obiektu wejściowego, żeby dla POST'a obiekt DTO nie zawierał żadnych pustych pól, a patch pozwalał ustawić np. tylko jedno pole? Na necie znalazłem podejście z osobnym obiektem PatchDTO, ale to wygląda jak mocny nadmiar

            //?? Czy jest jakiś sprytny sposób zablokowania pól przed edycją, oprócz private set, np. jeśli chcemy, żeby jakieś pola nie mogły być edytowane patch'em?
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
                return NotFound(id); //?? W tym przypadku powinno być zwracane NotFound, czy NotModified?
            }
        }

        // PUT api/blog/5
        [HttpPut("{id}")]
        [ActionName("UpdatePostTitle")]
        [ProducesResponseType(typeof(BlogPost), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BlogPost), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Put(long id, [FromBody] BlogPostDTO updatedPost)
        {
            //TODO: zwracać zawsze stan aktualny z bazy, a nie obiekt z parametrów
            //TODO: w controller'ach używać DTO
            logger.LogInformation("Calling put for object: {@0}", updatedPost);
            try
            {
                var modifiedPost = await repository.UpdateAsync(mappingProvider.Map<BlogPost>(updatedPost));
                return Ok(mappingProvider.Map<BlogPostDTO>(modifiedPost));
            }
            catch (InvalidOperationException) //?? Czy muszę osobno przechwytywać wyjątek z duplikacją tytułu i zwracać wtedy 409, czy coś innego?
            {
                logger.LogWarning("There was nothing to update");
                return NotFound(id);
            }
//            post.Modified = DateTime.Now.ToLongDateString();

            //try //[Note] raczej nie ma potrzeby używać tu try-catch -> zazwyczaj w web dev'ie stosuje się podejście global exception handler'a
             //Użyć Update, żeby nie zmieniać całego kontekstu
            
            //[Note] - jeśli jest tworzony nowy zasób - zwracać tak jak w Post, jeśli jest modyfikowany, nie trzeba zwracać ścieżki, bo jest w wywołaniu URL - Co tu ma być zwrócone, żeby było zgodne z HATEOS'em (żeby zwróciło w responsie url'a?) - to co w Post?
            //return Ok(updatedPost);//Może być NoContent
        }

        // DELETE api/blog/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)] //DONE: ProduceResponse dla pozostałych endpointów
        public async Task<ActionResult> Delete(int id)
        {
            var post = await repository.GetAsync(id); //?? czy w praktyce tak się robi, czy raczej od razu próbować usunąć dla lepszej wydajności?
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
//TODO: usunąć nullable z encji w Core.Models
//TODO: Implementacja DTO dla kontrolerów, mają mieć pola nullowalne. DTO ma nie mieć pola 'ModifiedDate' //[Note] - DTO i encje są modelami danych, ale DTO jest uproszczony, na poziomie tylko kontrolera, a encja jest modelem pełnym, domenowym
//TODO: dodać w kontrolerach implementację akcji patch dla aktualizacji tylko określonych pól, jeśli nie są nullami
//TODO: wyszukiwanie postów po tytule
//TODO: pageowanie rezultatów zwracanych przez getAll posts //[Note] - możliwe 2 podejścia a) podawać do backend'u rozmiar paczki do zwrotu i wtedy fronend odpowiada za wyznaczanie stron (bardziej elastyczne rozwiązanie), b) podawać do backendu numer strony do zwrotu, a backend wylicza strony (lepsze w naszym przypadku, bo nie mamy frontendu)
