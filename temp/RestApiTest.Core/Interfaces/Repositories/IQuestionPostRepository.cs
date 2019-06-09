using RestApiTest.Core.Models;
using System.Threading.Tasks;

namespace RestApiTest.Core.Interfaces.Repositories
{
    public interface IQuestionPostRepository : IBlogPostRepository
    {
        Task<Tag> AddTagAsync(Tag tagToAdd, BlogPost relatedPost);
        Task<Tag> RemoveTagAsync(long id, BlogPost relatedPost);
        //??Co powinno być zwrócone z poniższej akcji, jeśli nie IActionResult
        Task SetSolvedStatus(QuestionPost postToUpdate, bool isSolved); //[Note] tak, bo jest to bezpośrednio operacja na danych, możliwa do ogarnięcia prostym selectem, a nie logika biznesowa - Czy to ma tu sens, czy raczej powinno być ustawiane przez POST'a ogólnego dla tego obiektu?
    }
}
