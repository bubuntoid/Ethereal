using FluentMigrator;

namespace Ethereal.Domain.Migrations
{
    [Migration(1)]
    public class InitializeDatabaseMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("processingJob")
                .WithColumn("id").AsGuid().PrimaryKey()
                .WithColumn("status").AsString(32).NotNullable()
                .WithColumn("totalStepsCount").AsInt32().Nullable()
                .WithColumn("currentStepIndex").AsInt32().Nullable()
                .WithColumn("currentStepDescription").AsString(256).Nullable()
                .WithColumn("localPath").AsString(2000).NotNullable()
                .WithColumn("createdDate").AsDateTime().NotNullable()
                .WithColumn("processedDate").AsDateTime().Nullable();

            Create.Table("processingJobVideo")
                .WithColumn("processingJobId").AsGuid().PrimaryKey().ForeignKey("processingJob", "id")
                .WithColumn("url").AsString(2000).NotNullable()
                .WithColumn("id").AsString(2000).NotNullable()
                .WithColumn("title").AsString(2000).NotNullable()
                .WithColumn("duration").AsTime().NotNullable()
                .WithColumn("originalDescription").AsString().Nullable()
                .WithColumn("description").AsString().Nullable();
        }
    }
}