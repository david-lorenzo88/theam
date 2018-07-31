using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Theam.API.Data;

namespace Theam.Tests
{
    public class ApiControllerTestBase
    {
        protected HttpClient GetClient()
        {
            var builder = WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>();

            var server = new TestServer(builder);

            //Launch DB Seeder
            using (var scope = server.Host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                using (var context = services.GetRequiredService<ApiContext>())
                {
                    DbSeeder.Initialize(context).Wait();
                }
            }

            var client = server.CreateClient();
            
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        
    }
}
