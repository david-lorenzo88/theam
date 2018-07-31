using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Theam.Tests
{
    public class ApiControllerTestBase
    {
        protected HttpClient GetClient()
        {
            var builder = WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>();
            
            var server = new TestServer(builder);
            var client = server.CreateClient();
            
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        
    }
}
