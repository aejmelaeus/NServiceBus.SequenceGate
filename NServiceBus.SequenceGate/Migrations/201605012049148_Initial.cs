namespace NServiceBus.SequenceGate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrackedObjectEntities", "SequenceAnchor", c => c.Long(nullable: false));
            AlterColumn("dbo.TrackedObjectEntities", "ScopeId", c => c.String());
            DropColumn("dbo.TrackedObjectEntities", "TimeStampUTC");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TrackedObjectEntities", "TimeStampUTC", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TrackedObjectEntities", "ScopeId", c => c.String(nullable: false));
            DropColumn("dbo.TrackedObjectEntities", "SequenceAnchor");
        }
    }
}
