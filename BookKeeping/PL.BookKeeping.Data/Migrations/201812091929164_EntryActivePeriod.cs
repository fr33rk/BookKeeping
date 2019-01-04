namespace PL.BookKeeping.Data.Migrations
{
	using System.Data.Entity.Migrations;

	public partial class EntryActivePeriod : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.Entry", "ActiveFrom", c => c.DateTime(nullable: false, precision: 0));
			AddColumn("dbo.Entry", "ActiveUntil", c => c.DateTime(precision: 0));
			Sql(@"UPDATE Entry SET activeFrom = ""2000/01/01""");
		}

		public override void Down()
		{
			DropColumn("dbo.Entry", "ActiveUntil");
			DropColumn("dbo.Entry", "ActiveFrom");
		}
	}
}