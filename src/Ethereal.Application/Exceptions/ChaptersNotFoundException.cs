namespace Ethereal.Application.Exceptions
{
    public class ChaptersNotFoundException : YovpsException
    {
        public ChaptersNotFoundException(string message = "Chapters not found", 
            string description = "Try to specify description manually") 
            : base(message, description)
        {
            
        }
    }
}