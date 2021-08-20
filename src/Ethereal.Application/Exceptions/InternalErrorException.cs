namespace Ethereal.Application.Exceptions
{
    public class InternalErrorException : EtherealException
    {
        public InternalErrorException(string errorCode = "internal_error", string errorMessage = "Internal server error") 
            : base(errorCode, errorMessage)
        {
        }
    }
}