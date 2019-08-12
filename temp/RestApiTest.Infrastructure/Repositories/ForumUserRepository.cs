using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestApiTest.Core.DTO;
using RestApiTest.Core.Interfaces.Repositories;
using RestApiTest.Core.Models;
using RestApiTest.Infrastructure.Data;

namespace RestApiTest.Infrastructure.Repositories
{
    public class ForumUserRepository : BaseRepository<ForumUser>, IForumUserRepository
    {
        private ForumContext localContext;

        public ForumUserRepository(ForumContext context):base(context)
        {
            this.context = localContext = context;
        }

        //TODO: implementacja pozostałych repozytoriów i kontrolerów
        public override async Task<ForumUser> AddAsync(ForumUser objectToAdd, Action additionalPreSteps = null)
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
            ForumUser user = await localContext.Users.FindAsync(id);
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

        public Task<IEnumerable<ForumUser>> GetAllUsersAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<ForumUser> GetAsync(long id)
        {
            throw new System.NotImplementedException();
        }

        public override async Task<ForumUser> UpdateAsync(ForumUser objectToUpdate, Action additionalPreSteps = null)
        {
            throw new System.NotImplementedException();
        }

        public async Task UpdateLastLoginDate(long id)
        {
            ForumUser user = await localContext.Users.FindAsync(id);
            if (user != null)
            {
                user.LastLoggedIn = DateTime.UtcNow;
                await context.SaveChangesAsync();
            }
        }
    }
}
