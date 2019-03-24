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

        //todo: GET api/blog
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogPost>>> Get()
        {
            var obj = await context.BlogPosts.ToListAsync();//.ToAsyncEnumerable();
            if (obj != null)
            {
                return Ok(obj); // TODO: Done make async
            }
            else
            {
                return NoContent();
            }
        }

        //todo: GET api/blog/5
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
            //todo: Done statuses: 200, 404
        }

        // POST api/blog
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BlogPost value) //?? Czy tutaj to FromBody jest konieczne? Czy domyślnie typy złożone nie powinny być odczytywane z body?
        {
            if (value != null)
            {
                context.BlogPosts.Add(value);
                try
                {
                    await context.SaveChangesAsync();
                    //todo: Done - statuses: 404, 201 (CreatedAtRoute();
                    return CreatedAtRoute("GetBlogPost", value); //?? W jaki sposób przerobić to na pojedynczy punkt wyjścia? Czy jest jakiś typ wspólny dla tych helpersów i czy tak się w ogóle robie w web dev'ie?
                }
                catch (Exception)
                {
                    return StatusCode(500); 
                }
            }
            else
            {
                return NotFound(value); //?? Co robi ta wartość przekazana w ten sposób? Wyświetla jakiego obiektu nie udało się znaleźć?
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
            //todo: Done - todo + async
            //todo: Done - status 404, 200
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
            //todo: Done - todo + async
            //todo: Done - status 404, 200, 204 (np content)
        }
        //TODO: global exception handler
        //TODO: swagger documentation (Swagger UI)
    }
}
