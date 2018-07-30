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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Constants.POLICIY_ADMIN_USER)]
    public class UsersController : BaseApiController
    {
        
        public UsersController(IRepository<User> repo, IMapper mapper) : base(mapper, repo)
        {
        }
        // GET api/values
        [HttpGet]
        public async Task<ActionResult<BaseResponse<UserDTO[]>>> Get()
        {
            return CreateResponse(true, _mapper.Map<UserDTO[]>(await _userRepo.Get()));
        }

        // GET api/values/5
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

        // POST api/values
        [HttpPost]
        public async Task<ActionResult<BaseResponse<UserDTO>>> Post([FromBody] UserDTO user)
        {
            try
            {
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

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<ActionResult<BaseResponse<UserDTO>>> Put(int id, [FromBody] UserDTO user)
        {
            try
            {
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

        // DELETE api/values/5
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
