using System;

namespace Ethereal.Application.Exceptions;

public abstract class EtherealException : Exception
{
    protected EtherealException(string errorCode, string errorMessage)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public string ErrorCode { get; }
    public string ErrorMessage { get; }
}