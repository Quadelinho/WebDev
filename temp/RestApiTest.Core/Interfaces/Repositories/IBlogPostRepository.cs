using RestApiTest.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestApiTest.Core.Interfaces.Repositories
{
    public interface IBlogPostRepository : IBaseRepository<BlogPost>, ICommentable, IMarkable
    {
        Task<IEnumerable<BlogPost>> GetAllBlogPostsAsync(); //?? Czy taka forma ma sens, bo wygląda to na trochę bzdurny interfejs. Czy warto w ogóle dodać np. interface rozszerzający IBase o metodę GetAll?
        //?? W takim razie co powinno być zwracane z tasków zamiast IActionResult?
    }
}
