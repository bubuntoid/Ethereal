using AutofacContrib.NSubstitute;
using AutoFixture;
using Ethereal.Domain;
using YoutubeExplode;

namespace Ethereal.Application.UnitTests
{
    public class WithInMemoryDatabaseTestBase
    {
        protected EtherealDbContext DbContext { get; } 
        
        protected AutoSubstitute Substitute { get; }

        protected IFixture Fixture { get; }

        protected TestSettings Settings;
        
        protected WithInMemoryDatabaseTestBase()
        {
            DbContext = new EtherealInMemoryDatabase();
            
            Fixture = new Fixture();
            Fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            
            Settings = new TestSettings();

            Substitute = AutoSubstitute
                .Configure()
                .Provide(DbContext)
                .Provide(new YoutubeClient())
                .Build();
        }
    }
}