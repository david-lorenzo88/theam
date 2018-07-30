using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theam.API.Models
{
    public class BaseResponse<T>
    {
        public bool success { get; set; }
        public string errorMsg { get; set; }
        public T result { get; set; }
    }
}
