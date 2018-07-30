using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theam.API.Models
{
    public class CustomerDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Url { get; set; }
        public virtual UserDTO UserCreated { get; set; }
        public virtual UserDTO UserModified { get; set; }
        [JsonIgnore]
        public IFormFile ImageFile { get; set; }
    }
}
