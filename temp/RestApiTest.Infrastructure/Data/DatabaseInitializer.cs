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

        //public DatabaseInitializer(/*IBlogPostRepository repository, IForumUserRepository usersRepository*/)
        //{
        //    this.blogsRepository = repository;
        //    this.usersRepository = usersRepository;
        //}

        public async void PrepareSampleData()
        { //?? Jak zapewnić tutaj możliwość użycia interfejsów repo zamiast rzeczywistych typów, żeby zapewnić łatwe testowanie? Użycie tego automatycznego dependency injection tak jak w controllerach mi nie zadziałało
            //IForumUserRepository usersRepository = new ForumUserRepository(); //?? Jawne używanie rzeczywistych typów tutaj wymaga przekazywania wszystkich zależności, co raczej nie jest dobrym podejściem. Jak to zrobić inaczej?
            //var users = await usersRepository.GetAllUsersAsync();//GetAllBlogPostsAsync();
            //if (users == null || users.Count() == 0)
            //{
            //    CreateSampleUsers();
            //    CreateSamplePosts();

            //    //var sampleData = CreateSampleData(5);
            //    //foreach (var post in sampleData)
            //    //{
            //    //    repository.AddAsync(post);
            //    //}
            //}
        }

        private void CreateSampleUsers()
        {

        }

        private void CreateSamplePosts()
        {
            //List<QuestionPost> posts = new List<QuestionPost>();
            //for (int postIndex = 1; postIndex <= requiredNumberOfSamples; ++postIndex)
            //{
            //    posts.Add(new QuestionPost()
            //    {
            //        //Author = "User" + postIndex,
            //        Content = DateTime.Now.ToShortDateString(),
            //        //                   Modified = DateTime.Now.ToLongDateString(),
            //        Title = "Entry #" + postIndex
            //    });
            //}
            //return posts;
        }
    }
}
