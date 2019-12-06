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
using System.IO;
using Newtonsoft.Json;
using RestApiTest.Core.Exceptions;

namespace RestApiTest.ControllersUnitTests
{
    public class UnitTest1
    {
        private MapperConfiguration mapperConfig;
        private IMapper mapper;

        public UnitTest1()
        {
            mapperConfig = new MapperConfiguration(c => c.AddProfile(new MappingProfile()));
            mapper = mapperConfig.CreateMapper();
        }

        [Fact]
        public async Task TestGetAll()
        {
            //Arrange
            IQueryable<BlogPost> expectedPosts = ReturnFakePosts();
            var logger = new Mock<ILogger<BlogController>>();
            var repository = new Mock<IBlogPostRepository>();
            var hostingEnvironment = new Mock<IHostingEnvironment>();
            var configuration = new Mock<IConfiguration>();
            var configurationSection = new Mock<IConfigurationSection>();
            configurationSection.Setup(s => s.Value).Returns("1");
            configuration.Setup(c => c.GetSection(It.IsAny<String>())).Returns(configurationSection.Object);

            repository.Setup(r => r.GetAllBlogPostsAsync()).Returns(expectedPosts);
            //            var tst = mapper.Map<BlogPostDTO>(expectedPosts.First()); //zwraca prawidlowo mapowanie, ale ProjectTo zwraca puste wyniki z null'em w source'ie, chociaz Author, Comments i Votes zostaly uzupelnione
            //            var test = mapper.ProjectTo<BlogPostDTO>(expectedPosts); //[Note] - ProjectTo jest wewnetrznie optymalizowany pod katem budowy Linq i zapytan do bazy, zeby nie
                                                                                            //zaciagac zbednych kolumn (Map powoduje zaciaganie przez Linq wszystkich wlasciwosci, nawet jesli nie sa uzywane przez DTO.
                                                                                            //ProjectTo zawsze spodziewa sie miec dostep do jakiegos polaczenia z baza, nawet jesli jest wywolywane z poziomu testu i nie pobiera nic z bazy -
                                                                                            //musi byc albo zwykla baza, albo in memory (ale in memory nie jest najlepszym rozwiazaniem, bo nie sprawdza constrain'ow ani wiezow referencyjnych
                                                                                            //Dodatkowo, moze sie zdarzyc, ze na bazie in memory testy przejda, a produkcja na rzeczywistej bazie nie bedzie dzialac
                                                                                            //Czy tak uzyty AutoMapper prubuje uderzac do bazy lub czegos innego definiowanego w services, zeby wyrzucal null source?

            BlogController controller = new BlogController(logger.Object, repository.Object, hostingEnvironment.Object, mapper/*autoMapper.Object*/, configuration.Object);

            //Act
            var posts = await controller.GetAll();

            //Assert
            var result = Assert.IsType<OkObjectResult>(posts.Result);
            //var returnedValue = Assert.IsType<IQueryable<BlogPostDTO>>(result.Value);
            var returnedVal = Assert.IsAssignableFrom<IQueryable<BlogPostDTO>>(result.Value);
            Assert.Equal(2, returnedVal.Count());
            //autoMapper.Verify(m => m.ProjectTo<BlogPostDTO>(It.IsAny<IQueryable<BlogPost>>(), It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task TestGet()
        {
            //Arrange
            BlogPost expectedPostData = new BlogPost() { Id = 765, Title = "Fake value", Author = null, Content = "Fake content", Comments = null, Votes = null };
            var logger = new Mock<ILogger<BlogController>>();
            var repository = new Mock<IBlogPostRepository>();
            var hostingEnvironment = new Mock<IHostingEnvironment>();
            var configuration = new Mock<IConfiguration>();
            var configurationSection = new Mock<IConfigurationSection>();
            configurationSection.Setup(s => s.Value).Returns("1");
            configuration.Setup(c => c.GetSection(It.IsAny<String>())).Returns(configurationSection.Object);

            repository.Setup(r => r.GetAsync(It.IsAny<long>())).ReturnsAsync(expectedPostData);

            BlogController controller = new BlogController(logger.Object, repository.Object, hostingEnvironment.Object, mapper/*autoMapper.Object*/, configuration.Object);

            //Act
            var post = await controller.Get(765);

            //Assert
            var result = Assert.IsType<OkObjectResult>(post.Result);
            var returnedValue = Assert.IsType<BlogPostDTO>(result.Value);
            Assert.Equal(expectedPostData.Title, returnedValue.Title);
            Assert.Equal(expectedPostData.Id, returnedValue.Id);
            Assert.Equal(expectedPostData.Content, returnedValue.Content);
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
            
            var configuration = new Mock<IConfiguration>();
            var configurationSection = new Mock<IConfigurationSection>();
            configurationSection.Setup(s => s.Value).Returns("1");
            configuration.Setup(c => c.GetSection(It.IsAny<String>())).Returns(configurationSection.Object);

            var queryString = new NameValueCollection { { "postsPerPage", "1" } };
            var mockRequest = new Mock</*System.Web.*/HttpRequest>();
//            var queryS = new QueryString();
//            queryS.Add("postsPerPage", "1");
//            var mockRequest = CreateMockRequest(queryS);
            //mockRequest.Setup(r => r.Body.Read); //?? mock'owanie requestow na potrzeby metod wywolujacych z uzyciem FromQuery - tak, trzeba zamockowac caly context: https://stackoverflow.com/questions/22311805/how-to-set-the-value-of-a-query-string-in-test-method-moq
            mockRequest.Setup(r => r.QueryString).Returns(() =>
            {
                var str = new QueryString();
                str.Add("postsPerPage", "1");
                return str;
            });
            mockRequest.Setup(r => r.Query).Returns(() => 
            {
                //return (IQueryCollection)new List<object>();
                return (IQueryCollection)new List<KeyValuePair<string, string>>() { { new KeyValuePair<string, string>("postsPerPage", "44") } };
            });
            var mockHttpContext = new Mock<HttpContext>();//?? jak przekazac ten context do controller'a? Kiedy i gdzie jest budowany faktyczny obiekt httpcontext?
            //mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);
            mockHttpContext.Setup(c => c.Request).Returns(() =>
            {
                return mockRequest.Object;
            });
            //var mockControllerBase = new Mock<ControllerBase>();
            //mockControllerBase.Setup(r => r.Request).Returns(mockRequest.Object);

            decimal outPostsNumber = 0;
            repository.Setup(r => r.GetPostsContaingInTitle(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), out outPostsNumber))
                .Returns(expectedPosts);

            BlogController controller = new BlogController(logger.Object, repository.Object, hostingEnvironment.Object, mapper, configuration.Object);
            //controller.Request = mockRequest.Object;

            //Act
            var posts = await controller.GetPosts("fake", 0, 1);

            //Assert
            var result = Assert.IsType<OkObjectResult>(posts.Result);
            //var returnedValue = Assert.IsType<IQueryable<BlogPostDTO>>(result.Value);
            var returnedVal = Assert.IsAssignableFrom<IQueryable<BlogPostDTO>>(result.Value);
            Assert.Equal(2, returnedVal.Count());
            //autoMapper.Verify(m => m.ProjectTo<BlogPostDTO>(It.IsAny<IQueryable<BlogPost>>(), null), Times.Once);
        }

        [Fact]
        public async Task ShouldAddPost()
        {
            //Arrange
            BlogPost expectedPostData = new BlogPost() { Id = 765, Title = "Fake value", Author = null, Content = "Fake content", Comments = null, Votes = null };
            var logger = new Mock<ILogger<BlogController>>();
            var repository = new Mock<IBlogPostRepository>();
            var hostingEnvironment = new Mock<IHostingEnvironment>();
            var configuration = new Mock<IConfiguration>();
            var configurationSection = new Mock<IConfigurationSection>();
            configurationSection.Setup(s => s.Value).Returns("1");
            configuration.Setup(c => c.GetSection(It.IsAny<String>())).Returns(configurationSection.Object);

            repository.Setup(r => r.AddAsync(It.IsAny<BlogPost>(), null)).ReturnsAsync(expectedPostData);

            BlogController controller = new BlogController(logger.Object, repository.Object, hostingEnvironment.Object, mapper/*autoMapper.Object*/, configuration.Object);

            //Act
            var post = await controller.Post(mapper.Map<BlogPostDTO>(expectedPostData));

            //Assert
            var result = Assert.IsType<CreatedAtRouteResult>(post);
            var returnedValue = Assert.IsType<BlogPostDTO>(result.Value);
            Assert.Equal(expectedPostData.Title, returnedValue.Title);
            Assert.Equal(expectedPostData.Id, returnedValue.Id);
            Assert.Equal(expectedPostData.Content, returnedValue.Content);
        }

        [Fact]
        public async Task ShouldThrowExceptionWhileAddingEmptyPost()
        {
            //Arrange
            var logger = new Mock<ILogger<BlogController>>();
            var repository = new Infrastructure.Repositories.BlogPostRepository(null);
            var hostingEnvironment = new Mock<IHostingEnvironment>();
            var configuration = new Mock<IConfiguration>();
            var configurationSection = new Mock<IConfigurationSection>();
            configurationSection.Setup(s => s.Value).Returns("1");
            configuration.Setup(c => c.GetSection(It.IsAny<String>())).Returns(configurationSection.Object);
            
            BlogController controller = new BlogController(logger.Object, repository, hostingEnvironment.Object, mapper, configuration.Object);

            //Act & Assert
            await Assert.ThrowsAsync<BlogPostsDomainException>(async () => { await controller.Post(mapper.Map<BlogPostDTO>(null)); });
        }

        private IQueryable<BlogPost> ReturnFakePosts()
        {
            List<BlogPost> posts = new List<BlogPost>();
            var mockAuthor = new Mock<ForumUser>().Object;
            var mockComments = new List<Comment>() { new Mock<Comment>().Object };
            var mockVotes = new List<Vote>() { new Mock<Vote>().Object };
            posts.Add(new BlogPost() { Id = 1, Title = "First fake title", Content = "First fake content", Author = mockAuthor, Comments = mockComments, Votes = mockVotes}/*new Mock<BlogPost>().Object*/);
            posts.Add(new BlogPost() { Id = 2 , Title = "Second fake title", Content = "Second fake content", Author = mockAuthor, Comments = mockComments, Votes = mockVotes}/*new Mock<BlogPost>().Object*/);
            return posts.AsQueryable();
        }

        [Fact]
        public async Task ShouldApplyPatch()
        {
            //Arrange
            BlogPost expectedPostData = new BlogPost() { Id = 765, Title = "Fake value", Author = null, Content = "Fake content", Comments = null, Votes = null };
            BlogPost modifiedPostData = new BlogPost() { Id = 765, Title = "Updated fake", Author = null, Content = "Fake content", Comments = null, Votes = null };
            var logger = new Mock<ILogger<BlogController>>();
            var repository = new Mock<IBlogPostRepository>();
            var hostingEnvironment = new Mock<IHostingEnvironment>();
            var configuration = new Mock<IConfiguration>();
            var configurationSection = new Mock<IConfigurationSection>();
            configurationSection.Setup(s => s.Value).Returns("1");
            configuration.Setup(c => c.GetSection(It.IsAny<String>())).Returns(configurationSection.Object);

            repository.Setup(r => r.GetAsync(It.IsAny<long>())).ReturnsAsync(expectedPostData);
            repository.Setup(r => r.ApplyPatchAsync(It.IsAny<BlogPost>(), It.IsAny<List<Core.DTO.PatchDTO>>())).ReturnsAsync(modifiedPostData);

            BlogController controller = new BlogController(logger.Object, repository.Object, hostingEnvironment.Object, mapper/*autoMapper.Object*/, configuration.Object);

            //Act
            var post = await controller.Patch(765, new List<Core.DTO.PatchDTO>() { new Core.DTO.PatchDTO() { PropertyName = "Title", PropertyValue = "Updated fake"} });

            //Assert
            var result = Assert.IsType<OkObjectResult>(post);
            var returnedValue = Assert.IsType<BlogPostDTO>(result.Value);
            Assert.Equal(modifiedPostData.Title, returnedValue.Title);
            Assert.Equal(expectedPostData.Id, returnedValue.Id);
            Assert.Equal(expectedPostData.Content, returnedValue.Content);
            //repository.Verify(r => r.GetAsync(It.IsAny<long>()));
            //repository.Verify(r => r.ApplyPatchAsync(It.IsAny<BlogPost>(), It.IsAny<List<Core.DTO.PatchDTO>>()));
            repository.VerifyAll();
        }

        private Mock<HttpRequest> CreateMockRequest(object body)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);

            var json = JsonConvert.SerializeObject(body);

            sw.Write(json);
            sw.Flush();

            ms.Position = 0;

            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(x => x.Body).Returns(ms);

            return mockRequest;
        }
    }
}
