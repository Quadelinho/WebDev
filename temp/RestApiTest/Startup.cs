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
        private readonly ILogger<ForumContext> logger;

        //?? Coś tu nie gra z docker'em - bez zmian czasem działa, a innym razem w ogóle nie reaguje na ten projekt (choć hello-world odpowiada i nie zgłasza nic żadneego błędu)

        //?? gdzie definiuje się mapowanie tego automatycznego Dependency Injection, np. kiedy mam kilka implementacji danego interfejsu? Jawnie w ConfigureServices, a domyślne mapowanie można jakoś podejrzeć?
        public Startup(/*IConfiguration configuration,*/ ILogger<ForumContext> log, IHostingEnvironment environment) //[Note] Jest tu już wstrzykiwany obiekt logger'a - Atomatyczne dependency injection jest w stanie to ogarnąć
        {
            //Configuration = configuration;
            var configBuilder = new ConfigurationBuilder();
            configBuilder.SetBasePath(environment.ContentRootPath);
            configBuilder.AddJsonFile("appsettings.json", false, true); //?? Muszę tu jeszcze wtedy jawnie podać też ten plik appsettings.Development.json, tak? Czy on jest brany z automatu kiedy jest środowisko debug? Jeśli tak, to jak to jest definiowane?
            configBuilder.AddJsonFile("connectionSettings.json", false, true); //?? Czy jeśli zdecyduję się mieć kilka plików konfiguracyjnych, to muszę zawsze ten domyślny jawnie podawać, czy mogę jakoś tylko dodać dodatkowe?
            Configuration = configBuilder.Build();

            logger = log;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            //services.AddDbContext<BlogDBContext>(opt => opt.UseInMemoryDatabase("BlogPostDB_01"));
            //services.AddDbContext<BlogDBContext>(opt => opt.UseSqlite("Data Source=Data/BlogPostDB_01.db"));

            //TODO: [Done] config file Json i dodać tam connection string (wstrzyknąć IConfig).

            //var connectionString = @"Data Source=plktw4624n\SQLEXPRESS;Database=BlogPostDB_01;Trusted_Connection=true";
            var connectionString = Configuration.GetValue<string>("connectionString");
           // services.AddDbContext<ForumContext>(opt => opt.UseSqlServer(connectionString)); //TODO: [Done] użyć puli połączeń
            services.AddDbContextPool<ForumContext>(opt => opt.UseSqlServer(connectionString)); //?? Czy jeszcze trzeba na tym etapie dodać jakieś szczególne zabezpieczenia przed współbieżnością,
            //                                                                                          albo jakieś zarządzanie tą pulą, czy to co już jest z automatu i już zrobione jako async w kontrolerze wystarczy?

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
