using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Theam.API.Models;
using Xunit;

namespace Theam.Tests
{
    public class CustomerTest : ApiControllerTestBase
    {
        private readonly HttpClient _client;

        public CustomerTest()
        {
            _client = base.GetClient();
        }

        private async Task<string> GetToken(string userName, string password)
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

        private async Task<BaseResponse<T>> PostAsync<T>(string controllerName, HttpContent content)
        {
            var response = await _client.PostAsync($"/api/{controllerName}", content);
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<BaseResponse<T>>(stringResponse);
        }

        private async Task<BaseResponse<T>> GetAsync<T>(string controllerName, string id = null)
        {
            var response = await _client.GetAsync($"/api/{controllerName}/{id}");
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<BaseResponse<T>>(stringResponse);
        }

        [Theory]
        [InlineData("customers")]
        public async Task CustomerInsertAndRead(string controllerName)
        {
            var token = await GetToken("user@test.com", "David123");

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            using (var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
            {
                content.Add(new StringContent("Test API"), "Name");
                content.Add(new StringContent("testing"), "Surname");

                var result = await PostAsync<Customer>(controllerName, content);

                Assert.NotNull(result?.result);

                var resultGet = await GetAsync<Customer>(controllerName, result.result.Id.ToString());

                Assert.NotNull(result?.result);

                Assert.Equal(result.result.Id, resultGet.result.Id);
            }
        }

        [Theory]
        [InlineData("customers")]
        public async Task CustomersList(string controllerName)
        {
            var token = await GetToken("user@test.com", "David123");
            
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            using (var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
            {
                content.Add(new StringContent("Test API"), "Name");
                content.Add(new StringContent("testing"), "Surname");

                var result = await PostAsync<Customer>(controllerName, content);

                var resultGet = await GetAsync<List<Customer>>(controllerName);

                Assert.NotNull(resultGet?.result);
                Assert.NotEmpty(resultGet.result);
            }
        }
    }
}
