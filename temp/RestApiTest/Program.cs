using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
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
                logging.AddFile("Logs/BlogPosts-{Date}.log"); //?? Czy to może kolidować z jakimś innym pakietem, bo nie działa i żadne kroki z neta nie pomogły
                });
    }
}
