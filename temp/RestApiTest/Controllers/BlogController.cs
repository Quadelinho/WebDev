using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestApiTest.Data;
using RestApiTest.Models;
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
        private BlogDBContext context;
        ILogger<BlogController> logger;

        public BlogController(ILogger<BlogController> log,  BlogDBContext ctx, IHostingEnvironment environment)
        {
            logger = log;
            logger.LogError("Sample error {0}, {@1}", environment, new { value = 1, value2 ="test" });
            context = ctx;
            if(environment.IsDevelopment() && ctx.BlogPosts.Count() <= 0)
            {
                ctx.BlogPosts.AddRange(CreateSampleData(5));
                ctx.SaveChanges();
            }
        }

        //GET api/blog
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BlogPost>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<IEnumerable<BlogPost>>> Get()
        {
            logger.LogInformation("Calling get for all objects");
            var obj = await context.BlogPosts.ToListAsync();
            if (obj != null)
            {
                return Ok(obj);
            }
            else
            {
                logger.LogWarning("There is no blog post element to be returned");
                return NoContent();
            }
        }

        //GET api/blog/5
        [HttpGet("{id}", Name = "GetBlog")] //[Note] Co daje ta nazwa? Odwołanie się przez nią mi nie przechodzi - nie do url, tylko alias na potrzeby kodu
                                            //[Note] Czy to można zdefiniować, żeby podawać wartość w parametrze typu "...?id=5"? //??Jak wywołać to z użyciem tego ActionName? ODP: tak, ale trzeba by zmienić routing
        [ProducesResponseType(typeof(BlogPost), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<BlogPost>> Get(long id)
        {
            logger.LogInformation("Calling get for object with id =" + id);
            BlogPost obj = await context.BlogPosts.FindAsync(id); //?? Wykonanie wpadło tutaj zaraz jak tylko wpisałem wartość w pasku adresowym, bez zatwierdzenia - czy to jakaś forma cache'owania a'priori przeglądarki (po kliknięciu enter już od razu dostałem wynik)? Czy da się wymusić, żeby nie były robione takie operacje dopóki user nie wciśnie enter?
            if (obj != null)
            {
                return Ok(obj);
            }
            else
            {
                logger.LogWarning("No blog post with given id exists");
                return NotFound(id);
            }
        }

        // POST api/blog
        [HttpPost]
        [ProducesResponseType(typeof(BlogPost), StatusCodes.Status201Created)] //[Note] Co definiuje się w takich przypadkach jako typ zwracany? Muszę podawać zawsze typ rzeczywisty, bo interface nie może być obiektem typeof? ODP: tak, podaje się typ rzeczywisty
        public async Task<IActionResult> Post([FromBody] BlogPost value) //[note] Czy tutaj to FromBody jest konieczne? Czy domyślnie typy złożone nie powinny być odczytywane z body? ODP: nie jest konieczne, bo domyślnie są odczytywane z body, ale poprawia czytelność
        {
            logger.LogInformation("Calling post for the following object: {@0} ", value); //?? Czy przy tym nie ma tej automatycznej weryfikacji modelu? W body post'a miałem więcej pól i wszystko przeszło. Czy da się wymusić kontrolę 1:1 (żeby body było w 100% zgodne z modelem?
 //           value.Modified = DateTime.Now.ToLongDateString();
            context.BlogPosts.Add(value);
            await context.SaveChangesAsync();
            return CreatedAtRoute("GetBlog", new { id = value.Id }, value); //[note] W jaki sposób przerobić to na pojedynczy punkt wyjścia? Czy jest jakiś typ wspólny dla tych helpersów i czy tak się w ogóle robie w web dev'ie? ODP: nie stosuje się tego podejścia w aplikacjach web'owych
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
            var post = await context.BlogPosts.FindAsync(id);
            if(post == null)
            {
                logger.LogWarning("There was nothing to update");
                return NotFound(id);
                //throw new BlogPostsDomainException("There is no post with given id");
            }
            post.Title = updatedPost.Title;
//            post.Modified = DateTime.Now.ToLongDateString();

            //try //[Note] raczej nie ma potrzeby urzywać tu try-catch -> zazwyczaj w web dev'ie stosuje się podejście global exception handler'a
                await context. SaveChangesAsync(); //?? Co dokładnie robi to drugie przeciążenie, z parametrem bool? //Użyć Update, żeby nie zmieniać całego kontekstu
            
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
            var post = await context.BlogPosts.FindAsync(id);
            if (post == null)
            {
                logger.LogWarning("Element with givrn ID doesn't exist - nothing is deleted");
                return NoContent();
            }

            context.Remove(post);
            await context.SaveChangesAsync();
            logger.LogInformation("Element with given ID has been successfully removed");
            return NoContent(); //[Note] Jak zwrócić element usunięty? Ma być wtedy zwracany status OK z obiektem? [OK + obiekt]
        }
        //DONE: global exception handler
        //DONE: swagger documentation (Swagger UI)

        private List<BlogPost> CreateSampleData(int requiredNumberOfSamples)
        {
            List<BlogPost> posts = new List<BlogPost>();
            for (int postIndex = 1; postIndex <= requiredNumberOfSamples; ++postIndex)
            {
                posts.Add(new BlogPost()
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