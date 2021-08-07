using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using YOVPS.Core.Exceptions;
using YOVPS.WebAPI.Models;

namespace YOVPS.WebAPI.Filters
{
    public class ExceptionFilter : IAsyncExceptionFilter 
    {
        public Task OnExceptionAsync(ExceptionContext context)
        {
            var exception = context.Exception as YovpsException;
            context.Result = new ObjectResult(new ApiErrorDto
            {
                Error = context.Exception.Message,
                Description = exception?.Description ?? context.Exception.StackTrace,
            })
            {
                StatusCode = 400
            };

            return Task.CompletedTask;
        }
    }
}