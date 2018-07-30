using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Theam.API.Models;
using Theam.API.Repositories;

namespace Theam.API.Controllers
{
    

    public class BaseApiController : ControllerBase
    {
        protected readonly IMapper _mapper;
        protected readonly IRepository<User> _userRepo;

        public BaseApiController(IMapper mapper, IRepository<User> userRepo)
        {
            _mapper = mapper;
            _userRepo = userRepo;
        }

        protected ActionResult<BaseResponse<T>> CreateResponse<T>(bool success, T result = default(T), string errorMsg = null)
        {
            var response = new BaseResponse<T> { success = success, errorMsg = errorMsg, result = result };
            if (success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        protected async Task<UserDTO> GetCurrentUser()
        {
            var currentUserEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            return _mapper.Map<UserDTO>((await _userRepo.Get(u => u.Email == currentUserEmail.Value)).FirstOrDefault());
        }

        protected async Task<string> SaveFile(IFormFile file, string physicalBasePath)
        {
            try
            {
                Directory.CreateDirectory(physicalBasePath);

                var fileName = file.FileName;

                var physicalPath = Path.Combine(physicalBasePath, fileName);
                int i = 0;
                while (System.IO.File.Exists(physicalPath))
                {
                    fileName = string.Format("{0}-{1}{2}", Path.GetFileNameWithoutExtension(file.FileName), i++, Path.GetExtension(file.FileName));
                    physicalPath = Path.Combine(physicalBasePath, fileName);
                }

                using (var fileStream = new FileStream(physicalPath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return fileName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected bool DeleteFile(string physicalPath)
        {
            try
            {
                System.IO.File.Delete(physicalPath);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}