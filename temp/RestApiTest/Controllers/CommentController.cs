using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestApiTest.Core.Exceptions;
using RestApiTest.Core.Interfaces.Repositories;
using RestApiTest.Core.Models;
using RestApiTest.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiTest.Controllers
{
    //[Note] - raczej się nie stosuje generalizacji w tym kontekście - Czy jest jakiś sposób na uogólnienie kontrolerów i czy w ogóle się coś takiego stosuje? (Bo one są bardzo podobne)
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private ICommentRepository repository;
        private ILogger<CommentController> logger;
        private IMapper mappingProvider;

        public CommentController(ILogger<CommentController> log, ICommentRepository repository, IMapper mapper)
        {
            logger = log;
            this.repository = repository;
            mappingProvider = mapper;
        }

        //GET api/comment/5
        [HttpGet("{id}", Name = "GetComment")] //?? Czy kontroler komentarzy też ma być zrobiony w "standardowy" sposób, skoro raczej nie powinno być dostępne 
                                                    //dla użytkownika dostania pojedynczego komentarza po wpisaniu adresu w przeglądarce
        [ProducesResponseType(typeof(CommentDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CommentDTO>> Get(long id)
        {
            logger.LogInformation("Calling get for comment with id =" + id);
            Comment comment = await repository.GetAsync(id);
            if (comment != null)
            {
                return Ok(mappingProvider.Map<CommentDTO>(comment));
            }
            else
            {
                logger.LogWarning("No comment with given id exists");
                return NotFound(id);
            }
        }

        // POST api/comment
        [HttpPost]
        [ProducesResponseType(typeof(CommentDTO), StatusCodes.Status201Created)]
        public async Task<IActionResult> Post([FromBody] CommentDTO commentToAdd)
        {
            logger.LogInformation("Calling post for the following object: {@0} ", commentToAdd); 
            Comment insertedComment = await repository.AddAsync(mappingProvider.Map<Comment>(commentToAdd));
            CommentDTO commentToReturn = mappingProvider.Map<CommentDTO>(insertedComment);
            return CreatedAtRoute("GetComment", new { id = commentToReturn.Id }, commentToReturn);
        }

        // PUT api/comment/5
        [HttpPut("{id}")]
        [ActionName("UpdateComment")]
        [ProducesResponseType(typeof(CommentDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CommentDTO), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Put(long id, [FromBody] CommentDTO updatedComment)
        {
            logger.LogInformation("Calling put for object: {@0}", updatedComment);
            try
            {
                Comment modifiedComment = await repository.UpdateAsync(mappingProvider.Map<Comment>(updatedComment));
                return Ok(mappingProvider.Map<CommentDTO>(modifiedComment));
                //[Note] - zwracać zawsze aktualny status z bazy - Zwracać ten updatedComment, czy raczej powinienem zwracać z repo obiekt na nowo zaczytany z bazy 
                //(żeby np. wyeliminować przekłamanie, gdyby coś zostało ustawione na bazie inaczej)?
            }
            catch (BlogPostsDomainException)
            {
                logger.LogWarning("There was nothing to update");
                return NotFound(id);
            }
        }
        //[Note] aktualizacja tylko jednego pola - metoda patch

        // DELETE api/comment/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Delete(int id)
        {
            Comment comment = await repository.GetAsync(id);
            if (comment == null)
            {
                logger.LogWarning("Element with given ID doesn't exist - nothing is deleted");
                return NoContent();
            }

            await repository.DeleteAsync(id);
            logger.LogInformation("Element with given ID has been successfully removed");
            return Ok();
        }

        //?? Nie widzi tutaj nazwy controller'a automatycznie - jeśli nie podałem poniżej jawnie /comment/ to metoda była wywoływana przy wywołaniu get z BlogPost/id
        [HttpGet("/posts/{id}/comments/", Name = "GetAllCommentsForPost")] //[Note] Jak tu powinien wyglądać routing, żeby dało się coś takiego wywołać? //Done: składnia definiowania routingu -> po id - powinno być coś takiego jak posts/id/comments/ (zgodnie z zasadmi rest'a, żeby nie sugerowało, że to id komentarza) - dokumentacja: https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-2.2#attribute-routing
        [ProducesResponseType(typeof(IEnumerable<CommentDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<IEnumerable<CommentDTO>>> GetAllCommentsForPost(long postId)
        {
            logger.LogInformation("Calling get for all comments of given post");
            var comments = await repository.GetAllCommentsForPost(postId);
            long? count = comments?.Count();
            if (count.HasValue && count.Value > 0)
            {
                return Ok(mappingProvider.ProjectTo<CommentDTO>(comments));
            }
            else
            {
                //logger.LogInfo("No comments to return");
                return NoContent();
            }
        }

        [HttpGet("/users/{id}/comments/", Name = "GetAllCommentsForUser")]
        [ProducesResponseType(typeof(IEnumerable<CommentDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<IEnumerable<CommentDTO>>> GetAllCommentsForUser(long userId)
        {
            logger.LogInformation("Calling get for all comments of given user");
            var comments = await repository.GetAllCommentsForUser(userId); 
            long? count = comments?.Count();
            if (count.HasValue && count.Value > 0)
            {
                return Ok(mappingProvider.ProjectTo<CommentDTO>(comments));
            }
            else
            {
                //logger.LogWarning("No comments to return"); //TODO: logowanie dla debug'a tego typu wpisów zbędnych
                return NoContent();
            }
        }

        [HttpPut("/comments/{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> ApproveComment(long id)
        {
            try
            {
                await repository.ApproveCommentAsync(id);
                return Ok();
            }
            catch (System.Exception)
            {
                return NotFound(id);
            }
        }

        [HttpPut("/comments/markSolution/{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> MarkCommentAsSolution(long id)
        {
            try
            {
                await repository.MarkCommentAsSolutionAsync(id);
                return Ok();
            }
            catch (System.Exception)
            {
                return NotFound(id);
            }
        }

        //TODO: Metody do pobierania wszystkich zatwierdzonych lub niezatwierdzonych komentarzy (lub parametr do metody get)
    }
}
