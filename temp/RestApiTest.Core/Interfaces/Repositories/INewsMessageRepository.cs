using RestApiTest.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestApiTest.Core.Interfaces.Repositories
{
    public interface INewsMessageRepository : IBaseRepository<NewsMessage>
    {
        //[Note] - tak, taki wspólny interface generyczny jest zalecany - czy stosuje się w praktyce np. jeden ogólny interface do repo typu np. ICRUD z metodami takimi jak Task<IActionResult> DeleteAsync(long id)?
        Task<IEnumerable<NewsMessage>> GetAllMessagesAsync();
    }
}
 