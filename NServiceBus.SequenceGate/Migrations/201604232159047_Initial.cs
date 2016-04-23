namespace NServiceBus.SequenceGate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TrackedObjectEntities",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        SequenceGateId = c.String(nullable: false),
                        ObjectId = c.String(nullable: false),
                        SequenceAnchor = c.Long(nullable: false),
                        ScopeId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TrackedObjectEntities");
        }
    }
}
