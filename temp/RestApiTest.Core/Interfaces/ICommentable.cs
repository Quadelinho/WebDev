using RestApiTest.Core.Models;
using System.Threading.Tasks;

namespace RestApiTest.Core.Interfaces
{
    //??Czyli, skoro to nie ma być zgodne z zasadami segregacji interfejsów SOLID'a, to znaczy, że ten interface nie ma sensu. Ale jak w takim razie mam oznaczyć w API akcje CRUD'a dla komentarzy?
    public interface ICommentable
    {
    //    Task<Comment/*IActionResult*/> AddCommentAsync(Comment commentToAdd, BlogPost relatedPost, ForumUser author);
    //    Task<IActionResult> DeleteCommentAsync(Comment commentToRemove);
    //    Task<Comment/*IActionResult*/> UpdateCommentAsync(Comment commentToUpdate);
    //    Task<IAsyncEnumerable<Comment>> GetAllCommentsAsync(BlogPost relatedPost);
    }
}
