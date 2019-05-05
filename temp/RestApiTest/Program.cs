using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace RestApiTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
            //.UseSerilog() //?? Coś się zmieniło? W przykładach w necie używają tego wywołania, a u mnie tego nie widzi. W NIP też tego nie widziałem.
            .ConfigureLogging(logging => {
                logging.ClearProviders(); //?? Dlaczego to jest tu dodane w przykładzie NIP? Nie znalazłem do tego żadnej dokumentacji, ani użycia w necie. Czy jest ryzyko, że jakiś provider będzie zarejstrowany jeszcze z innego punktu?
                logging.AddConsole();
                //logging.AddFile("Logs/BlogPosts-{Date}.log");
                logging.SetMinimumLevel(LogLevel.Information);
                });
    }
}
