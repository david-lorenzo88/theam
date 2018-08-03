using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Theam.API.Data;
using Theam.API.Models;
using Xunit;

namespace Theam.Tests
{
    public class ApiControllerTestBase
    {
        protected readonly HttpClient _client;

        public ApiControllerTestBase()
        {
            _client = GetClient();
        }

        private HttpClient GetClient()
        {
            //Create Test Server
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
            //Create and configure Client
            var client = server.CreateClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        protected async Task<string> GetToken(string userName, string password)
        {
            var response = await _client.PostAsJsonAsync<TokenRequest>($"/api/auth", new TokenRequest
            {
                Email = userName,
                Password = password,
            });
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<BaseResponse<string>>(stringResponse);

            Assert.True(result?.success);
            Assert.NotNull(result?.result);
            Assert.NotEmpty(result?.result);

            return result.result;
        }

        protected async Task<BaseResponse<T>> PostAsync<T>(string controllerName, HttpContent content)
        {
            var response = await _client.PostAsync($"/api/{controllerName}", content);
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<BaseResponse<T>>(stringResponse);
        }

        protected async Task<BaseResponse<T>> PutAsync<T>(string controllerName, HttpContent content, int id)
        {
            var response = await _client.PutAsync($"/api/{controllerName}/{id}", content);
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<BaseResponse<T>>(stringResponse);
        }

        protected async Task<BaseResponse<T>> DeleteAsync<T>(string controllerName, int id)
        {
            var response = await _client.DeleteAsync($"/api/{controllerName}/{id}");
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<BaseResponse<T>>(stringResponse);
        }

        protected async Task<BaseResponse<T>> GetAsync<T>(string controllerName, string id = null)
        {
            var response = await _client.GetAsync($"/api/{controllerName}/{id}");
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<BaseResponse<T>>(stringResponse);
        }

    }
}
