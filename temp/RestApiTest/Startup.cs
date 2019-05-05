using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestApiTest.Data;
using Swashbuckle.AspNetCore.Swagger;

namespace RestApiTest
{
    public class Startup
    {
        private readonly ILogger<BlogDBContext> logger;

        //?? Coś tu nie gra z docker'em - bez zmian czasem działa, a innym razem w ogóle nie reaguje na ten projekt (choć hello-world odpowiada i nie zgłasza nic żadneego błędu)

        //?? gdzie definiuje się mapowanie tego automatycznego Dependency Injection, np. kiedy mam kilka implementacji danego interfejsu?
        public Startup(IConfiguration configuration, ILogger<BlogDBContext> log) //[Note] Jest tu już wstrzykiwany obiekt logger'a - Atomatyczne dependency injection jest w stanie to ogarnąć
        {
            Configuration = configuration;
            logger = log;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            //services.AddDbContext<BlogDBContext>(opt => opt.UseInMemoryDatabase("BlogPostDB_01"));
            services.AddDbContext<BlogDBContext>(opt => opt.UseSqlite("Data Source=Data/BlogPostDB_01.db"));

            //Register swagger service
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "BlogPost",
                    Version = "v1",
                    Description = "Swagger description for blog post",
                    Contact = new Contact { Name = "Test User", Email = "test@niepodam.pl" }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseExceptionHandler("/api/Error");
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseExceptionHandler("/api/Error");
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseMvc();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(); //?? Jakiego rodzaju akcje stosuje się w praktyce jako parametr?
                              
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "BlogPost API V1");
                c.RoutePrefix = string.Empty; // serve the Swagger UI at the app's root //[Note] Dodanie tego wpisu powoduje, że zawsze jeśli zostanie podany "pusty" url (np. tylko localhost + port), bez konkretnego wywołania - z automatu zostanie wyświetlona strona z dokumentacją API
            });
        }
    }
}

//DONE: dodać logowanie w endpointach ??Serilog
