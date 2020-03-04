using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RestApiTest.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestApiTest.Core.Interfaces.Repositories;
using RestApiTest.Core.Models;
using RestApiTest.DTO;
using RestApiTest.Infrastructure.Data;
using RestApiTest.Infrastructure.Repositories;
using Swashbuckle.AspNetCore.Swagger;

namespace RestApiTest.Infrastructure.IntegrationTests
{
    public class CustomWebAppFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services => 
            {
                //var serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase();
                //DbContextOptionsBuilder<ForumContext> optB = new DbContextOptionsBuilder<ForumContext>();
                
                services.AddDbContext<ForumContext>(options => options.UseInMemoryDatabase("BlogPost_test"));
            });
            //base.ConfigureWebHost(builder);
        }
    }
}
