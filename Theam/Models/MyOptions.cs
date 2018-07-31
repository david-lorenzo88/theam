using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theam.API.Models
{
    public class MyOptions
    {
        public string SecurityKey { get; set; }
        public string Domain { get; set; }
        public string ImagesUploadPath { get; set; }
        public string ImagesBaseUrl { get; set; }
    }
}
