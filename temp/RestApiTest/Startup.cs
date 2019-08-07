using AutoMapper;
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

namespace RestApiTest
{
    public class Startup
    {
        private readonly ILogger<ForumContext> logger;
        private IMapper mapper;
        
        //[Note] - tutaj, w ConfigureServices - gdzie definiuje się mapowanie tego automatycznego Dependency Injection, np. kiedy mam kilka implementacji danego interfejsu? Jawnie w ConfigureServices, a domyślne mapowanie można jakoś podejrzeć?
        public Startup(/*IConfiguration configuration,*/ ILogger<ForumContext> log, IHostingEnvironment environment/*, IDbInitializer dbInitializer*/) //[Note] Jest tu już wstrzykiwany obiekt logger'a - Atomatyczne dependency injection jest w stanie to ogarnąć
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
            //[Note] - te kolejne metody umożliwiające rejestrowanie kolejnych usług są uzyskiwane na zasadzie mechanizmu zwanego "fluent interfaces"
                //Każdy pakiet dodaje swoje extensiony do namespace'a frameworkowego (np. System.Collections) i wtedy każda klasa, która w using'u poda ten pakiet widzi wszystkie to rozszerzenia
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddTransient<IBlogPostRepository, BlogPostRepository>();  //[Note] - może przez dziedziczenie po wspólnych interface'ach, bo normalnie powinno to działać bez jawnej deklaracji - Dlaczego muszę wszystkie te repozytoria rejestrować jawnie dla DI?
            services.AddTransient<ICommentRepository, CommentRepository>();
            services.AddTransient<IForumUserRepository, ForumUserRepository>();
            services.AddTransient<IDbInitializer, DatabaseInitializer>();
            ConfigureAutoMapper();
            services.AddSingleton<IMapper>(mapper);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            //services.AddDbContext<BlogDBContext>(opt => opt.UseInMemoryDatabase("BlogPostDB_01"));
            //services.AddDbContext<BlogDBContext>(opt => opt.UseSqlite("Data Source=Data/BlogPostDB_01.db"));

            //[Done] config file Json i dodać tam connection string (wstrzyknąć IConfig).

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
            //services.AddAutoMapper(); //[Note] - Trzeba zainstalować pakiet nuget'owy AutoMapper.Extensions.Microsoft.DependencyInjection W necie była informacja, żeby zarejestrować AutoMappera tutaj taką metodą. Czy teraz już nie jest to potrzebne, czy jeszcze czegoś mi brakuje?(https://medium.com/ps-its-huuti/how-to-get-started-with-automapper-and-asp-net-core-2-ecac60ef523f)
            //?? Ta metoda rejestracji AutoMappera jest oznaczona jako obsolate;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseExceptionHandler("/api/Error");
                IDbInitializer dbInitializer = new DatabaseInitializer(); //?? Czy ten inicjalizator bazy ma tutaj być podany jawnie? Bo nie udało mi się go zarejestrować do wywołania automatycznie przez dependency injection
                dbInitializer.PrepareSampleData(); //TODO: do Program.cs (żeby było tworzoene na etapie run, a nie strzału service'u) lub po useMVC
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

        private void ConfigureAutoMapper()
        {
            var mapperConfig = new MapperConfiguration(cfg => 
            {   //[Note - po refleksjach] Jak działa AutoMapper od środka - robi jakieś dziedziczenie / interfejsy, czy po refleksji? Jaki to ma wpływ na wydajność w praktyce?
                //?? Czy użycie przeładowania z MemberList np. Source, powoduje, że mapowany obiekt będzie walidowany pod kątem zawierania wszystkich pól z obiektu Source?
                //Przykład definiowania mapowania innego niż domyślne
                //cfg.CreateMap<BlogPost, BlogPostDTO>().ForMember(destination => destination.AuthorId, opts => opts.MapFrom(source => source.Author.Id)); //[Note] - powinno wystarczyć tylko to uszczegółowione (skoro zostało zdefiniowane uszczegółowienie, to ogół będzie domyślnie) - Czy jeśli mam jakieś propertiesy zdefiniowane osobno, to muszę wtedy dodać mapę od tych standardowych nazwanych jednakowo? 
                cfg.CreateMap<BlogPost, BlogPostDTO>().ReverseMap();
                //cfg.CreateMap<Comment, CommentDTO>().ForMember(destination => destination.AuthorId, opts => opts.MapFrom(source => source.Author.Id)).ReverseMap();
                cfg.CreateMap<Comment, CommentDTO>().ReverseMap();
                cfg.CreateMap<ForumUser, ForumUserDTO>().ReverseMap(); //[Note] - trzeba użyć w odwołaniach EntityFramework'a include, żeby określić, żeby referencje były zaciągane, lub skonfigurować eager loading - Jak zapewnić, by Automapper mapował typy zagnieżdżone? W BlogPostDTO i CommentDTO nie wyświetla nic dla ForumUser
                cfg.CreateMap<NewsMessage, NewsMessageDTO>().ReverseMap();
                cfg.CreateMap<Core.Models.Tag, TagDTO>().ReverseMap();
                cfg.CreateMap<Vote, VoteDTO>().ReverseMap();
                //[Note - reverse] Czy mam definiować 2 wpisy dla AutoMapper'a, żeby było możliwe mapowanie w obie strony (bo np. get powinien mapować odpowiedź repo na DTO, a post powinien mapować DTO na obiekt modelowy)
            });
            mapperConfig.AssertConfigurationIsValid();
            mapper = mapperConfig.CreateMapper();
        }
    }
}