using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace MicroService.APIGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {  
            NLogBuilder.ConfigureNLog("nlog.config");
            Console.Title = "MicroService.APIGateway";
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, builder) =>
            { 
                builder.AddJsonFile("ocelot.json", false, true);
            }) 
                .UseStartup<Startup>();
    }
}
