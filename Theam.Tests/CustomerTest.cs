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


        [Theory]
        [InlineData("customers")]
        public async Task CustomerInsertAndRead(string controllerName)
        {
            var token = await GetToken("joanaydavid@gmail.com", "David123");

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            using (var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
            {
                content.Add(new StringContent("Test API"), "Name");
                content.Add(new StringContent("testing"), "Surname");

                var response = await _client.PostAsync($"/api/{controllerName}", content);
                response.EnsureSuccessStatusCode();

                var stringResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<BaseResponse<Customer>>(stringResponse);

                Assert.NotNull(result?.result);

                var responseGet = await _client.GetAsync($"/api/{controllerName}/{result.result.Id}");
                responseGet.EnsureSuccessStatusCode();
                var stringResponseGet = await responseGet.Content.ReadAsStringAsync();
                var resultGet = JsonConvert.DeserializeObject<BaseResponse<Customer>>(stringResponseGet);

                Assert.NotNull(result?.result);

                Assert.Equal(result.result.Id, resultGet.result.Id);
            }
        }

        [Theory]
        [InlineData("customers")]
        public async Task CustomersList(string controllerName)
        {
            var token = await GetToken("joanaydavid@gmail.com", "David123");

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            using (var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
            {
                content.Add(new StringContent("Test API"), "Name");
                content.Add(new StringContent("testing"), "Surname");

                var response = await _client.PostAsync($"/api/{controllerName}", content);
                response.EnsureSuccessStatusCode();

                var responseGet = await _client.GetAsync($"/api/{controllerName}");
                responseGet.EnsureSuccessStatusCode();
                var stringResponseGet = await responseGet.Content.ReadAsStringAsync();
                var resultGet = JsonConvert.DeserializeObject<BaseResponse<List<Customer>>>(stringResponseGet);

                Assert.NotNull(resultGet?.result);
                Assert.NotEmpty(resultGet.result);

            }
        }
    }
}
