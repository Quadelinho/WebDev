using Microsoft.EntityFrameworkCore;
using RestApiTest.Core.DTO;
using RestApiTest.Core.Exceptions;
using RestApiTest.Core.Interfaces;
using RestApiTest.Core.Interfaces.Repositories;
using RestApiTest.Core.Models;
using RestApiTest.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiTest.Infrastructure.Repositories
{ //[Note] - nie nie, musi. - Czy każda klasa modelu musi / powinna mieć swoje pokrycie w klasie Repository,czy np. operacje na komentarzach mogą być z poziomu BlogPostRepository (bo komentarze nie mogą przecież być procesowane samodzielnie, w oderwaniu od postów)
    public class BlogPostRepository : BaseRepository<BlogPost>, IBlogPostRepository
    {
        //temp \/
        private ForumContext localContext;
        
        public BlogPostRepository(ForumContext context) : base(context)
        {
            //temp \/
            this.context = localContext = context;
        }

        public override async Task<BlogPost> AddAsync(BlogPost objectToAdd, Action additionalPreSteps = null) //[Note] - nie, bo to w założeniach nie są na tyle długotrwałe operacje - czy w praktyce w tego typu operacjach stosuje się cancellation token'y, czy raczej tylko w przypadku jakichś bardzo dużych obiektów  (np. z całego formularza)
        {
            return await base.AddAsync(objectToAdd, () => 
            {
                additionalPreSteps?.Invoke();
                ThrowOnTitleDuplication(objectToAdd.Title, objectToAdd.Id); //?? Które reguły walidacyjne mają być w Validator'ze? Czy walidacja duplikacji tytułu ma być tu, czy przeniesiona do validator'a?
                objectToAdd.UpdateModifiedDate();
            });
        }

        //[Note] - tak, IAsyncEnumerable ma wewnątrz taski i yeld'a, więc samo użycie tego typu już oznacza async'a - Czy jeśli chcę używać IAsyncEnumerable, które ma taski wewnątrz, to znaczy, że samej metody już nie mogę oznaczyć jako async
        public IQueryable<BlogPost> /*Task<*//*IEnumerable*//*IQueryable<BlogPost>*/ GetAllBlogPostsAsync() //=> (IEnumerable<BlogPost>)context.Posts?.ToAsyncEnumerable();
        {
            //return /*await*/ context.Posts;//.ToList();
            return /*await*/ localContext.Posts.Include(p => p.Author)
                .Include(p => p.Comments)
                .Include(p => p.Votes)
                .Select(p => p);
                //.async();// LoadAsync();
        }

        public override async Task<BlogPost> GetAsync(long id)
        {
            //return await context.Posts.Include(p => p.Author).FindAsync(id);
            return await localContext.Posts.Where(p => p.Id == id)
                .Include(p => p.Author)
                .Include(p => p.Comments)
                .Include(p => p.Votes)
                .FirstOrDefaultAsync();
        }

        public override async Task<BlogPost> UpdateAsync(BlogPost objectToUpdate, Action additionalPreSteps = null)
        {
            return await base.UpdateAsync(objectToUpdate, () => 
            {
                additionalPreSteps?.Invoke();
                ThrowOnTitleDuplication(objectToUpdate.Title, objectToUpdate.Id);
                objectToUpdate.UpdateModifiedDate();
            });
        }

        private void ThrowOnTitleDuplication(string titleToCheck, long sourcePostId)
        {
            bool isTitleDuplicate = localContext.Posts.FirstOrDefault(p => p.Title == titleToCheck && p.Id != sourcePostId) != null; //[Note] - at this point First, but it can be changed in future implementations of the framework - Czy FirstOrDefault jest wydajniejsze niż where? Która opcja będzie pchała najmniej zbędnych danych?
            if (isTitleDuplicate)
            {
                throw new BlogPostsDomainException("Update failed - the post with given title already exists");
            }
        }

        public long GetAllMarksOfGivenType(bool refersToPositive, long relatedItemId)
        {
            return (long)localContext.Votes.Count(v => v.VotedPost.Id == relatedItemId && v.IsLike == refersToPositive);
        }

        public async Task AddMarkAsync(Vote voteToAdd)
        {
            if (voteToAdd == null || voteToAdd.VotedPost == null)
            {
                throw new BlogPostsDomainException("Adding mark failed - empty object");
            }

            Vote alreadyExistingVoteEntry = localContext.Votes.Where(v => v.Voter.Id == voteToAdd.Voter.Id && v.VotedPost.Id == voteToAdd.VotedPost.Id) as Vote;
            if (alreadyExistingVoteEntry == null)
            {
                await localContext.Votes.AddAsync(voteToAdd);
                await context.SaveChangesAsync();
            }
            else if (alreadyExistingVoteEntry.IsLike != voteToAdd.IsLike) //Given user has already voted but with opposite value -> update
            {
                alreadyExistingVoteEntry.IsLike = voteToAdd.IsLike;
                await context.SaveChangesAsync(); //?? Co dokładnie robi to drugie przeciążenie, z parametrem bool? - doczytać
            }
        }

        public async Task RemoveMarkAsync(long id)
        {
            Vote voteToRemove = await localContext.Votes.FindAsync(id);
            if (voteToRemove != null)
            {
                IVotable relatedObject = null;
                if (voteToRemove.VotedPost != null)
                {
                    relatedObject = voteToRemove.VotedPost;
                }
                else
                {
                    relatedObject = voteToRemove.VotedComment;
                }
                relatedObject?.RemoveReferenceToVote(voteToRemove.Id);
                context.Remove(voteToRemove);
                await context.SaveChangesAsync(); //[Note] - potencjalnie tak, bo inacej EF domyślnie może się wywalić - Czy tutaj usuwając Vote'a muszę też zaktualizować powiązany z nim obiekt, czy EF sam to już ogarnie w context'cie?
                                                  //TODO: repo vote'ów powinno usuwać zależności wcześniej, a całość ma być wyowłana przez service
            }
        }

        public override async Task<BlogPost> ApplyPatchAsync(BlogPost objectToModify, List<PatchDTO> propertiesToUpdate)
        {
            var properties = propertiesToUpdate.ToDictionary(p => p.PropertyName, p => p.PropertyValue);
            if(properties.ContainsKey("Modified"))
            {
                throw new InvalidOperationException("Attempt to apply patch to restricted property");
            }
            if(properties.ContainsKey("Title"))
            {
                ThrowOnTitleDuplication(objectToModify.Title, objectToModify.Id);
            }

            var entityEntry = context.Entry(objectToModify);
            entityEntry.CurrentValues.SetValues(properties); //[Note] Metoda SetValues ma przydatne przeładowanie, pozwalające przekazać cały obiekt - wówczas wszystkie właściwości o takich samych nazwach zostaną przekopiowane (pozwala to z automatu używać np. obiektów DTO)
            entityEntry.State = Microsoft.EntityFrameworkCore.EntityState.Modified; //?? Na necie widziałem, że definiują ten stan - czy to jakoś przyspiesza operacje, czy jest nadmiarowe, czy może tylko w celach informacyjncyh dla zachowania spójności danych? //TODO: sprawdzić / poszukać, czy to daje jakąś optymalizację
            entityEntry.Entity.UpdateModifiedDate();
            await context.SaveChangesAsync();
            return entityEntry.Entity;
        }

        public IQueryable<BlogPost> GetPostsContaingInTitle(string textToSearch, int pageNo, int postsPerPage, out decimal totalPagesCount)
        {
            IQueryable<BlogPost> foundPosts = null;
            if(String.IsNullOrWhiteSpace(textToSearch))
            {
                foundPosts = localContext.Posts; //[Note] - można uprościć wszystkie wywołania include dla zależności EF (sprawdzić podejście z drugim rodzajem ładowania - lazy loading, gdzie typy referncyjne w modelu muszą być zdefiniowane jako virtual)
            }
            else
            {
                foundPosts = localContext.Posts.Where(p => p.Title.Contains(textToSearch, StringComparison.InvariantCultureIgnoreCase));
            }
            return GetPostsChunk(foundPosts, pageNo, postsPerPage, out totalPagesCount);
        }
        
        public /*async*/ IQueryable<BlogPost> GetBlogPostsChunkAsync(int pageNo, int postsPerPage, out decimal totalPagesCount)
        {
            return GetPostsChunk(localContext.Posts, pageNo, postsPerPage, out totalPagesCount);
        }
        
        private IQueryable<BlogPost> GetPostsChunk(IQueryable<BlogPost> postsCollection, int pageNo, int postsPerPage, out decimal totalPages)
        {
            IQueryable<BlogPost> chunkToReturn = Enumerable.Empty<BlogPost>().AsQueryable();
            int? postsToSkip = null;
            decimal totalPostsInCollection = postsCollection != null ? postsCollection.Count() : 0;
            if (totalPostsInCollection > 0)
            {
                totalPages = Math.Ceiling(totalPostsInCollection / postsPerPage);
                int numberOfPostsToTake = postsPerPage > 0 ? postsPerPage : 1;
                postsToSkip = pageNo * numberOfPostsToTake;
                chunkToReturn = postsCollection
                    .Skip(postsToSkip.Value)
                    .Take(numberOfPostsToTake)
                    .Include(p => p.Author)
                    .Include(p => p.Comments)
                    .Include(p => p.Votes);
            }
            else
            {
                totalPages = 0;
            }
            return chunkToReturn;
        }
    }
}
