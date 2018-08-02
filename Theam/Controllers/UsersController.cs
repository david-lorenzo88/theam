using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Theam.API.Data;
using Theam.API.Models;
using Theam.API.Repositories;
using Theam.API.Utils;

namespace Theam.API.Controllers
{
    /// <summary>
    /// Controller that manages the user data
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Constants.POLICIY_ADMIN_USER)]
    public class UsersController : BaseApiController
    {
        
        public UsersController(IRepository<User> repo, IMapper mapper) : base(mapper, repo)
        {
        }
        /// <summary>
        /// Gets the users list
        /// </summary>
        /// <returns>the users list</returns>
        [HttpGet]
        public async Task<ActionResult<BaseResponse<UserDTO[]>>> Get()
        {
            return CreateResponse(true, _mapper.Map<UserDTO[]>(await _userRepo.Get()));
        }

        /// <summary>
        /// Gets a user by id
        /// </summary>
        /// <param name="id">Id of the user</param>
        /// <returns>the user data</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<BaseResponse<UserDTO>>> Get(int id)
        {
            var users = await _userRepo.Get(u => u.Id == id);

            if (users == null || users.Length == 0)
            {
                return CreateResponse<UserDTO>(false, null, "User not found");
            }
            return CreateResponse(true, _mapper.Map<UserDTO>(users.First()));
        }

        /// <summary>
        /// Inserts a user in the system
        /// </summary>
        /// <param name="user">the user data</param>
        /// <returns>the user data with assigned ID</returns>
        [HttpPost]
        public async Task<ActionResult<BaseResponse<UserDTO>>> Post([FromBody] UserDTO user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CreateResponse<UserDTO>(false, null, GetModelStateErrors());
                }
                user.Password = PasswordHasherHelper.ComputePassword(user.Password);
                _userRepo.Add(_mapper.Map<User>(user));
                await _userRepo.SaveAsync();
                return CreateResponse(true, _mapper.Map<UserDTO>((await _userRepo.Get(u => u.Id == user.Id)).FirstOrDefault()));
            }
            catch (Exception ex)
            {
                return CreateResponse<UserDTO>(false, null, ex.Message);
            }
        }

        /// <summary>
        /// Updates a user in the system
        /// </summary>
        /// <param name="id">the user ID</param>
        /// <param name="customer">the user data</param>
        /// <returns>the user data</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<BaseResponse<UserDTO>>> Put(int id, [FromBody] UserDTO user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CreateResponse<UserDTO>(false, null, GetModelStateErrors());
                }
                var users = await _userRepo.Get(u => u.Id == id);

                if (users == null || users.Length == 0)
                {
                    return CreateResponse<UserDTO>(false, null, "User not found");
                }
                if (!string.IsNullOrEmpty(user.Password))
                {
                    user.Password = PasswordHasherHelper.ComputePassword(user.Password);
                }
                user.Id = id;
                _userRepo.Update(_mapper.Map<User>(user));
                await _userRepo.SaveAsync();

                return CreateResponse(true, _mapper.Map<UserDTO>((await _userRepo.Get(u => u.Id == user.Id)).FirstOrDefault()));
            }
            catch (Exception ex)
            {
                return CreateResponse<UserDTO>(false, null, ex.Message);
            }
        }

        /// <summary>
        /// Deletes a user from the system
        /// </summary>
        /// <param name="id">ID of the user to delete</param>
        /// <returns>success true or false depending on the result of the deletion</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<BaseResponse<UserDTO>>> Delete(int id)
        {
            try
            {
                var users = await _userRepo.Get(u => u.Id == id);

                if (users == null || users.Length == 0)
                {
                    return CreateResponse<UserDTO>(false, null, "User not found");
                }
                _userRepo.Delete(users.First());
                await _userRepo.SaveAsync();

                return CreateResponse<UserDTO>(true);
            }
            catch (Exception ex)
            {
                return CreateResponse<UserDTO>(false, null, ex.Message);
            }
        }
    }
}
