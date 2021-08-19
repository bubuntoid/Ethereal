using FluentMigrator;

namespace Ethereal.Domain.Migrations
{
    [Migration(1)]
    public class InitializeDatabaseMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("");
        }
    }
}