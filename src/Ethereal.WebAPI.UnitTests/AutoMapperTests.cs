using AutoMapper;
using NUnit.Framework;

namespace Ethereal.WebAPI.UnitTests;

[TestFixture]
public class AutoMapperTests
{
    [Test]
    public void AutoMapper_Configuration_IsValid()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>());
        config.AssertConfigurationIsValid();
    }
}