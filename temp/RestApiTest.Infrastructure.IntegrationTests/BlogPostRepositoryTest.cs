using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestApiTest.Core.Interfaces.Repositories;
using RestApiTest.Core.Models;
using RestApiTest.Infrastructure.Data;
using RestApiTest.Infrastructure.Repositories;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace RestApiTest.Infrastructure.IntegrationTests
{
    public class BlogPostRepositoryTest : IClassFixture<WebApplicationFactory<Startup>> //?? Czy tu muszę zawsze w takim razie używać referencji do projektu głównego aplikacji, żeby mieć dostęp do Startup'u? Czy connection string'a używa się dla testów inaczej niż przez WebHostBuilder'a?
    {
        private readonly WebApplicationFactory<Startup> factory;
        private ForumContext context;

        public BlogPostRepositoryTest(WebApplicationFactory<Startup> factory)
        {
 //           var projectDir = Directory.GetCurrentDirectory();
 //           var configPath = Path.Combine(projectDir, "connectionSettings.json");
            this.factory = factory.WithWebHostBuilder(builder =>
            {
 //               builder.ConfigureAppConfiguration((context, conf) =>
//                {
//                    conf.AddJsonFile(configPath);
//                });
                builder.ConfigureServices(services => 
                {
                    services.AddDbContext<ForumContext>(opt => opt.UseInMemoryDatabase("BlogPost_test1"));
                });
                //TODO: Sprawdzić podejście z klasą Fixture, żeby podać w Services.AddDbContextPool.UseInMemory: https://www.codingame.com/playgrounds/35462/creating-web-api-in-asp-net-core-2-0/part-3---integration-tests
 //               using (var scope = builder.Build().Services.CreateScope())
 //               {
                    //[NIe używa się w testach integracyjnych] Czy da się wywołać context z DependencyInjection na poziomie tych testów?
//                    context = scope.ServiceProvider.GetRequiredService<ForumContext>();
//                    context = scope.ServiceProvider.GetService<ForumContext>();
                    //context = builder.Build().Services.GetService<ForumContext>();
 //               }
            });
        }

        [Fact]
        public async Task Test1()
        {
            // Arrange
            var request = "https://localhost:5001/api/posts/";
  //          HttpClient client = factory.CreateClient();//new HttpClient();
 //           ForumContext ctx = new ForumContext((new DbContextOptionsBuilder().UseInMemoryDatabase().Options));
            //ForumContext ctx1;
            //ForumContext ctx2; //?? Jak tutaj dostać ForumContext używający tej bazy inMemory?
            //factory.WithWebHostBuilder(services =>
            //{
            //    ctx1 = services.Build().Services.GetService<ForumContext>();
            //    ctx2 = services.Build().Services.GetRequiredService<ForumContext>();
            //});

            // Act
            //var response = await client.GetAsync(request);

 //           using (var serviceScope = app.ApplicationServices.CreateScope())
 //           {
                
                //[Note] - jeśli są problemy z już użytym contextem, można w connection stringu podać flagę dopuszczającą wiele połączeń: MultipleActiveResultSets=true
 //               var context = serviceScope.ServiceProvider.GetService<ForumContext>();
                //context.Database.EnsureCreated(); //[Note] jeśli baza zostanie stworzona przez EnsureCreated - nie używane są do tego migracje i nie da się już na takiej bazie użyć migracji. EnsureCreated jest zazwyczaj używane tylko jeśli potrzeba jakiejś szybko tworzonej bazy na potrzeby testów (https://stackoverflow.com/questions/38238043/how-and-where-to-call-database-ensurecreated-and-database-migrate)
                //               IDbInitializer dbInitializer = new DatabaseInitializer(loggerFactory.CreateLogger<DatabaseInitializer>()/*app.ApplicationServices.GetService<ILogger<DatabaseInitializer>*/);
                //               dbInitializer.PrepareSampleData(serviceScope.ServiceProvider.GetService<ForumContext>(), true);
//                IBlogPostRepository blogPostRepo = new BlogPostRepository(ctx);
            BlogPost postToAdd = new BlogPost();
            postToAdd.Content = "Content from test";
            postToAdd.Title = "Title from test";
  //          blogPostRepo.AddAsync(postToAdd);
 //           }
            

            // Assert
            //response.EnsureSuccessStatusCode();
        }
    }
}
