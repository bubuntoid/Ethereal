using System.Threading.Tasks;
using Ethereal.Application.Exceptions;
using Ethereal.Domain;
using Ethereal.WebAPI.Contracts.Infrastructure;
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
                context.Result = new ObjectResult(new ErrorResponseDto
                {
                    ErrorCode = etherealException.ErrorCode,
                    ErrorMessage = etherealException.Message,
                })
                {
                    StatusCode = 400
                };
                
                return Task.CompletedTask;
            }
            
            context.Result = new ObjectResult(new ErrorResponseDto
            {
                ErrorCode = "internal_error",
                ErrorMessage = context.Exception.Message + context.Exception.StackTrace,
            })
            {
                StatusCode = 400
            };
            
            return Task.CompletedTask;
        }
    }
}