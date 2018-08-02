using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Theam.API.Models;
using Theam.API.Repositories;

namespace Theam.API.Controllers
{
    
    /// <summary>
    /// Base controller for generic actions
    /// </summary>
    public class BaseApiController : ControllerBase
    {
        protected readonly IMapper _mapper;
        protected readonly IRepository<User> _userRepo;

        public BaseApiController(IMapper mapper, IRepository<User> userRepo)
        {
            _mapper = mapper;
            _userRepo = userRepo;
        }
        /// <summary>
        /// Generic method to create the response and be consistent through all the API
        /// </summary>
        /// <typeparam name="T">Type of response object</typeparam>
        /// <param name="success">Indicates if the response is sucessfull or not</param>
        /// <param name="result">Response object</param>
        /// <param name="errorMsg">indicates the error message if applicable (success == false)</param>
        /// <returns></returns>
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
        /// <summary>
        /// Gets current logged in user
        /// </summary>
        /// <returns>User data</returns>
        protected async Task<UserDTO> GetCurrentUser()
        {
            var currentUserEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            return _mapper.Map<UserDTO>((await _userRepo.Get(u => u.Email == currentUserEmail.Value)).FirstOrDefault());
        }
        /// <summary>
        /// Saves a file in the filesystem and returns the generated fileName
        /// </summary>
        /// <param name="file">File data</param>
        /// <param name="physicalBasePath">physical path where to save the file</param>
        /// <returns></returns>
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
        /// <summary>
        /// Deletes a specified file from the filesystem
        /// </summary>
        /// <param name="physicalPath">the file path</param>
        /// <returns>true if the file was deleted successfully, false otherwise</returns>
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
        /// <summary>
        /// Generates a string with all validation Model errors
        /// </summary>
        /// <returns>the string with all errors concatenated</returns>
        protected string GetModelStateErrors()
        {
            StringBuilder b = new StringBuilder();
            foreach (var value in ModelState.Values)
            {
                foreach (var error in value.Errors)
                {
                    b.AppendFormat("{0} ", error.ErrorMessage);
                }
            }
            return b.ToString();
        }
    }
}