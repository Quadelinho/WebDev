using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestApiTest.Core.DTO;
using RestApiTest.Core.Interfaces.Repositories;
using RestApiTest.Core.Models;
using RestApiTest.Infrastructure.Data;

namespace RestApiTest.Infrastructure.Repositories
{
    public class ForumUserRepository : IForumUserRepository
    {
        private ForumContext context;

        public ForumUserRepository(ForumContext context)
        {
            this.context = context;
        }

        //TODO: implementacja pozostałych repozytoriów i kontrolerów
        public Task<ForumUser> AddAsync(ForumUser objectToAdd)
        {
            objectToAdd.SetInitialValues();//SetUserRegistrationDate();

            throw new System.NotImplementedException();
        }

        public Task<ForumUser> ApplyPatchAsync(ForumUser objectToModify, List<PatchDTO> propertiesToUpdate)
        {
            throw new System.NotImplementedException();
        }

        public async Task ConfirmUserAccountAsync(long id)
        {
            ForumUser user = await context.Users.FindAsync(id);
            if(user != null)
            {
                user.Confirm();
                await context.SaveChangesAsync();
            }
        }

        public Task<ForumUser> ConfirmUserAccountAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync(long id)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<ForumUser>> GetAllUsersAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<ForumUser> GetAsync(long id)
        {
            throw new System.NotImplementedException();
        }

        public Task<ForumUser> UpdateAsync(ForumUser objectToUpdate)
        {
            throw new System.NotImplementedException();
        }

        public async Task UpdateLastLoginDate(long id)
        {
            ForumUser user = await context.Users.FindAsync(id);
            if (user != null)
            {
                user.LastLoggedIn = DateTime.UtcNow;
                await context.SaveChangesAsync();
            }
        }
    }
}
