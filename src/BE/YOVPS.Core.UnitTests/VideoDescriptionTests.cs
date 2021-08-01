using System;
using NUnit.Framework;
using FluentAssertions;
using YOVPS.Core.Exceptions;

namespace YOVPS.Core.UnitTests
{
    public class VideoDescriptionTests
    {
        private readonly string description = @"
                        1. (0:00) KOPI / KOBASOLO – Swayed in Spring Reiniscence
                        2. (09:13) Memai Siren -Nisemono no Utage
                        3. (34:36) Vivid Undress – Sayonara Dilemma
                        4. (1:23:31) Vivid Undress – Yours";

        private readonly VideoChapter[] expectedChapters = new[]
        {
            new VideoChapter 
            {
                Original = "1. (0:00) KOPI / KOBASOLO – Swayed in Spring Reiniscence", 
                Name = "1. KOPI  KOBASOLO – Swayed in Spring Reiniscence", 
                StartTimespan = new TimeSpan(00, 00, 00),
                EndTimespan = new TimeSpan(00, 09, 13),
            },
            
            new VideoChapter 
            {
                Original = "2. (09:13) Memai Siren -Nisemono no Utage", 
                Name = "2. Memai Siren -Nisemono no Utage", 
                StartTimespan = new TimeSpan(00, 09, 13),
                EndTimespan = new TimeSpan(00, 34, 36),
            },
            
            new VideoChapter 
            {
                Original = "3. (34:36) Vivid Undress – Sayonara Dilemma", 
                Name = "3. Vivid Undress – Sayonara Dilemma", 
                StartTimespan = new TimeSpan(00, 34, 36),
                EndTimespan = new TimeSpan(01, 23, 31)
            },
            
            new VideoChapter 
            {
                Original = "4. (1:23:31) Vivid Undress – Yours", 
                Name = "4. Vivid Undress – Yours", 
                StartTimespan = new TimeSpan(01, 23, 31),
            }
        };

        [Test]
        public void ParseChaptersTest()
        {
            var videoDescription = new VideoDescription(this.description);

            var chapters = videoDescription.ParseChapters();

            chapters.Should().BeEquivalentTo(expectedChapters);
        }

        [TestCase("")]
        [TestCase("String without timecodes")]
        [TestCase("String without timecodes\n with new line")]
        [TestCase(null)]
        public void DescriptionHasNoTimeCodes_ChaptersParseExceptionExpected(string desc)
        {
            var videoDescription = new VideoDescription(desc);
            Assert.Throws<ChaptersParseException>(() => videoDescription.ParseChapters());
        }
    }
}