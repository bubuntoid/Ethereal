using System;

namespace Ethereal.Application.Exceptions
{
    public abstract class YovpsException : Exception
    {
        public string Description { get; set; } 
        
        protected YovpsException(string message, string description) : base(message)
        {
            Description = description;
        }
    }
}