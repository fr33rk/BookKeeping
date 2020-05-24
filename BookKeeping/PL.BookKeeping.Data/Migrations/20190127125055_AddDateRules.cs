using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace PL.BookKeeping.Data.Migrations
{
	public partial class AddDateRules : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<DateTime>("DateBeforeRule", "ProcessingRules");
			migrationBuilder.AddColumn<DateTime>("DateAfterRule", "ProcessingRules");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn("DateAfterRule", "ProcessingRules");
			migrationBuilder.DropColumn("DateBeforeRule", "ProcessingRules");
		}
	}
}