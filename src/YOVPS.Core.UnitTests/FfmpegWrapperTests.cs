using System.IO;
using NUnit.Framework;

namespace YOVPS.Core.UnitTests
{
    [TestFixture]
    public class FfmpegWrapperTests
    {
        [SetUp]
        public void SetUp()
        {
            FfmpegWrapper.ExecutablesPath = Path.Combine(Directory.GetCurrentDirectory(), "ffmpeg", "ffmpeg.exe");
        }
    }
}