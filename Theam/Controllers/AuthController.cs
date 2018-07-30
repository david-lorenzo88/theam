using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Theam.API.Models;
using Theam.API.Repositories;
using Theam.API.Utils;

namespace Theam.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseApiController
    {

        private readonly MyOptions _options;
        public AuthController(IRepository<User> repo, IOptions<MyOptions> optionsAccesor, IMapper mapper) : base (mapper, repo)
        {
            _options = optionsAccesor.Value;
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse<string>>> Token([FromBody] TokenRequest request)
        {
            var users = await _userRepo.Get(u => u.Email == request.Email && PasswordHasherHelper.ComparePassword(request.Password, u.Password));

            if (users != null && users.Length > 0)
            {
                var _user = users.First();

                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Email, request.Email));

                if(_user.Roles.Any(r => r.Role.Name == Constants.ROLE_ADMIN))
                {
                    claims.Add(new Claim(Constants.CLAIM_IS_ADMIN_USER, "1"));
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecurityKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _options.Domain,
                    audience: _options.Domain,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds);

                return CreateResponse(true, new JwtSecurityTokenHandler().WriteToken(token));
            }

            return CreateResponse<string>(false, null, "Could not verify email and password");
        }
    }
}