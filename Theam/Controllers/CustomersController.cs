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
using Theam.API.Filters;
using Theam.API.Models;
using Theam.API.Repositories;
using Theam.API.Utils;

namespace Theam.API.Controllers
{
    /// <summary>
    /// Controller that manages the customer data
    /// </summary>
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
        /// <summary>
        /// Gets the customer list
        /// </summary>
        /// <returns>the customer list</returns>
        [HttpGet]
        public async Task<ActionResult<BaseResponse<CustomerDTO[]>>> Get()
        {
            return CreateResponse(true, ImagesHelper.FillImagesURL(_mapper.Map<CustomerDTO[]>(await _repo.Get()), _options.ImagesBaseUrl));
        }

        /// <summary>
        /// Gets a customer by id
        /// </summary>
        /// <param name="id">Id of the customer</param>
        /// <returns>the customer data</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<BaseResponse<CustomerDTO>>> Get(int id)
        {
            var customers = await _repo.Get(u => u.Id == id);
            if (customers == null || customers.Length == 0)
            {
                return CreateResponse<CustomerDTO>(false, null, "Customer not found");
            }
            return CreateResponse(true, ImagesHelper.FillImagesURL(_mapper.Map<CustomerDTO>(customers.First()), _options.ImagesBaseUrl));
        }

        /// <summary>
        /// Inserts a customer in the system
        /// </summary>
        /// <param name="customer">the customer data</param>
        /// <returns>the customer data with assigned ID</returns>
        [HttpPost]
        public async Task<ActionResult<BaseResponse<CustomerDTO>>> Post([FromForm] CustomerDTO customer)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CreateResponse<CustomerDTO>(false, null, GetModelStateErrors()); 
                }
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
                return CreateResponse(true, ImagesHelper.FillImagesURL(customer, _options.ImagesBaseUrl));
            }
            catch (Exception ex)
            {
                return CreateResponse<CustomerDTO>(false, null, ex.Message);
            }
        }

        /// <summary>
        /// Updates a customer in the system
        /// </summary>
        /// <param name="id">the customer ID</param>
        /// <param name="customer">the customer data</param>
        /// <returns>the customer data</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<BaseResponse<CustomerDTO>>> Put(int id, [FromForm] CustomerDTO customer)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CreateResponse<CustomerDTO>(false, null, GetModelStateErrors());
                }

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

                return CreateResponse(true, ImagesHelper.FillImagesURL(customer, _options.ImagesBaseUrl));
            }
            catch (Exception ex)
            {
                return CreateResponse<CustomerDTO>(false, null, ex.Message);
            }
        }

        /// <summary>
        /// Deletes a customer from the system
        /// </summary>
        /// <param name="id">ID of the customer to delete</param>
        /// <returns>success true or false depending on the result of the deletion</returns>
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
