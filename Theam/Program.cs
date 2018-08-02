using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Theam.API.Data;

namespace Theam
{
    /// <summary>
    /// Initializes the web host where the app will run
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main method that runs the app
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                using (var context = services.GetRequiredService<ApiContext>())
                {
                    DbSeeder.Initialize(context).Wait();
                }
            }

            host.Run();
        }
        /// <summary>
        /// Builds the web host where the app will run
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
