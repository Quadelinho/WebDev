using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestApiTest.Core.Interfaces.Repositories;
using RestApiTest.Core.Models;
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
        ILogger<BlogController> logger;

        public BlogController(ILogger<BlogController> log, IBlogPostRepository repository, IHostingEnvironment environment)
        {
            logger = log;
            //Slogger.LogError("Sample error {0}, {@1}", environment, new { value = 1, value2 ="test" });
            this.repository = repository;
            var posts = repository.GetAllBlogPostsAsync();
            if (posts == null || posts.Result.Count() == 0)
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
        [ProducesResponseType(typeof(BlogPost), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<BlogPost>> Get(long id)
        {
            logger.LogInformation("Calling get for object with id =" + id);
            BlogPost post = await repository.GetAsync(id);
            if (post != null)
            {
                return Ok(post);
            }
            else
            {
                logger.LogWarning("No blog post with given id exists");
                return NotFound(id);
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BlogPost>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetAll()
        {
            //?? Czy to powinno też zwracać obiekty klasy pochodnej (QuestionPost), bo skoro są pochodnymi to są też postami (domenowo tak samo - pytanie też jest postem)
            logger.LogInformation("Calling get for all posts");
            var posts = await repository.GetAllBlogPostsAsync(); //?? Jak tutaj się robi jakieś "porcjowanie", albo coś w rodzaju yield'a, żeby nie pchać dużej paczki w response'ie/
            long? count = posts?.Count();
            if(count.HasValue && count.Value > 0)
            {
                return Ok(posts);
            }
            else
            {
                logger.LogWarning("No posts to return");
                return NoContent();
            }
        }

        // POST api/blog
        [HttpPost]
        [ProducesResponseType(typeof(BlogPost), StatusCodes.Status201Created)] //[Note] Co definiuje się w takich przypadkach jako typ zwracany? Muszę podawać zawsze typ rzeczywisty, bo interface nie może być obiektem typeof? ODP: tak, podaje się typ rzeczywisty
        public async Task<IActionResult> Post([FromBody] BlogPost postToAdd) //[note] Czy tutaj to FromBody jest konieczne? Czy domyślnie typy złożone nie powinny być odczytywane z body? ODP: nie jest konieczne, bo domyślnie są odczytywane z body, ale poprawia czytelność
        {
            logger.LogInformation("Calling post for the following object: {@0} ", postToAdd); //?? Czy przy tym nie ma tej automatycznej weryfikacji modelu? W body post'a miałem więcej pól i wszystko przeszło. Czy da się wymusić kontrolę 1:1 (żeby body było w 100% zgodne z modelem?
//           postToAdd.Modified = DateTime.Now.ToLongDateString();
            await repository.AddAsync(postToAdd);
            return CreatedAtRoute("GetBlog", new { id = postToAdd.Id }, postToAdd); //[note] W jaki sposób przerobić to na pojedynczy punkt wyjścia? Czy jest jakiś typ wspólny dla tych helpersów i czy tak się w ogóle robie w web dev'ie? ODP: nie stosuje się tego podejścia w aplikacjach web'owych
        }

        // PUT api/blog/5
        [HttpPut("{id}")]
        [ActionName("UpdatePostTitle")]
        [ProducesResponseType(typeof(BlogPost), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BlogPost), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Put(long id, [FromBody] BlogPost updatedPost)
        {
            logger.LogInformation("Calling put for object: {@0}", updatedPost);
            var post = await repository.GetAsync(updatedPost.Id);
            if(post == null)
            {
                logger.LogWarning("There was nothing to update");
                return NotFound(id);
                //throw new BlogPostsDomainException("There is no post with given id");
            }
            post.Title = updatedPost.Title;
//            post.Modified = DateTime.Now.ToLongDateString();

            //try //[Note] raczej nie ma potrzeby używać tu try-catch -> zazwyczaj w web dev'ie stosuje się podejście global exception handler'a
             //Użyć Update, żeby nie zmieniać całego kontekstu
            
            //?? Co tu ma być zwrócone, żeby było zgodne z HATEOS'em (żeby zwróciło w responsie url'a?)
            return Ok(post);//Może być NoContent
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
        //DONE: global exception handler
        //DONE: swagger documentation (Swagger UI)

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