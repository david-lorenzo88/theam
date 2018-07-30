using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Theam.API.Data;
using Theam.API.Models;
using Theam.API.Repositories;

namespace Theam.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomersController : BaseApiController
    {
        private readonly IRepository<Customer> _repo;
        private readonly MyOptions _options;
        private readonly IHostingEnvironment _hostingEnvironment;
        public CustomersController(IRepository<Customer> repo, IMapper mapper, IOptions<MyOptions> optionsAccesor, 
            IRepository<User> userRepo, IHostingEnvironment hostingEnvironment) : base(mapper, userRepo)
        {
            _repo = repo;
            _options = optionsAccesor.Value;
            _hostingEnvironment = hostingEnvironment;
        }
        // GET api/values
        [HttpGet]
        public async Task<ActionResult<BaseResponse<CustomerDTO[]>>> Get()
        {
            return CreateResponse(true, _mapper.Map<CustomerDTO[]>(await _repo.Get()));
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BaseResponse<CustomerDTO>>> Get(int id)
        {
            var customers = await _repo.Get(u => u.Id == id);
            if (customers == null || customers.Length == 0)
            {
                return CreateResponse<CustomerDTO>(false, null, "Customer not found");
            }
            return CreateResponse(true, _mapper.Map<CustomerDTO>(customers.First()));
        }

        // POST api/values
        [HttpPost]
        public async Task<ActionResult<BaseResponse<CustomerDTO>>> Post([FromForm] CustomerDTO customer)
        {
            try
            {
                var user = await GetCurrentUser();

                customer.UserCreated = user;
                customer.UserModified = user;

                if (customer.ImageFile != null && customer.ImageFile.Length > 0)
                {
                    //Handle image upload
                    var directoryPath = Path.Combine(_hostingEnvironment.WebRootPath, _options.ImagesUploadPath);

                    var fileName = await SaveFile(customer.ImageFile, directoryPath);

                    var dbPath = Path.Combine(_options.ImagesUploadPath, fileName).Replace("\\", "/");
                    customer.Url = dbPath;   
                }
                var dao = _mapper.Map<Customer>(customer);
                _repo.Add(dao);
                await _repo.SaveAsync();
                customer.Id = dao.Id;
                return CreateResponse(true, customer);
            }
            catch (Exception ex)
            {
                return CreateResponse<CustomerDTO>(false, null, ex.Message);
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<ActionResult<BaseResponse<CustomerDTO>>> Put(int id, [FromForm] CustomerDTO customer)
        {
            try
            {
                var customers = await _repo.Get(u => u.Id == id);

                if (customers == null || customers.Length == 0)
                {
                    return CreateResponse<CustomerDTO>(false, null, "User not found");
                }
                var oldCustomer = customers.First();

                customer.Id = id;
                customer.UserModified = await GetCurrentUser();

                if (customer.ImageFile != null && customer.ImageFile.Length > 0)
                {
                    //Handle image upload
                    var directoryPath = Path.Combine(_hostingEnvironment.WebRootPath, _options.ImagesUploadPath);

                    if (!string.IsNullOrEmpty(oldCustomer.Url))
                    {
                        //There is old image, delete it
                        DeleteFile(Path.Combine(directoryPath, Path.GetFileName(oldCustomer.Url)));
                    }

                    var fileName = await SaveFile(customer.ImageFile, directoryPath);

                    var dbPath = Path.Combine(_options.ImagesUploadPath, fileName).Replace("\\", "/");
                    customer.Url = dbPath;
                }

                _repo.Update(_mapper.Map<Customer>(customer));
                await _repo.SaveAsync();

                return CreateResponse(true, customer);
            }
            catch (Exception ex)
            {
                return CreateResponse<CustomerDTO>(false, null, ex.Message);
            }
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<BaseResponse<CustomerDTO>>> Delete(int id)
        {
            try
            {
                var customers = await _repo.Get(u => u.Id == id);

                if (customers == null || customers.Length == 0)
                {
                    return CreateResponse<CustomerDTO>(false, null, "Customer not found");
                }
                _repo.Delete(customers.First());
                await _repo.SaveAsync();

                return CreateResponse<CustomerDTO>(true);
            }
            catch (Exception ex)
            {
                return CreateResponse<CustomerDTO>(false, null, ex.Message);
            }
        }
    }
}
