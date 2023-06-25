using System;
using Ethereal.Application.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace Ethereal.Application.UnitTests.Tests;

[TestFixture]
public class VideoDescriptionTests
{
    private readonly string description = @"
                        1. (0:00) KOPI / KOBASOLO – Swayed in Spring Reiniscence
                        https://www.youtube.com/watch?v=YO_spdAYjPk&ab_channel=SomeKindOfClassic
                        
                        2. (09:13) Memai Siren -Nisemono no Utage
                        https://www.youtube.com/watch?v=YO_spdAYjPk&ab_channel=SomeKindOfClassic

                        3. (34:36) Vivid Undress – Sayonara Dilemma
                        https://www.youtube.com/watch?v=YO_spdAYjPk&ab_channel=SomeKindOfClassic

                        4. (1:23:31) Vivid Undress – Yours
                        https://www.youtube.com/watch?v=YO_spdAYjPk&ab_channel=SomeKindOfClassic

                        5. Cutting Crew - (I Just) Died in Your Arms 03:17:21
                        6. Roxy Music - More Than This 03:21:40
                        7. Toto - Africa 03:28:00
                        7. Toto - Africa 04:30:00
                        7. Toto - Africa 05:30:00";

    private readonly VideoChapter[] expectedChapters =
    {
        new()
        {
            Original = "1. (0:00) KOPI / KOBASOLO – Swayed in Spring Reiniscence",
            Name = "1. KOPI  KOBASOLO – Swayed in Spring Reiniscence",
            UniqueName = "1. KOPI  KOBASOLO – Swayed in Spring Reiniscence",
            StartTimespan = new TimeSpan(00, 00, 00),
            EndTimespan = new TimeSpan(00, 09, 13),
        },

        new()
        {
            Original = "2. (09:13) Memai Siren -Nisemono no Utage",
            Name = "2. Memai Siren -Nisemono no Utage",
            UniqueName = "2. Memai Siren -Nisemono no Utage",
            StartTimespan = new TimeSpan(00, 09, 13),
            EndTimespan = new TimeSpan(00, 34, 36)
        },

        new()
        {
            Original = "3. (34:36) Vivid Undress – Sayonara Dilemma",
            Name = "3. Vivid Undress – Sayonara Dilemma",
            UniqueName = "3. Vivid Undress – Sayonara Dilemma",
            StartTimespan = new TimeSpan(00, 34, 36),
            EndTimespan = new TimeSpan(01, 23, 31)
        },

        new()
        {
            Original = "4. (1:23:31) Vivid Undress – Yours",
            Name = "4. Vivid Undress – Yours",
            UniqueName = "4. Vivid Undress – Yours",
            StartTimespan = new TimeSpan(01, 23, 31),
            EndTimespan = new TimeSpan(03, 17, 21)
        },

        new()
        {
            Original = "5. Cutting Crew - (I Just) Died in Your Arms 03:17:21",
            Name = "5. Cutting Crew - (I Just) Died in Your Arms",
            UniqueName = "5. Cutting Crew - (I Just) Died in Your Arms",
            StartTimespan = new TimeSpan(03, 17, 21),
            EndTimespan = new TimeSpan(03, 21, 40)
        },

        new()
        {
            Original = "6. Roxy Music - More Than This 03:21:40",
            Name = "6. Roxy Music - More Than This",
            UniqueName = "6. Roxy Music - More Than This",
            StartTimespan = new TimeSpan(03, 21, 40),
            EndTimespan = new TimeSpan(03, 28, 00)
        },

        new()
        {
            Original = "7. Toto - Africa 03:28:00",
            Name = "7. Toto - Africa",
            UniqueName = "7. Toto - Africa",
            StartTimespan = new TimeSpan(03, 28, 00), 
            EndTimespan = new TimeSpan(04, 30, 00)
        },

        new()
        {
            Original = "7. Toto - Africa 04:30:00",
            Name = "7. Toto - Africa",
            UniqueName = "7. Toto - Africa (2)",
            StartTimespan = new TimeSpan(04, 30, 00),
            EndTimespan = new TimeSpan(05, 30, 00)
        },
        
        new()
        {
            Original = "7. Toto - Africa 05:30:00",
            Name = "7. Toto - Africa",
            UniqueName = "7. Toto - Africa (3)",
            StartTimespan = new TimeSpan(05, 30, 00)
        },
    };

    [Test]
    public void ParseChaptersTest()
    {
        var videoDescription = new VideoDescription(description);

        var chapters = videoDescription.ParseChapters();

        chapters.Should().BeEquivalentTo(expectedChapters, opt => opt
            .Excluding(o => o.Index));
    }

    [TestCase("")]
    [TestCase("String without timecodes")]
    [TestCase("String without timecodes\n with new line")]
    [TestCase(null)]
    public void DescriptionHasNoTimeCodes_ChaptersParseExceptionExpected(string desc)
    {
        var videoDescription = new VideoDescription(desc);

        var ex = Assert.Throws<InternalErrorException>(() => videoDescription.ParseChapters());

        Assert.That(ex.ErrorMessage.Contains("Could not parse any chapter"));
    }
}