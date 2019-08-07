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
        //private ForumContext context;
        
        public BlogPostRepository(ForumContext context) : base(context)
        {
            //this.context = context;
        }

        public override async Task<BlogPost> AddAsync(BlogPost objectToAdd) //[Note] - nie, bo to w założeniach nie są na tyle długotrwałe operacje - czy w praktyce w tego typu operacjach stosuje się cancellation token'y, czy raczej tylko w przypadku jakichś bardzo dużych obiektów  (np. z całego formularza)
        {
            if (objectToAdd == null)
            {
                throw new ArgumentNullException("Failed to insert the post - empty entry, blogPost"); //[Note] - skoro to metoda publiczna, to lepiej robić walidację danych wejściowych też na tym etapie, nawet jeśli miałyby być zwielokrotnione te walidacje - czy takie podejście się stosuje, czy raczej się zwraca po prostu "pusty resultat"?
                                                                                                      //A może za kontrolę danych wejściowych powinien już odpowiadać kotroler, zanim zaangażuje wewnętrzne klasy?
            }

            bool isTitleDuplicate = context.Posts.FirstOrDefault(p => p.Title == objectToAdd.Title) != null;
            if (isTitleDuplicate)
            {
                throw new BlogPostsDomainException("Update failed - the post with given title already exists");
            }

            objectToAdd.UpdateModifiedDate();
            await context.Posts.AddAsync(objectToAdd); //[Note] - w rest trzeba zwracać error code + explanation; zależnie czy jest zdefiniowane wymaganie od klienta, jeśli nie - można dla global handlera - Czy w web'ówce stosuje się w tym punkcie kontrolę czy wpis już istnieje, czy to po prostu powinien złapać global handler?
            await context.SaveChangesAsync();
            return objectToAdd;
        }

        public override async Task DeleteAsync(long id)
        {
            BlogPost postToRemove = await context.Posts.FindAsync(id);
            if (postToRemove != null)
            {
                context.Posts.Remove(postToRemove);
                await context.SaveChangesAsync();
            }
        }

        //[Note] - tak, IAsyncEnumerable ma wewnątrz taski i yeld'a, więc samo użycie tego typu już oznacza async'a - Czy jeśli chcę używać IAsyncEnumerable, które ma taski wewnątrz, to znaczy, że samej metody już nie mogę oznaczyć jako async
        public IQueryable<BlogPost> /*Task<*//*IEnumerable*//*IQueryable<BlogPost>*/ GetAllBlogPostsAsync() //=> (IEnumerable<BlogPost>)context.Posts?.ToAsyncEnumerable();
        {
            //return /*await*/ context.Posts;//.ToList();
            return /*await*/ context.Posts.Include(p => p.Author)
                .Include(p => p.Comments)
                .Include(p => p.Votes)
                .Select(p => p);
                //.async();// LoadAsync();
        }

        public override async Task<BlogPost> GetAsync(long id)
        {
            //return await context.Posts.Include(p => p.Author).FindAsync(id);
            return await context.Posts.Where(p => p.Id == id)
                .Include(p => p.Author)
                .Include(p => p.Comments)
                .Include(p => p.Votes)
                .FirstOrDefaultAsync();
        }

        public override async Task<BlogPost> UpdateAsync(BlogPost objectToUpdate)
        {
            if (objectToUpdate == null)
            {
                throw new InvalidOperationException("Update failed - empty source object");
            }
            ThrowOnTitleDuplication(objectToUpdate.Title, objectToUpdate.Id);

            BlogPost post = await context.Posts.FindAsync(objectToUpdate.Id);
            if (post != null)
            {
                objectToUpdate.UpdateModifiedDate();
                context.Entry(post).CurrentValues.SetValues(objectToUpdate); //[Note] !! - To nie ogarnia zagnieżdżonych typów referencyjnych, tylko proste. Jeśli properties'y są referencjami, trzeba je zaktualizować indywidualnie (https://stackoverflow.com/questions/13236116/entity-framework-problems-updating-related-objects)
                await context.SaveChangesAsync(); //?? Było polecane użycie update, żeby nie zmieniać całego kontekstu, ale nie ma update'u asynchronicznego
                return post;//[Note] - jeśli było by ryzyko, że będzie jednoczesna aktualizacja na bazie, to powinno się odczytywać zawsze z bazy - Czy wystarczy, że zwrócę obiekt post, czy muszę go ponownie odczytywać z bazy, żeby wykluczyć jakiekolwiek niespójności (powinno raczej wystarczyć zwrócenie tego obiektu, bo context powinien być spójny z bazą)?
            }
            else
            {
                throw new BlogPostsDomainException("Update failed - no post to update");
            }
        }

        private void ThrowOnTitleDuplication(string titleToCheck, long sourcePostId)
        {
            bool isTitleDuplicate = context.Posts.FirstOrDefault(p => p.Title == titleToCheck && p.Id != sourcePostId) != null; //[Note] - at this point First, but it can be changed in future implementations of the framework - Czy FirstOrDefault jest wydajniejsze niż where? Która opcja będzie pchała najmniej zbędnych danych?
            if (isTitleDuplicate)
            {
                throw new BlogPostsDomainException("Update failed - the post with given title already exists");
            }
        }

        public long GetAllMarksOfGivenType(bool refersToPositive, long relatedItemId)
        {
            return (long)context.Votes.Count(v => v.VotedPost.Id == relatedItemId && v.IsLike == refersToPositive);
        }

        public async Task AddMarkAsync(Vote voteToAdd)
        {
            if (voteToAdd == null || voteToAdd.VotedPost == null)
            {
                throw new BlogPostsDomainException("Adding mark failed - empty object");
            }

            Vote alreadyExistingVoteEntry = context.Votes.Where(v => v.Voter.Id == voteToAdd.Voter.Id && v.VotedPost.Id == voteToAdd.VotedPost.Id) as Vote;
            if (alreadyExistingVoteEntry == null)
            {
                await context.Votes.AddAsync(voteToAdd);
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
            Vote voteToRemove = await context.Votes.FindAsync(id);
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

        public IQueryable<BlogPost> GetPostsContaingInTitle(string textToSearch)
        {
            IQueryable<BlogPost> temp = null;
            if(String.IsNullOrWhiteSpace(textToSearch))
            {
                temp = context.Posts; //[Note] - można uprościć wszystkie wywołania include dla zależności EF (sprawdzić podejście z drugim rodzajem ładowania - lazy loading, gdzie typy referncyjne w modelu muszą być zdefiniowane jako virtual)
            }
            else
            {
                temp = context.Posts.Where(p => p.Title.Contains(textToSearch, StringComparison.InvariantCultureIgnoreCase));
            }
            return temp
                .Include(p => p.Author)
                .Include(p => p.Comments)
                .Include(p => p.Votes);
        }

        public decimal GetTotalPostsCount()
        {
            return context.Posts.Count();
        }

        //public /*async*/ IQueryable<BlogPost> GetBlogPostsChunkAsync(int pageNo, int postsPerPage)
        public /*async*/ IQueryable<BlogPost> GetBlogPostsChunkAsync(int pageNo, int postsPerPage)
        {
            IQueryable<BlogPost> postsChunk = Enumerable.Empty<BlogPost>().AsQueryable();//null;
            long totalPostsCount = context.Posts.Count();
            if(totalPostsCount > 0)
            {
                int numberOfPostsToTake = postsPerPage > 0 ? postsPerPage : 1;
                int count = pageNo * numberOfPostsToTake;
                postsChunk = context.Posts.Include(p => p.Author)
                    .Include(p => p.Comments)
                    .Include(p => p.Votes)
                    .Skip(count).Take(numberOfPostsToTake);
            }
            return postsChunk;
        }
    }
}
