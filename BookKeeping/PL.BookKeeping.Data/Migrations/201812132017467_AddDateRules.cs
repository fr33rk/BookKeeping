namespace PL.BookKeeping.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDateRules : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProcessingRules", "DateBeforeRule", c => c.DateTime(precision: 0));
            AddColumn("dbo.ProcessingRules", "DateAfterRule", c => c.DateTime(precision: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProcessingRules", "DateAfterRule");
            DropColumn("dbo.ProcessingRules", "DateBeforeRule");
        }
    }
}
