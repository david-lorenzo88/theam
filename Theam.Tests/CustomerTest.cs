using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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


        [Theory]
        [InlineData("customers")]
        public async Task CustomerInsertAndRead(string controllerName)
        {
            var response = await _client.PostAsJsonAsync<Customer>($"/api/{controllerName}", new Customer
            {
                Name = "Test API",
                Surname = "testing",
            });
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

        [Theory]
        [InlineData("customers")]
        public async Task CustomersList(string controllerName)
        {
            var response = await _client.PostAsJsonAsync<Customer>($"/api/{controllerName}", new Customer
            {
                Name = "Test API",
                Surname = "testing",
            });
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
