using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Ethereal.Application.UnitTests
{
    public static class ResourcesHelper
    {
        public static async Task SaveResourceToFileAsync(string resourceFilename, string path)
        {
            await using var resource = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream($"Ethereal.Application.UnitTests.Resources.{resourceFilename}");
            
            if (resource == null)
            {
                throw new Exception($"Could not find resource {resourceFilename}");
            }
            
            await using var file = new FileStream(path, FileMode.Create, FileAccess.Write);
            await resource.CopyToAsync(file);
        }
    }
}