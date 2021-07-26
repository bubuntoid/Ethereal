using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using YOVPS.Core.Exceptions;

namespace YOVPS.WebAPI.Filters
{
    public class ExceptionFilter : IAsyncExceptionFilter 
    {
        public Task OnExceptionAsync(ExceptionContext context)
        {
            var exception = context.Exception as YovpsException;
            context.Result = new ObjectResult(new
            {
                error = context.Exception.Message,
                description = exception?.Description ?? null,
            })
            {
                StatusCode = 400
            };

            return Task.CompletedTask;
        }
    }
}