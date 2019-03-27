using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestApiTest.Data;
using RestApiTest.Models;

namespace RestApiTest.Controllers
{
    [Route("api/blog")] //[note] gdzie można podejrzeć dostępne tokeny, takie jak ten? - w dokumentacji na MSDN
    [ApiController]
    public class BlogController : ControllerBase
    {
        private BlogDBContext context;

        public BlogController(BlogDBContext ctx, IHostingEnvironment environment)
        {
            context = ctx;
            //todo: initialize database with basic data in debug
            if(environment.IsDevelopment() && ctx.BlogPosts.Count() <= 0)
            {
                ctx.BlogPosts.AddRange(CreateSampleData(5));
                ctx.SaveChanges();
            }
        }

        //GET api/blog
        [HttpGet]
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
                try
                {
                    await context.SaveChangesAsync();
                    return CreatedAtRoute("GetBlog", new { id = value.Id }, value); //[note] W jaki sposób przerobić to na pojedynczy punkt wyjścia? Czy jest jakiś typ wspólny dla tych helpersów i czy tak się w ogóle robie w web dev'ie? ODP: nie stosuje się tego podejścia w aplikacjach web'owych
                }
                catch (Exception)
                {
                    return StatusCode(500); 
                }
        }

        // PUT api/blog/5
        [HttpPut("{id}")]
        [ActionName("UpdatePostTitle")]
        // TODO [ResponseType(HttpStatus.Ok, typeof(BlogPost))]
        public async Task<ActionResult> Put(int id, [FromBody] BlogPost updatedPost)
        {

             
            var post = await context.BlogPosts.FindAsync(id);
            if(post == null)
            {
                return NotFound(id);
            }
            post.Title = updatedPost.Title; //?? Czy tutaj miałem zrobić coś jeszcze w ramach zadania domowego, z tym wyszukaniem przez obiekt modelu?

            //try
            //{
                await context. SaveChangesAsync(); //?? Co dokładnie robi to drugie przeciążenie, z parametrem bool? //Użyć Update, żeby nie zmieniać całego kontekstu
                return Ok(post);//Może być NoContent
            //}
            //catch (Exception)
            //{
            //    //return StatusCode(500); //?? Czy jeśli nie zwrócę w tej sytuacji niczego, lub nie przechwycę wyjątku, to przeglądarka z automatu zinterpretuje to jako 500?
            //}
        }

        // DELETE api/blog/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var post = await context.BlogPosts.FindAsync(id);
            if(post == null)
            {
                return NoContent();
            }
            try
            {
                context.Remove(post);
                await context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
        //TODO: global exception handler
        //TODO: swagger documentation (Swagger UI)

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