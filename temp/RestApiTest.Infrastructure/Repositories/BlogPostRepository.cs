
using System.Collections.Generic;
using System.Threading.Tasks;
using RestApiTest.Core.Interfaces.Repositories;
using RestApiTest.Core.Models;
using RestApiTest.Infrastructure.Data;

namespace RestApiTest.Core.Repositories
{ //[Note] - nie nie, musi. - Czy każda klasa modelu musi / powinna mieć swoje pokrycie w klasie Repository,czy np. operacje na komentarzach mogą być z poziomu BlogPostRepository (bo komentarze nie mogą przecież być procesowane samodzielnie, w oderwaniu od postów)
    public class BlogPostRepository : IBlogPostRepository
    {
        private ForumContext context;
        public BlogPostRepository(ForumContext context)
        {
            this.context = context;
        }

        public Task AddAsync(BlogPost objectToAdd)
        {
            throw new System.NotImplementedException();
        }

        public Task AddMarkAsync(bool refersToPositive)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync(long id)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<BlogPost>> GetAllBlogPostsAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<long> GetAllMarksOfGivenType(bool refersToPositive)
        {
            throw new System.NotImplementedException();
        }

        public Task<BlogPost> GetAsync(long id)
        {
            throw new System.NotImplementedException();
        }

        public Task RemoveMarkAsync(bool refersToPositive)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateAsync(BlogPost objectToUpdate)
        {
            throw new System.NotImplementedException();
        }
        //TODO: implementacja interfejsu repo

    }
}
