using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RestApiTest.Core.Interfaces.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task AddAsync(T objectToAdd);
        Task DeleteAsync(long id);
        Task UpdateAsync(T objectToUpdate);
        Task<T> GetAsync(long id);
    }
}
