namespace NServiceBus.SequenceGate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SequenceMembers",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        SequenceGateId = c.String(nullable: false, maxLength: 128),
                        ScopeId = c.String(nullable: false, maxLength: 128),
                        EndpointName = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.SequenceGateId, t.ScopeId, t.EndpointName }, unique: true);
            
            CreateTable(
                "dbo.SequenceObjects",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        SequenceMemberId = c.Long(nullable: false),
                        SequenceAnchor = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.Id, t.SequenceMemberId })
                .ForeignKey("dbo.SequenceMembers", t => t.SequenceMemberId, cascadeDelete: true)
                .Index(t => t.SequenceMemberId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SequenceObjects", "SequenceMemberId", "dbo.SequenceMembers");
            DropIndex("dbo.SequenceObjects", new[] { "SequenceMemberId" });
            DropIndex("dbo.SequenceMembers", new[] { "SequenceGateId", "ScopeId", "EndpointName" });
            DropTable("dbo.SequenceObjects");
            DropTable("dbo.SequenceMembers");
        }
    }
}
