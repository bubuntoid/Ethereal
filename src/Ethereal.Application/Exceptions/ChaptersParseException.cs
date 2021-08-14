namespace Ethereal.Application.Exceptions
{
    public class ChaptersParseException : YovpsException
    {
        public ChaptersParseException(string message = "Could not parse time codes from description",
            string description = "Make sure that first time code is 0:00") 
            : base(message, description)
        {
            
        }
    }
}