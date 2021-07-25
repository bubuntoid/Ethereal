using System;
using NUnit.Framework;
using FluentAssertions;

namespace YOVPS.Core.UnitTests
{
    public class VideoDescriptionTests
    {
        private readonly string src = @"
                        1. (0:00) KOPI / KOBASOLO – Swayed in Spring Reiniscence
                        2. (09:13) Memai Siren -Nisemono no Utage
                        3. (34:36) Vivid Undress – Sayonara Dilemma
                        4. (1:23:31) Vivid Undress – Yours";

        private readonly VideoChapter[] expectedChapters = new[]
        {
            new VideoChapter 
            {
                Original = "1. (0:00) KOPI / KOBASOLO – Swayed in Spring Reiniscence", 
                ParsedName = "1. KOPI  KOBASOLO – Swayed in Spring Reiniscence", 
                ParsedTimespan = new TimeSpan(00, 00, 00),
            },
            
            new VideoChapter 
            {
                Original = "2. (09:13) Memai Siren -Nisemono no Utage", 
                ParsedName = "2. Memai Siren -Nisemono no Utage", 
                ParsedTimespan = new TimeSpan(00, 09, 13),
            },
            
            new VideoChapter 
            {
                Original = "3. (34:36) Vivid Undress – Sayonara Dilemma", 
                ParsedName = "3. Vivid Undress – Sayonara Dilemma", 
                ParsedTimespan = new TimeSpan(00, 34, 36),
            },
            
            new VideoChapter 
            {
                Original = "4. (1:23:31) Vivid Undress – Yours", 
                ParsedName = "4. Vivid Undress – Yours", 
                ParsedTimespan = new TimeSpan(01, 23, 31),
            }
        };

        // [Test]
        // public async Task Test()
        // {
        //     var content = await new VideoSplitter().ProcessAsync("https://www.youtube.com/watch?v=TmQyfUpyeFk&list=LL&index=5&ab_channel=NewRetroWave");
        //     
        //     var zipFileStream = File.Create(Path.Combine(Directory.GetCurrentDirectory(), "archive.zip"));
        //     await zipFileStream.WriteAsync(content, 0, content.Length);
        //     zipFileStream.Flush();
        //     zipFileStream.Close();
        // }
        
        [Test]
        public void ParseChaptersTest()
        {
            var description = new VideoDescription(src);

            var chapters = description.ParseChapters();

            chapters.Should().BeEquivalentTo(expectedChapters);
        }
    }
}