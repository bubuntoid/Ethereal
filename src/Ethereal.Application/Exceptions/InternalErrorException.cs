namespace Ethereal.Application.Exceptions
{
    public class InternalErrorException : EtherealException
    {
        public InternalErrorException(string errorMessage = "Internal server error", string errorCode = "internal_error") 
            : base(errorCode, errorMessage)
        {
        }
    }
}