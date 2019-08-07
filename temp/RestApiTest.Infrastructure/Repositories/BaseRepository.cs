using Microsoft.EntityFrameworkCore;
using RestApiTest.Core.DTO;
using RestApiTest.Core.Exceptions;
using RestApiTest.Core.Interfaces.Repositories;
using RestApiTest.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApiTest.Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        //protected ForumContext context;
        protected DbContext context;
        public BaseRepository(DbContext context)
        {
            this.context = context;
        }

        public virtual async Task<T> AddAsync(T objectToAdd, Action additionalSteps = null)
        {
            if (objectToAdd == null)
            {
                throw new BlogPostsDomainException("Failed to add comment - empty object");
            }

            //[Note] - tak, nic nie stoi na przeszkodzie i korzysta się z tego w praktyce - Czy stosuje się w praktyce przekazywanie do generycznego bazowego repo akcji / delegatów pozwalających definiować np. dodatkowe przypisania warotści poza tym co jest w metodzie bazowej?
            //objectToAdd.SetInitialValues();

            var obj = await context.Set<T>().FindAsync(objectToAdd); //TODO: Używać DbContext i z niego brać context.Set<T> - samo weźmie odpowiednią kolekcję

            additionalSteps();
            //ForumUser author = await context.Users.FindAsync(objectToAdd.Author.Id);
            //if (author != null && author.IsConfirmed)
            //{
            //    objectToAdd.Author = author;
            //}
            //else
            //{
            //    throw new AuthorNotFoundException("Comment cannot be applied because user does not exist or is not confirmed");
            //}

            //await context.PostComments.AddAsync(objectToAdd); //[Note] - stosuje się bazową klasę DbContext i wywołuje generyczną metodę Set<T>(), która zwraca odpowiednią kolekcję. Ma to narzut, ale korzysta się z tego w praktyce - Jak na potrzeby bazowego repo rozróżniać co z contextu ma być używane - refleksją szukać pasującego typu? Czy to nie jest za duży narzut?
            //await context.SaveChangesAsync();
            //return objectToAdd; 
            throw new NotImplementedException();
        }
        
        public virtual Task<T> ApplyPatchAsync(T objectToModify, List<PatchDTO> propertiesToUpdate)
        {
            throw new NotImplementedException();
        }

        public virtual Task DeleteAsync(long id)
        {
            throw new NotImplementedException();
        }

        public virtual Task<T> GetAsync(long id)
        {
            throw new NotImplementedException();
        }

        public virtual Task<T> UpdateAsync(T objectToUpdate)
        {
            throw new NotImplementedException();
        }
    }
}
