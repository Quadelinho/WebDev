using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestApiTest.Data;
using RestApiTest.Models;

namespace RestApiTest.Controllers
{
    [Route("api/blog")] //?? gdzie można podejrzeć dostępne tokeny, takie jak ten?
    [ApiController]
    public class BlogController : ControllerBase
    {
        private BlogDBContext context;

        public BlogController(BlogDBContext ctx)
        {
            context = ctx;
            //todo: initialize database with basic data in debug

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
        [HttpGet("{id}")]
        [ActionName("GetBllogPost")]
        public async Task<ActionResult<BlogPost>> Get(int id)
        {
            BlogPost obj = await context.BlogPosts.FindAsync(id);
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
            if (value != null)
            {
                context.BlogPosts.Add(value);
                try
                {
                    await context.SaveChangesAsync();
                    return CreatedAtRoute("GetBlogPost", value); //[note] W jaki sposób przerobić to na pojedynczy punkt wyjścia? Czy jest jakiś typ wspólny dla tych helpersów i czy tak się w ogóle robie w web dev'ie? ODP: nie stosuje się tego podejścia w aplikacjach web'owych
                }
                catch (Exception)
                {
                    return StatusCode(500); 
                }
            }
            else
            {
                return NotFound(value); //[note] Co robi ta wartość przekazana w ten sposób? Wyświetla w response'ie szczegóły przekazanego obiektu
            }
        }

        // PUT api/blog/5
        [HttpPut("{id}")]
        [ActionName("UpdatePostTitle")]
        // TODO [ResponseType(HttpStatus.Ok, typeof(BlogPost))]
        public async Task<ActionResult> Put(int id, [FromBody] BlogPost updatedPost)
        {

            //TODO: przyjąć model blogppost, wyszukać 
            var post = await context.BlogPosts.FindAsync(id);
            if(post == null)
            {
                return NotFound(id);
            }
            post.Title = updatedPost.Title;

            try
            {
                await context.SaveChangesAsync(); //?? Co dokładnie robi to drugie przeciążenie, z parametrem bool?
                return Ok(post);
            }
            catch (Exception)
            {
                return StatusCode(500); //?? Czy jeśli nie zwrócę w tej sytuacji niczego, lub nie przechwycę wyjątku, to przeglądarka z automatu zinterpretuje to jako 500?
            }
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
    }
}

//Map:
//[note]