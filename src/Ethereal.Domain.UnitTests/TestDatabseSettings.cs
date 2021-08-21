namespace Ethereal.Domain.UnitTests
{
    public class TestDatabaseSettings : IDatabaseSettings
    {
        public string ConnectionString { get; } = "Server=localhost;Port=5432;Database=etherealDb;User Id=postgres;Password=root;";
    }
}