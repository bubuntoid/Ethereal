using System;

namespace Ethereal.Application.Exceptions
{
    public abstract class EtherealException : Exception
    {
        public string Description { get; set; } 
        
        protected EtherealException(string message, string description) : base(message)
        {
            Description = description;
        }
    }
}