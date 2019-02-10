using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace PL.BookKeeping.Data.Migrations
{
	public partial class EntryActivePeriod : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<DateTime>("ActiveFrom", "Entry", nullable: false);
			migrationBuilder.AddColumn<DateTime>("ActiveUntil", "Entry", nullable: true);
			migrationBuilder.Sql(@"UPDATE Entry SET activeFrom = ""2000/01/01""");

			migrationBuilder.InsertData(
				table: "Users",
				columns: new[] { "Key", "CreationDT", "CreatorKey", "Name" },
				values: new object[] { 1, DateTime.Now, null, "System user" });

			migrationBuilder.InsertData(
				table: "Entry",
				columns: new[] { "Key", "ActiveFrom", "ActiveUntil", "CreationDT", "CreatorKey", "Description", "ParentEntryKey" },
				values: new object[] { 1,
					new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
					null,
					DateTime.Now,
					1,
					"Uitgaven",
					null });

			migrationBuilder.InsertData(
				table: "Entry",
				columns: new[] { "Key", "ActiveFrom", "ActiveUntil", "CreationDT", "CreatorKey", "Description", "ParentEntryKey" },
				values: new object[] { 2,
					new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
					null,
					DateTime.Now,
					1,
					"Inkomsten",
					null });
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DeleteData(
				table: "Entry",
				keyColumn: "Key",
				keyValue: 1);

			migrationBuilder.DeleteData(
				table: "Entry",
				keyColumn: "Key",
				keyValue: 2);

			migrationBuilder.DeleteData(
				table: "Users",
				keyColumn: "Key",
				keyValue: 1);

			migrationBuilder.DropColumn("ActiveUntil", "Entry");
			migrationBuilder.DropColumn("ActiveFrom", "Entry");
		}
	}
}