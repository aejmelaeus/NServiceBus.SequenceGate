namespace NServiceBus.SequenceGate.Tests.Acceptance.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VIPs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VIPs",
                c => new
                    {
                        UserId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.VIPs");
        }
    }
}
