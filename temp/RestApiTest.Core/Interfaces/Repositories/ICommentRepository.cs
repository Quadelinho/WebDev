using RestApiTest.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestApiTest.Core.Interfaces.Repositories
{
    public interface ICommentRepository : IBaseRepository<Comment>
    {
        Task<IEnumerable<Comment>> GetAllCommentsForPost(long commentedPostId);
        Task<IEnumerable<Comment>> GetAllCommentsForUser(long authorId);
    }
}
