using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Theam.API.Models
{
    public class CustomerDTO
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        
        public string Url { get; set; }
        public virtual UserDTO UserCreated { get; set; }
        public virtual UserDTO UserModified { get; set; }
        [JsonIgnore]
        public IFormFile ImageFile { get; set; }
    }
}
