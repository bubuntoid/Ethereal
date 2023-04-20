namespace Ethereal.Application.Exceptions;

public class NotFoundException : EtherealException
{
    public NotFoundException(string errorMessage = "Object not found", string errorCode = "not_found")
        : base(errorCode, errorMessage)
    {
    }
}