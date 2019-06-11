using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestApiTest.Core.Exceptions;
using RestApiTest.Core.Interfaces.Repositories;
using RestApiTest.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiTest.Controllers
{
    //?? Czy jest jakiś sposób na uogólnienie kontrolerów i czy w ogóle się coś takiego stosuje? (Bo one są bardzo podobne)
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private ICommentRepository repository;
        private ILogger<CommentController> logger;

        public CommentController(ILogger<CommentController> log, ICommentRepository repository)
        {
            logger = log;
            this.repository = repository;
        }

        //GET api/comment/5
        [HttpGet("{id}", Name = "GetComment")] //?? Czy kontroler komentarzy też ma być zrobiony w "standardowy" sposób, skoro raczej nie powinno być dostępne 
                                                    //dla użytkownika dostania pojedynczego komentarza po wpisaniu adresu w przeglądarce
        [ProducesResponseType(typeof(Comment), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<Comment>> Get(long id)
        {
            logger.LogInformation("Calling get for comment with id =" + id);
            Comment comment = await repository.GetAsync(id);
            if (comment != null)
            {
                return Ok(comment);
            }
            else
            {
                logger.LogWarning("No comment with given id exists");
                return NotFound(id);
            }
        }

        // POST api/comment
        [HttpPost]
        [ProducesResponseType(typeof(Comment), StatusCodes.Status201Created)]
        public async Task<IActionResult> Post([FromBody] Comment commentToAdd)
        {
            logger.LogInformation("Calling post for the following object: {@0} ", commentToAdd); 
            await repository.AddAsync(commentToAdd);
            return CreatedAtRoute("GetComment", new { id = commentToAdd.Id }, commentToAdd);
        }

        // PUT api/comment/5
        [HttpPut("{id}")]
        [ActionName("UpdateComment")]
        [ProducesResponseType(typeof(Comment), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Put(long id, [FromBody] Comment updatedComment)
        {
            logger.LogInformation("Calling put for object: {@0}", updatedComment);
            try
            {
                await repository.UpdateAsync(updatedComment);
            }
            catch (BlogPostsDomainException)
            {
                logger.LogWarning("There was nothing to update");
                return NotFound(id);
            }
            return Ok(updatedComment); //?? Zwracać ten updatedComment, czy raczej powinienem zwracać z repo obiekt na nowo zaczytany z bazy 
                                        //(żeby np. wyeliminować przekłamanie, gdyby coś zostało ustawione na bazie inaczej)?
        }

        // DELETE api/comment/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Delete(int id)
        {
            var comment = await repository.GetAsync(id);
            if (comment == null)
            {
                logger.LogWarning("Element with given ID doesn't exist - nothing is deleted");
                return NoContent();
            }

            await repository.DeleteAsync(id);
            logger.LogInformation("Element with given ID has been successfully removed");
            return NoContent();
        }

        [HttpGet("{id}", Name = "GetAllCommentsForPost")] //?? Jak tu powinien wyglądać routing, żeby dało się coś takiego wywołać?
        [ProducesResponseType(typeof(IEnumerable<Comment>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<IEnumerable<Comment>>> GetAllCommentsForPost(long postId)
        {
            logger.LogInformation("Calling get for all comments of given post");
            var comments = await repository.GetAllCommentsForPost(postId);
            long? count = comments?.Count();
            if (count.HasValue && count.Value > 0)
            {
                return Ok(comments);
            }
            else
            {
                logger.LogWarning("No comments to return");
                return NoContent();
            }
        }

        [HttpGet("{id}", Name = "GetAllCommentsForUser")] //?? Jak tu powinien wyglądać routing, żeby dało się coś takiego wywołać?
        [ProducesResponseType(typeof(IEnumerable<Comment>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<IEnumerable<Comment>>> GetAllCommentsForUser(long userId)
        {
            logger.LogInformation("Calling get for all comments of given user");
            var comments = await repository.GetAllCommentsForUser(userId);
            long? count = comments?.Count();
            if (count.HasValue && count.Value > 0)
            {
                return Ok(comments);
            }
            else
            {
                logger.LogWarning("No comments to return");
                return NoContent();
            }
        }
    }
}
