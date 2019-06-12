using RestApiTest.Core.Exceptions;
using RestApiTest.Core.Interfaces;
using RestApiTest.Core.Interfaces.Repositories;
using RestApiTest.Core.Models;
using RestApiTest.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiTest.Core.Repositories
{ //[Note] - nie nie, musi. - Czy każda klasa modelu musi / powinna mieć swoje pokrycie w klasie Repository,czy np. operacje na komentarzach mogą być z poziomu BlogPostRepository (bo komentarze nie mogą przecież być procesowane samodzielnie, w oderwaniu od postów)
    public class BlogPostRepository : IBlogPostRepository
    {
        private ForumContext context;
        public BlogPostRepository(ForumContext context)
        {
            this.context = context;
        }

        public async Task AddAsync(BlogPost objectToAdd) //[Note] - nie, bo to w założeniach nie są na tyle długotrwałe operacje - czy w praktyce w tego typu operacjach stosuje się cancellation token'y, czy raczej tylko w przypadku jakichś bardzo dużych obiektów  (np. z całego formularza)
        {
            if(objectToAdd == null)
            {
                throw new ArgumentNullException("Failed to insert the post - empty entry, blogPost"); //[Note] - skoro to metoda publiczna, to lepiej robić walidację danych wejściowych też na tym etapie, nawet jeśli miałyby być zwielokrotnione te walidacje - czy takie podejście się stosuje, czy raczej się zwraca po prostu "pusty resultat"?
                                                                                                    //A może za kontrolę danych wejściowych powinien już odpowiadać kotroler, zanim zaangażuje wewnętrzne klasy?
            }

            bool isTitleDuplicate = context.Posts.FirstOrDefault(p => p.Title == objectToAdd.Title) != null;
            if (isTitleDuplicate)
            {
                throw new BlogPostsDomainException("Update failed - the post with given title already exists");
            }

            await context.Posts.AddAsync(objectToAdd); //?? Czy w web'ówce stosuje się w tym punkcie kontrolę czy wpis już istnieje, czy to po prostu powinien złapać global handler?
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            BlogPost postToRemove = await context.Posts.FindAsync(id);
            if(postToRemove != null)
            {
                context.Posts.Remove(postToRemove);
                await context.SaveChangesAsync();
            }
        }

        public async Task</*IAsyncEnumerable*/IEnumerable<BlogPost>> GetAllBlogPostsAsync() => (IEnumerable<BlogPost>)context.Posts?.ToAsyncEnumerable();

        public async Task<BlogPost> GetAsync(long id)
        {
            return await context.Posts.FindAsync(id);
        }

        public async Task UpdateAsync(BlogPost objectToUpdate)
        {
            if (objectToUpdate == null)
            {
                throw new InvalidOperationException("Update failed - empty source object");
            }
            //TODO: drugie query z Where do porównania
            bool isTitleDuplicate = context.Posts.FirstOrDefault(p => p.Title == objectToUpdate.Title && p.Id != objectToUpdate.Id) != null; //[Note] - at this point First, but it can be changed in future implementations of the framework - Czy FirstOrDefault jest wydajniejsze niż where? Która opcja będzie pchała najmniej zbędnych danych?
            if (isTitleDuplicate)
            {
                throw new BlogPostsDomainException("Update failed - the post with given title already exists");
            }

            BlogPost post = await context.Posts.FindAsync(objectToUpdate.Id);
            if (post != null)
            {
                context.Entry(post).CurrentValues.SetValues(objectToUpdate); //[Note] !! - To nie ogarnia zagnieżdżonych typów referencyjnych, tylko proste. Jeśli properties'y są referencjami, trzeba je zaktualizować indywidualnie (https://stackoverflow.com/questions/13236116/entity-framework-problems-updating-related-objects)
                await context.SaveChangesAsync(); //?? Było polecane użycie update, żeby nie zmieniać całego kontekstu, ale nie ma update'u asynchronicznego
            }
            else
            {
                throw new BlogPostsDomainException("Update failed - no post to update");
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
            if(voteToRemove != null)
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
//TODO: repo vote'ów powinno usuwać zależności wcześniej, a całość ma yć wyowłana przez service
            }
        }
    }
}
