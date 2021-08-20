using Ethereal.Domain;

namespace Ethereal.Application.Exceptions
{
    public class NotFoundException : EtherealException
    {
        public NotFoundException(string errorCode = "not_found", string errorMessage = "Object not found") 
            : base(errorCode, errorMessage)
        {
        }
    }
}