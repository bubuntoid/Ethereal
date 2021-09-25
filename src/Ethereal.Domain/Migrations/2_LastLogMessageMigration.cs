using FluentMigrator;

namespace Ethereal.Domain.Migrations
{
    [Migration(2)]
    public class LastLogMessageMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Alter.Table("processingJob").AddColumn("lastLogMessage").AsString(1024).Nullable();
        }
    }
}