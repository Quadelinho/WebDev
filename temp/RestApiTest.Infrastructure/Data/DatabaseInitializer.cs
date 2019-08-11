using Microsoft.Extensions.Logging;
using RestApiTest.Core.Interfaces.Repositories;
using RestApiTest.Core.Models;
using RestApiTest.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RestApiTest.Infrastructure.Data
{
    public class DatabaseInitializer : IDbInitializer
    {
        IBlogPostRepository blogsRepository;
        IForumUserRepository usersRepository;
        ILogger<DatabaseInitializer> logger;
        //ForumContext context;

        //public DatabaseInitializer(/*IBlogPostRepository repository, IForumUserRepository usersRepository*/)
        //{
        //    this.blogsRepository = repository;
        //    this.usersRepository = usersRepository;
        //}

        public DatabaseInitializer(ILogger<DatabaseInitializer> logger)
        {
            this.logger = logger;
        }

        public async void PrepareSampleData(ForumContext context, bool skipIfDbNotEmpty)
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
                await context.Users.AddRangeAsync(usersToAdd);
                //await context.SaveChangesAsync();
                await context.Posts.AddRangeAsync(CreateSamplePosts(usersToAdd[0]));
                await context.SaveChangesAsync();
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

//?? Jak dokładnie działa ta inicjalizacja bazy użyta w przykładzie NIP, bo ten zapis jest dla mnie nie całkiem jasny? 
//- jak tam następuje odwołanie do tej klasy ConfigureWebHostHelper, skoro nigdzie nie widać tam jej jawnego wywołania, tylko odwołanie do jej metody?
// https://github.com/wi7a1ian/nip-lab-2018/blob/dev/add-v2-controller-tests/src/Nip.Blog/Services/Posts/Posts.API/Program.cs
//?? W przykładzie NIP jest informacja, żeby hasła trzymać w managerze haseł secrets.json - czym jest ten plik i jak tam można bezpiczenie trzymać hasła (czy tam jest tylko ścieżka do jakiegoś zewnętrznego narzędzia?)