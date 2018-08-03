using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Theam.API.Models;
using Xunit;

namespace Theam.Tests
{
    public class TestCases : ApiControllerTestBase
    {
        [Theory]
        [InlineData("users")]
        public async Task UsersRestrictAccessToUserRole(string controllerName)
        {
            var token = await GetToken("user@test.com", "David123");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var ex = await Assert.ThrowsAsync<HttpRequestException>(async () =>
                await PostAsync<UserDTO>(controllerName, new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(new UserDTO
                {
                    Email = "usertest@test.com",
                    Name = "user test",
                    Surname = "lastname test",
                    Password = "Pwd123!",
                    Roles = new List<UserRoleDTO>
                    {
                        new UserRoleDTO
                        {
                            Role = new RoleDTO{Id = API.Utils.Constants.ROLE_ADMIN_ID }
                        }
                    }

                }), System.Text.Encoding.UTF8, "application/json"))
            );

            Assert.Contains("403", ex.Message);
            Assert.Contains("Forbidden", ex.Message);
        }



        [Theory]
        [InlineData("users")]
        public async Task UserInsertAndRead(string controllerName)
        {
            var token = await GetToken("admin@test.com", "David123");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var result = await PostAsync<UserDTO>(controllerName, new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(new UserDTO
            {
                Email = "usertest@test.com",
                Name = "user test",
                Surname = "lastname test",
                Password = "Pwd123!",
                Roles = new List<UserRoleDTO>
                {
                    new UserRoleDTO
                    {
                        Role = new RoleDTO{Id = API.Utils.Constants.ROLE_ADMIN_ID }
                    }
                }
            }), System.Text.Encoding.UTF8, "application/json"));

            Assert.NotNull(result?.result);
            Assert.True(result.success);

            var resultGet = await GetAsync<UserDTO>(controllerName, result.result.Id.ToString());

            Assert.NotNull(resultGet?.result);

            Assert.Equal(result.result.Id, resultGet.result.Id);

        }



        [Theory]
        [InlineData("users")]
        public async Task UsersList(string controllerName)
        {
            var token = await GetToken("admin@test.com", "David123");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var result = await PostAsync<UserDTO>(controllerName, new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(new UserDTO
            {
                Email = "usertest@test.com",
                Name = "user test",
                Surname = "lastname test",
                Password = "Pwd123!",
                Roles = new List<UserRoleDTO>
                {
                    new UserRoleDTO
                    {
                        Role = new RoleDTO{Id = API.Utils.Constants.ROLE_ADMIN_ID }
                    }
                }
            }), System.Text.Encoding.UTF8, "application/json"));

            var resultGet = await GetAsync<List<UserDTO>>(controllerName);

            Assert.NotNull(resultGet?.result);
            Assert.NotEmpty(resultGet.result);

        }

        [Theory]
        [InlineData("users")]
        public async Task UserDeleteRead(string controllerName)
        {
            var token = await GetToken("admin@test.com", "David123");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var result = await PostAsync<UserDTO>(controllerName, new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(new UserDTO
            {
                Email = "usertest@test.com",
                Name = "user test",
                Surname = "lastname test",
                Password = "Pwd123!",
                Roles = new List<UserRoleDTO>
                {
                    new UserRoleDTO
                    {
                        Role = new RoleDTO{Id = API.Utils.Constants.ROLE_ADMIN_ID }
                    }
                }
            }), System.Text.Encoding.UTF8, "application/json"));

            Assert.NotNull(result?.result);

            var resultGet = await GetAsync<UserDTO>(controllerName, result.result.Id.ToString());

            Assert.NotNull(resultGet?.result);

            Assert.Equal(result.result.Id, resultGet.result.Id);

            var resultDelete = await DeleteAsync<UserDTO>(controllerName, result.result.Id);

            Assert.NotNull(resultDelete);
            Assert.True(resultDelete.success);

        }




        [Theory]
        [InlineData("customers")]
        public async Task CustomerInsertAndRead(string controllerName)
        {
            var token = await GetToken("user@test.com", "David123");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using (var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
            {
                content.Add(new StringContent("Test API"), "Name");
                content.Add(new StringContent("testing"), "Surname");

                var result = await PostAsync<CustomerDTO>(controllerName, content);

                Assert.NotNull(result?.result);

                var resultGet = await GetAsync<CustomerDTO>(controllerName, result.result.Id.ToString());

                Assert.NotNull(resultGet?.result);

                Assert.Equal(result.result.Id, resultGet.result.Id);
            }
        }

        [Theory]
        [InlineData("customers")]
        public async Task CustomerInsertAndReadWithImage(string controllerName)
        {
            var token = await GetToken("user@test.com", "David123");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using (var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
            {

                content.Add(new StringContent("Test API"), "Name");
                content.Add(new StringContent("testing"), "Surname");

                content.Add(new ByteArrayContent(File.ReadAllBytes("test_img.png")), "ImageFile", "test_img.png");

                var result = await PostAsync<CustomerDTO>(controllerName, content);

                Assert.NotNull(result?.result);

                var resultGet = await GetAsync<CustomerDTO>(controllerName, result.result.Id.ToString());

                Assert.NotNull(resultGet?.result);

                Assert.Equal(result.result.Id, resultGet.result.Id);
                Assert.NotEmpty(resultGet.result.Url);
                Assert.NotNull(resultGet.result.Url);
            }
        }

        [Theory]
        [InlineData("customers")]
        public async Task CustomersList(string controllerName)
        {
            var token = await GetToken("user@test.com", "David123");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using (var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
            {
                content.Add(new StringContent("Test API"), "Name");
                content.Add(new StringContent("testing"), "Surname");

                var result = await PostAsync<CustomerDTO>(controllerName, content);

                var resultGet = await GetAsync<List<CustomerDTO>>(controllerName);

                Assert.NotNull(resultGet?.result);
                Assert.NotEmpty(resultGet.result);
            }
        }

        [Theory]
        [InlineData("customers")]
        public async Task CustomerDeleteReadWithImage(string controllerName)
        {
            var token = await GetToken("user@test.com", "David123");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using (var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
            {

                content.Add(new StringContent("Test API"), "Name");
                content.Add(new StringContent("testing"), "Surname");

                content.Add(new ByteArrayContent(File.ReadAllBytes("test_img.png")), "ImageFile", "test_img.png");

                var result = await PostAsync<CustomerDTO>(controllerName, content);

                Assert.NotNull(result?.result);

                var resultGet = await GetAsync<CustomerDTO>(controllerName, result.result.Id.ToString());

                Assert.NotNull(resultGet?.result);

                Assert.Equal(result.result.Id, resultGet.result.Id);
                Assert.NotEmpty(resultGet.result.Url);
                Assert.NotNull(resultGet.result.Url);

                var resultDelete = await DeleteAsync<CustomerDTO>(controllerName, result.result.Id);

                Assert.NotNull(resultDelete);
                Assert.True(resultDelete.success);
            }
        }
    }
}
