using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theam.API.Models;

namespace Theam.API.Filters
{
    public class ValidateModelActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(new BaseResponse<string> { success = false, errorMsg = GetModelStateErrors(context.ModelState), result = null });
            }
        }

        private string GetModelStateErrors(ModelStateDictionary modelState)
        {
            StringBuilder b = new StringBuilder();
            foreach (var value in modelState.Values)
            {
                foreach (var error in value.Errors)
                {
                    b.AppendFormat("{0}. ", error);
                }
            }
            return b.ToString();
        }
    }
}
