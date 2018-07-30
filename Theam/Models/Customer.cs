using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theam.API.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Url { get; set; }
        public virtual User UserCreated { get; set; }
        public virtual User UserModified { get; set; }
    }
}
