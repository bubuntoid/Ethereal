namespace Ethereal.Application.Exceptions
{
    public class InvalidOperationException : EtherealException
    {
        public InvalidOperationException(string errorMessage = "Invalid operation",
            string errorCode = "invalid_operation")
            : base(errorCode, errorMessage)
        {
        }
    }
}