using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestApiTest.Data;
using RestApiTest.Exceptions;
using RestApiTest.Models;
using Serilog;
using Serilog.Core;

namespace RestApiTest.Controllers
{
    [Route("api/[controller]")] //[note] gdzie można podejrzeć dostępne tokeny, takie jak ten? - w dokumentacji na MSDN
    [ApiController]
    public class BlogController : ControllerBase
    {
        private BlogDBContext context;
        //private Logger logger;
        //ILogger<BlogController> logger;
        ILogger<BlogController> logger;

        public BlogController(ILogger<BlogController> log/*ILogger<BlogController> log*/,  BlogDBContext ctx, IHostingEnvironment environment)
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
            var obj = await context.BlogPosts.ToListAsync();
            if (obj != null)
            {
                return Ok(obj);
            }
            else
            {
                return NoContent();
            }
        }

        //GET api/blog/5
        [HttpGet("{id}", Name = "GetBlog")]
        //?? Czy to można zdefiniować, żeby podawać wartość w parametrze typu "...?id=5"? //??Jak wywołać to z użyciem tego ActionName? ODP: tak, ale trzeba by zmienić routing
        public async Task<ActionResult<BlogPost>> Get(long id)
        {
            BlogPost obj = await context.BlogPosts.FindAsync(id); //?? Wykonanie wpadło tutaj zaraz jak tylko wpisałem wartość w pasku adresowym, bez zatwierdzenia - czy to jakaś forma cache'owania a'priori przeglądarki (po kliknięciu enter już od razu dostałem wynik)? Czy da się wymusić, żeby nie były robione takie operacje dopóki user nie wciśnie enter?
            if (obj != null)
            {
                return Ok(obj);
            }
            else
            {
                return NotFound(id);
            }
        }

        // POST api/blog
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BlogPost value) //[note] Czy tutaj to FromBody jest konieczne? Czy domyślnie typy złożone nie powinny być odczytywane z body? ODP: nie jest konieczne, bo domyślnie są odczytywane z body, ale poprawia czytelność
        {
           
                context.BlogPosts.Add(value);
                //try
                //{
                    await context.SaveChangesAsync();
                    return CreatedAtRoute("GetBlog", new { id = value.Id }, value); //[note] W jaki sposób przerobić to na pojedynczy punkt wyjścia? Czy jest jakiś typ wspólny dla tych helpersów i czy tak się w ogóle robie w web dev'ie? ODP: nie stosuje się tego podejścia w aplikacjach web'owych
                //}
                //catch (Exception)
                //{
                //    return StatusCode(500); 
                //}
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
            var post = await context.BlogPosts.FindAsync(id);
            if(post == null)
            {
                return NotFound(id);
                //throw new BlogPostsDomainException("There is no post with given id");
            }
            post.Title = updatedPost.Title;

            //try //[Note] raczej nie ma potrzeby urzywać tu try-catch -> zazwyczaj w web dev'ie stosuje się podejście global exception handler'a
            //{
                await context. SaveChangesAsync(); //?? Co dokładnie robi to drugie przeciążenie, z parametrem bool? //Użyć Update, żeby nie zmieniać całego kontekstu
            
            //?? Co tu ma być zwrócone, żeby było zgodne z HATEOS'em (żeby zwróciło w responsie url'a?)
            return Ok(post);//Może być NoContent
            
            
            //}
            //catch (Exception)
            //{
            //    //return StatusCode(500); //?? Czy jeśli nie zwrócę w tej sytuacji niczego, lub nie przechwycę wyjątku, to przeglądarka z automatu zinterpretuje to jako 500?
            //}
        }

        // DELETE api/blog/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)] //TODO: ProduceResponse dla pozostałych endpointów
        public async Task<ActionResult> Delete(int id)
        {
            var post = await context.BlogPosts.FindAsync(id);
            if(post == null)
            {
                return NoContent();
            }
           // try
            //{
                context.Remove(post);
                await context.SaveChangesAsync();
                return NoContent();
            //}
            //catch (Exception)
            //{
            //    return StatusCode(500);
            //}
        }
        //DONE: global exception handler
        //TODO: swagger documentation (Swagger UI) //?? Czy jest jakiś pakiet instalujący od razu wszystkie potrzebne komponenty Swagger'a, czy trzeba to zawsze instalować osobno (3 pakiety)?

        private List<BlogPost> CreateSampleData(int requiredNumberOfSamples)
        {
            List<BlogPost> posts = new List<BlogPost>();
            for (int postIndex = 1; postIndex <= requiredNumberOfSamples; ++postIndex)
            {
                posts.Add(new BlogPost()
                {
                    Author = "User" + postIndex,
                    Content = DateTime.Now.ToShortDateString(),
                    Modified = DateTime.Now,
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