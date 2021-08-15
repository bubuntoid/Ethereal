using System.Threading.Tasks;
using Ethereal.Application.Exceptions;
using Ethereal.Domain;
using Ethereal.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ethereal.WebAPI.Filters
{
    public class ExceptionFilter : IAsyncExceptionFilter 
    {
        public Task OnExceptionAsync(ExceptionContext context)
        {
            if (context.Exception is EtherealException etherealException)
            {
                context.Result = new ObjectResult(new ApiErrorDto
                {
                    ErrorCode = etherealException.ErrorCode,
                    ErrorMessage = etherealException.ErrorMessage,
                })
                {
                    StatusCode = 400
                };
                
                return Task.CompletedTask;
            }
            
            context.Result = new ObjectResult(new ApiErrorDto
            {
                ErrorCode = "internal_error",
                ErrorMessage=  context.Exception.Message,
            })
            {
                StatusCode = 400
            };
            
            return Task.CompletedTask;
        }
    }
}