using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Moq;
using RestApiTest.Controllers;
using RestApiTest.Core.Interfaces.Repositories;
using System;
using Xunit;
using System.Linq;
using RestApiTest.Core.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using RestApiTest.DTO;
using Microsoft.AspNetCore.Http;
using System.Collections.Specialized;

namespace RestApiTest.ControllersUnitTests
{
    public class UnitTest1
    {
        [Fact]
        public async Task TestGetAll()
        {
            //Arrange
            IQueryable<BlogPost> expectedPosts = ReturnFakePosts();
            var logger = new Mock<ILogger<BlogController>>();
            var repository = new Mock<IBlogPostRepository>();
            var hostingEnvironment = new Mock<IHostingEnvironment>();
            var autoMapper = new Mock<IMapper>();
            autoMapper.Setup(m => m.ProjectTo<BlogPostDTO>(It.IsAny<IQueryable<BlogPost>>(), It.IsAny<object>())) //[Note] - dla domyslnych: It.IsAny<object>(), dla params: It.IsAny<object[]>() - Czy jest jakis sposob na mock dla metod z domyslnymi parametrami? 
                .Returns(() => 
                {
                    var t = new List<BlogPostDTO>();
                    foreach (BlogPost post in expectedPosts)
                    {
                        t.Add(new BlogPostDTO() { Id = post.Id, Title = post.Title }); 
                    }
                    return t.AsQueryable();
                });
            var configuration = new Mock<IConfiguration>();
            var configurationSection = new Mock<IConfigurationSection>();
            configurationSection.Setup(s => s.Value).Returns("1");
            configuration.Setup(c => c.GetSection(It.IsAny<String>())).Returns(configurationSection.Object);

            repository.Setup(r => r.GetAllBlogPostsAsync()).Returns(expectedPosts);
            
            BlogController controller = new BlogController(logger.Object, repository.Object, hostingEnvironment.Object, autoMapper.Object, configuration.Object);

            //Act
            var posts = await controller.GetAll();

            //Assert
            var result = Assert.IsType<OkObjectResult>(posts.Result);
            //var returnedValue = Assert.IsType<IQueryable<BlogPostDTO>>(result.Value);
            var returnedVal = Assert.IsAssignableFrom<IQueryable<BlogPostDTO>>(result.Value);
            Assert.Equal(2, returnedVal.Count());
            autoMapper.Verify(m => m.ProjectTo<BlogPostDTO>(It.IsAny<IQueryable<BlogPost>>(), It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task TestGet()
        {
            //Arrange
            BlogPost expectedPostData = new BlogPost() { Id = 765, Title = "Fake value", Author = null, Content = "Fake content", Comments = null, Votes = null };
            var logger = new Mock<ILogger<BlogController>>();
            var repository = new Mock<IBlogPostRepository>();
            var hostingEnvironment = new Mock<IHostingEnvironment>();
            var autoMapper = new Mock<IMapper>(); //?? czy automapera sie mockuje, czy jest na to jakies sprytniejsze podejscie?
            autoMapper.Setup(m => m.Map<BlogPost, BlogPostDTO>(It.IsAny<BlogPost>())).Returns<BlogPost>( p => new BlogPostDTO() {Id = p.Id, Title = p.Title, Content = p.Content });
            var configuration = new Mock<IConfiguration>();
            var configurationSection = new Mock<IConfigurationSection>();
            configurationSection.Setup(s => s.Value).Returns("1");
            configuration.Setup(c => c.GetSection(It.IsAny<String>())).Returns(configurationSection.Object);

            repository.Setup(r => r.GetAsync(It.IsAny<long>())).ReturnsAsync(expectedPostData);

            BlogController controller = new BlogController(logger.Object, repository.Object, hostingEnvironment.Object, autoMapper.Object, configuration.Object);

            //Act
            var post = await controller.Get(765);

            //Assert
            var result = Assert.IsType<OkObjectResult>(post.Result);
            var returnedValue = Assert.IsType<BlogPostDTO>(result.Value);
            Assert.Equal(expectedPostData.Title, returnedValue.Title);
            Assert.Equal(expectedPostData.Id, returnedValue.Id);
            Assert.Equal(expectedPostData.Content, returnedValue.Content);
            autoMapper.Verify(m => m.Map<BlogPost, BlogPostDTO>(It.IsAny<BlogPost>()), Times.Once());
        }

        [Fact]
        public async Task ShouldReturnNotFoundForGetWithNotExistingID()
        {
            //Arrange
            var logger = new Mock<ILogger<BlogController>>();
            var repository = new Mock<IBlogPostRepository>();
            var hostingEnvironment = new Mock<IHostingEnvironment>();
            var autoMapper = new Mock<IMapper>();
            autoMapper.Setup(m => m.Map<BlogPost, BlogPostDTO>(It.IsAny<BlogPost>()));
            var configuration = new Mock<IConfiguration>();
            var configurationSection = new Mock<IConfigurationSection>();
            configurationSection.Setup(s => s.Value).Returns("1");
            configuration.Setup(c => c.GetSection(It.IsAny<String>())).Returns(configurationSection.Object);

            repository.Setup(r => r.GetAsync(It.IsAny<long>())).ReturnsAsync((BlogPost)null);

            BlogController controller = new BlogController(logger.Object, repository.Object, hostingEnvironment.Object, autoMapper.Object, configuration.Object);
            const long searchedId = 75;
            //Act
            var post = await controller.Get(searchedId);

            //Assert
            var res = Assert.IsType<NotFoundObjectResult>(post.Result);
            Assert.Equal(searchedId, res.Value);
            autoMapper.Verify(m => m.Map<BlogPost, BlogPostDTO>(It.IsAny<BlogPost>()), Times.Never());
        }

        [Fact]
        public async Task ShouldFindPostsByTitle()
        {
            //Arrange
            IQueryable<BlogPost> expectedPosts = ReturnFakePosts();
            var logger = new Mock<ILogger<BlogController>>();
            var repository = new Mock<IBlogPostRepository>();
            var hostingEnvironment = new Mock<IHostingEnvironment>();
            var autoMapper = new Mock<IMapper>();
            autoMapper.Setup(m => m.ProjectTo<BlogPostDTO>(It.IsAny<IQueryable<BlogPost>>(), null))
                .Returns(() =>
                {
                    var t = new List<BlogPostDTO>();
                    foreach (BlogPost post in expectedPosts)
                    {
                        t.Add(new BlogPostDTO() { Id = post.Id, Title = post.Title });
                    }
                    return t.AsQueryable();
                });
            var configuration = new Mock<IConfiguration>();
            var configurationSection = new Mock<IConfigurationSection>();
            configurationSection.Setup(s => s.Value).Returns("1");
            configuration.Setup(c => c.GetSection(It.IsAny<String>())).Returns(configurationSection.Object);

            var queryString = new NameValueCollection { { "postsPerPage", "1" } };
            var mockRequest = new Mock<HttpRequest>();
            //mockRequest.Setup(r => r.Body.Read); //?? mock'owanie requestow na potrzeby metod wywolujacych z uzyciem FromQuery - tak, trzeba zamockowac caly context: https://stackoverflow.com/questions/22311805/how-to-set-the-value-of-a-query-string-in-test-method-moq
            mockRequest.Setup(r => r.QueryString).Returns(() =>
            {
                var str = new QueryString();
                str.Add("postsPerPage", "1");
                return str;
            });

            decimal outPostsNumber = 0;
            repository.Setup(r => r.GetPostsContaingInTitle(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), out outPostsNumber))
                .Returns(expectedPosts);

            BlogController controller = new BlogController(logger.Object, repository.Object, hostingEnvironment.Object, autoMapper.Object, configuration.Object);

            //Act
            var posts = await controller.GetPosts("fake", 0, 1);

            //Assert
            var result = Assert.IsType<OkObjectResult>(posts.Result);
            //var returnedValue = Assert.IsType<IQueryable<BlogPostDTO>>(result.Value);
            var returnedVal = Assert.IsAssignableFrom<IQueryable<BlogPostDTO>>(result.Value);
            Assert.Equal(2, returnedVal.Count());
            autoMapper.Verify(m => m.ProjectTo<BlogPostDTO>(It.IsAny<IQueryable<BlogPost>>(), null), Times.Once);
        }

        private IQueryable<BlogPost> ReturnFakePosts()
        {
            List<BlogPost> posts = new List<BlogPost>();
            posts.Add(new Mock<BlogPost>().Object);
            posts.Add(new Mock<BlogPost>().Object);
            return posts.AsQueryable();
        }
    }
}
