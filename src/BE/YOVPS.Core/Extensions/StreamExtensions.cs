using System.IO;

namespace YOVPS.Core.Extensions
{
    public static class StreamExtensions
    {
        public static byte[] ReadFully(this Stream input)
        {
            using var ms = new MemoryStream();
            input.CopyTo(ms);
            return ms.ToArray();
        }
    }
}