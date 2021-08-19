using Ethereal.Domain;

namespace Ethereal.Application.Exceptions
{
    public class NotFoundException : EtherealException
    {
        public NotFoundException(string errorCode = "not_found", string message = "Object not found") 
            : base(errorCode, message)
        {
        }
    }
}