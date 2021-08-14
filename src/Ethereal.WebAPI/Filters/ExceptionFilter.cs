using System.Threading.Tasks;
using Ethereal.Application.Exceptions;
using Ethereal.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ethereal.WebAPI.Filters
{
    public class ExceptionFilter : IAsyncExceptionFilter 
    {
        public Task OnExceptionAsync(ExceptionContext context)
        {
            var exception = context.Exception as EtherealException;
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