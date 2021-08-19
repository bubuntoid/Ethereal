namespace Ethereal.Application.Exceptions
{
    public class InternalErrorException : EtherealException
    {
        public InternalErrorException(string errorCode = "internal_error", string message = "Internal server error") 
            : base(errorCode, message)
        {
        }
    }
}