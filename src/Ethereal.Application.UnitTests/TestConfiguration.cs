using System.IO;

namespace Ethereal.Application.UnitTests
{
    public class TestConfiguration
    {
        public static string GetTempDirectory()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "temp");
            Directory.CreateDirectory(path);
            return path;
        }  
    }
}