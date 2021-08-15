using System;

namespace Ethereal.Application.Exceptions
{
    public abstract class EtherealException : Exception
    {
        public string ErrorCode { get; }
        
        public string ErrorMessage { get; } 
        
        protected EtherealException(string errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
            ErrorMessage = message;
        }
    }
}