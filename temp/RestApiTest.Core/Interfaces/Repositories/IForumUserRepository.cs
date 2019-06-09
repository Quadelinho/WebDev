using RestApiTest.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestApiTest.Core.Interfaces.Repositories
{
    public interface IForumUserRepository : IBaseRepository<ForumUser>
    {
        Task<IEnumerable<ForumUser>> GetAllUsersAsync(); //[Note] - tak, docelowo będzie, tylko w innej warstwie - czy tutaj nie przydałoby się też coś tak jak to IQueryable?
    }
}
