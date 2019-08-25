using Microsoft.Extensions.Logging;
using RestApiTest.Core.Interfaces.Repositories;
using RestApiTest.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace RestApiTest.Infrastructure.Data
{
    public class DatabaseInitializer : IDbInitializer
    {
        IBlogPostRepository blogsRepository;
        IForumUserRepository usersRepository;
        ILogger<DatabaseInitializer> logger;
        
        public DatabaseInitializer(ILogger<DatabaseInitializer> logger)
        {
            this.logger = logger;
        }

        public void PrepareSampleData(ForumContext context, bool skipIfDbNotEmpty)
        { //[Note] - trzeba to dobrze zarejestrować w startup'ie - patrz NIP - Jak zapewnić tutaj możliwość użycia interfejsów repo zamiast rzeczywistych typów, żeby zapewnić łatwe testowanie? Użycie tego automatycznego dependency injection tak jak w controllerach mi nie zadziałało
            
            if(context == null)
            {
                logger.LogDebug("Empty context - no sample data created");
                return;
            }

            var userEntry = context.Users.FirstOrDefault();
            if(userEntry == null || !skipIfDbNotEmpty)
            {
                List<ForumUser> usersToAdd = CreateSampleUsers();
                context.Users.AddRange(usersToAdd);
                context.SaveChanges();
                context.Posts.AddRange(CreateSamplePosts(usersToAdd[0]));
                context.SaveChanges();
            }
        }

        private List<ForumUser> CreateSampleUsers()
        {
            List<ForumUser> newUsers = new List<ForumUser>()
            {
                new ForumUser(){Email = "_sample@mail_.com", Login = "sampler", Name = "Sammy", ReputationPoints = 0 },
                new ForumUser(){Email = "john.doe@mail.com", Login = "jdoe", Name = "John Doe", ReputationPoints = 1 },
                new ForumUser(){Email = "jack.tester@mail.com", Login = "jack", Name = "Jack Tester", ReputationPoints = 1 }
            };
            foreach (ForumUser user in newUsers)
            {
                user.SetInitialValues();
                user.Confirm();
            }

            return newUsers;
        }

        private List<BlogPost> CreateSamplePosts(ForumUser sampleAuthor)
        {
            
            List<BlogPost> newPosts = new List<BlogPost>()
            {
                new BlogPost(){Author = sampleAuthor, Content = "This is a sample content", Title = "Sample title 1"},
                new BlogPost(){Author = sampleAuthor, Content = "Yet another sample", Title = "Test title"},
                new BlogPost(){Author = sampleAuthor, Content = "One more sample to ignore", Title = "Is this sample again?"}
            };
            List<Comment> sampleComments = new List<Comment>()
            {
                new Comment(){Author = sampleAuthor, Content = "Does anyone like my post?", RelatedPost = newPosts[0], Title = "Question" },
                new Comment(){Author = sampleAuthor, Content = "Will there be anything else or just samples?", RelatedPost = newPosts[0], Title = "I want more content!" }
            };
            return newPosts;
        }
    }
}

//[To initializeDatabase jest extension'em, dlatego jest wywoływana bezpośrednio z obiektu IWebHost] Jak dokładnie działa ta inicjalizacja bazy użyta w przykładzie NIP, bo ten zapis jest dla mnie nie całkiem jasny? 
//- jak tam następuje odwołanie do tej klasy ConfigureWebHostHelper, skoro nigdzie nie widać tam jej jawnego wywołania, tylko odwołanie do jej metody?
// https://github.com/wi7a1ian/nip-lab-2018/blob/dev/add-v2-controller-tests/src/Nip.Blog/Services/Posts/Posts.API/Program.cs
//[Note] - Secrets.json jest plikiem w systemie przechowującym hasła. Tylko administrator danej organizacji może tam edytować dane połączeniowe. Dostęp do pliku z poziomu projektu może być przez kliknięcie PPM na projekcie (np. RestApiTest) i wybranie opcji 'Manage User Secrets' - -  W przykładzie NIP jest informacja, żeby hasła trzymać w managerze haseł secrets.json - czym jest ten plik i jak tam można bezpiczenie trzymać hasła (czy tam jest tylko ścieżka do jakiegoś zewnętrznego narzędzia?)