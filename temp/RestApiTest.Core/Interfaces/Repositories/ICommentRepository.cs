using RestApiTest.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiTest.Core.Interfaces.Repositories
{
    public interface ICommentRepository : IBaseRepository<Comment>
    {
        Task<IQueryable<Comment>> GetAllCommentsForPost(long commentedPostId);
        Task<IQueryable<Comment>> GetAllCommentsForUser(long authorId);
        Task<Comment> ApproveCommentAsync(long id);
        Task<Comment> MarkCommentAsSolutionAsync(long id);
    }
}
