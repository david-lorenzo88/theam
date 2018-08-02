using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theam.API.Models;

namespace Theam.API.Utils
{
    /// <summary>
    /// Helper class to manage images url
    /// </summary>
    public class ImagesHelper
    {
        public static CustomerDTO[] FillImagesURL(CustomerDTO[] customers, string baseUrl)
        {
            foreach (var c in customers)
            {
                if (!string.IsNullOrEmpty(c.Url))
                {
                    c.Url = string.Format("{0}/{1}", baseUrl.TrimEnd('/'), c.Url);
                }
            }
            return customers;
        }
        public static CustomerDTO FillImagesURL(CustomerDTO customer, string baseUrl)
        {
            if (!string.IsNullOrEmpty(customer.Url))
            {
                customer.Url = string.Format("{0}/{1}", baseUrl.TrimEnd('/'), customer.Url);
            }
            return customer;
        }
    }
}
