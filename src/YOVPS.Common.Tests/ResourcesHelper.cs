using System.IO;

namespace YOVPS.Common.Tests
{
    public static class ResourcesHelper
    {
        public static string GetResourceFullPath(string name)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "Resources", name);
        }
    }
}