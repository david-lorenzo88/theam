using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Theam.API.Models
{
    public class TokenRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }

        
    }
}
