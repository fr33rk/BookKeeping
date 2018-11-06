namespace PL.BookKeeping.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ObsoleteEntry : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Entry", "IsActive", c => c.Boolean(nullable: false));
			Sql("UPDATE Entry SET IsActive = TRUE");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Entry", "IsActive");
        }
    }
}
