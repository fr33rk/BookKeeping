using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace PL.BookKeeping.Data.Migrations
{
	public partial class InitialCreate : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Users",
				columns: table => new
				{
					Key = table.Column<int>(nullable: false)
						.Annotation("MySQL:AutoIncrement", true),
					CreationDT = table.Column<DateTime>(nullable: false),
					CreatorKey = table.Column<int>(nullable: true),
					Name = table.Column<string>(nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Users", x => x.Key);
					table.ForeignKey(
						name: "FK_Users_Users_CreatorKey",
						column: x => x.CreatorKey,
						principalTable: "Users",
						principalColumn: "Key",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "Entry",
				columns: table => new
				{
					Key = table.Column<int>(nullable: false)
						.Annotation("MySQL:AutoIncrement", true),
					CreationDT = table.Column<DateTime>(nullable: false),
					CreatorKey = table.Column<int>(nullable: true),
					Description = table.Column<string>(maxLength: 40, nullable: true),
					ParentEntryKey = table.Column<int>(nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Entry", x => x.Key);
					table.ForeignKey(
						name: "FK_Entry_Users_CreatorKey",
						column: x => x.CreatorKey,
						principalTable: "Users",
						principalColumn: "Key",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_Entry_Entry_ParentEntryKey",
						column: x => x.ParentEntryKey,
						principalTable: "Entry",
						principalColumn: "Key",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "Periods",
				columns: table => new
				{
					Key = table.Column<int>(nullable: false)
						.Annotation("MySQL:AutoIncrement", true),
					CreationDT = table.Column<DateTime>(nullable: false),
					CreatorKey = table.Column<int>(nullable: true),
					Name = table.Column<string>(maxLength: 10, nullable: false),
					PeriodStart = table.Column<DateTime>(nullable: false),
					PeriodEnd = table.Column<DateTime>(nullable: false),
					Year = table.Column<int>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Periods", x => x.Key);
					table.ForeignKey(
						name: "FK_Periods_Users_CreatorKey",
						column: x => x.CreatorKey,
						principalTable: "Users",
						principalColumn: "Key",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "ProcessingRules",
				columns: table => new
				{
					Key = table.Column<int>(nullable: false)
						.Annotation("MySQL:AutoIncrement", true),
					CreationDT = table.Column<DateTime>(nullable: false),
					CreatorKey = table.Column<int>(nullable: true),
					EntryKey = table.Column<int>(nullable: true),
					Priority = table.Column<int>(nullable: false),
					NameRule = table.Column<string>(nullable: true),
					AccountRule = table.Column<string>(nullable: true),
					CounterAccountRule = table.Column<string>(nullable: true),
					CodeRule = table.Column<string>(nullable: true),
					MutationTypeRule = table.Column<int>(nullable: true),
					AmountRule = table.Column<string>(nullable: true),
					MutationKindRule = table.Column<string>(nullable: true),
					RemarksRule = table.Column<string>(nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ProcessingRules", x => x.Key);
					table.ForeignKey(
						name: "FK_ProcessingRules_Users_CreatorKey",
						column: x => x.CreatorKey,
						principalTable: "Users",
						principalColumn: "Key",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_ProcessingRules_Entry_EntryKey",
						column: x => x.EntryKey,
						principalTable: "Entry",
						principalColumn: "Key",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "EntryPeriods",
				columns: table => new
				{
					Key = table.Column<int>(nullable: false)
						.Annotation("MySQL:AutoIncrement", true),
					CreationDT = table.Column<DateTime>(nullable: false),
					CreatorKey = table.Column<int>(nullable: true),
					EntryKey = table.Column<int>(nullable: false),
					PeriodKey = table.Column<int>(nullable: false),
					TotalAmount = table.Column<decimal>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_EntryPeriods", x => x.Key);
					table.ForeignKey(
						name: "FK_EntryPeriods_Users_CreatorKey",
						column: x => x.CreatorKey,
						principalTable: "Users",
						principalColumn: "Key",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_EntryPeriods_Entry_EntryKey",
						column: x => x.EntryKey,
						principalTable: "Entry",
						principalColumn: "Key",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_EntryPeriods_Periods_PeriodKey",
						column: x => x.PeriodKey,
						principalTable: "Periods",
						principalColumn: "Key",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "Transactions",
				columns: table => new
				{
					Key = table.Column<int>(nullable: false)
						.Annotation("MySQL:AutoIncrement", true),
					CreationDT = table.Column<DateTime>(nullable: false),
					CreatorKey = table.Column<int>(nullable: true),
					Date = table.Column<DateTime>(nullable: false),
					Name = table.Column<string>(maxLength: 100, nullable: false),
					Account = table.Column<string>(maxLength: 18, nullable: false),
					CounterAccount = table.Column<string>(maxLength: 27, nullable: true),
					Code = table.Column<string>(maxLength: 2, nullable: true),
					MutationType = table.Column<int>(nullable: false),
					Amount = table.Column<decimal>(nullable: false),
					MutationKind = table.Column<string>(maxLength: 20, nullable: true),
					Remarks = table.Column<string>(maxLength: 500, nullable: true),
					EntryPeriodKey = table.Column<int>(nullable: true),
					FingerPrint = table.Column<int>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Transactions", x => x.Key);
					table.ForeignKey(
						name: "FK_Transactions_Users_CreatorKey",
						column: x => x.CreatorKey,
						principalTable: "Users",
						principalColumn: "Key",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_Transactions_EntryPeriods_EntryPeriodKey",
						column: x => x.EntryPeriodKey,
						principalTable: "EntryPeriods",
						principalColumn: "Key",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateIndex(
				name: "IX_Entry_CreatorKey",
				table: "Entry",
				column: "CreatorKey");

			migrationBuilder.CreateIndex(
				name: "IX_Entry_ParentEntryKey",
				table: "Entry",
				column: "ParentEntryKey");

			migrationBuilder.CreateIndex(
				name: "IX_EntryPeriods_CreatorKey",
				table: "EntryPeriods",
				column: "CreatorKey");

			migrationBuilder.CreateIndex(
				name: "IX_EntryPeriods_EntryKey",
				table: "EntryPeriods",
				column: "EntryKey");

			migrationBuilder.CreateIndex(
				name: "IX_EntryPeriods_PeriodKey",
				table: "EntryPeriods",
				column: "PeriodKey");

			migrationBuilder.CreateIndex(
				name: "IX_Periods_CreatorKey",
				table: "Periods",
				column: "CreatorKey");

			migrationBuilder.CreateIndex(
				name: "IX_ProcessingRules_CreatorKey",
				table: "ProcessingRules",
				column: "CreatorKey");

			migrationBuilder.CreateIndex(
				name: "IX_ProcessingRules_EntryKey",
				table: "ProcessingRules",
				column: "EntryKey");

			migrationBuilder.CreateIndex(
				name: "IX_Transactions_CreatorKey",
				table: "Transactions",
				column: "CreatorKey");

			migrationBuilder.CreateIndex(
				name: "IX_Transactions_EntryPeriodKey",
				table: "Transactions",
				column: "EntryPeriodKey");

			migrationBuilder.CreateIndex(
				name: "IX_Users_CreatorKey",
				table: "Users",
				column: "CreatorKey");

			CreateProcedures(migrationBuilder);
		}

		private void CreateProcedures(MigrationBuilder migrationBuilder)
		{
			var storedProcedure =
				"CREATE PROCEDURE `RECALC_AMOUNTS` ( " +
				"	IN PPeriodStart DATE, " +
				"    IN PPeriodEnd DATE " +
				") " +
				"BEGIN " +
				"	DECLARE ep_parent_key int; " +
				"   DECLARE total_amount DECIMAL(10,2); " +
				"   DECLARE ep_key int; " +
				"   DECLARE done INT DEFAULT FALSE; " +
				"    " +
				"	DECLARE lowerCursor CURSOR FOR " +
				"	    SELECT EP.Key, SUM(T.Amount) AS Total_Amount " +
				"          FROM Periods P " +
				"          JOIN EntryPeriods EP on (EP.PeriodKey = P.Key) " +
				"          JOIN Transactions T on (T.EntryPeriodKey = EP.Key) " +
				"          JOIN Entry E ON (E.Key = EP.EntryKey) " +
				"         WHERE P.PeriodStart <= PPeriodEnd " +
				"           AND P.PeriodEnd >= PPeriodStart " +
				"      GROUP BY EP.Key, E.ParentEntryKey;    " +
				"       " +
				"    DECLARE middleCursor CURSOR FOR " +
				"		SELECT PEP.Key, SUM(EP.TotalAmount) AS Total_Amount " +
				"		  FROM Periods P " +
				"		  JOIN EntryPeriods EP on (EP.PeriodKey = P.Key) " +
				"          JOIN Entry E ON (E.Key = EP.EntryKey) " +
				"          JOIN EntryPeriods PEP ON ((PEP.EntryKey = E.ParentEntryKey) AND (PEP.PeriodKey = P.Key)) " +
				"         WHERE P.PeriodStart <= PPeriodEnd " +
				"           AND P.PeriodEnd >= PPeriodStart " +
				"           AND EP.TotalAmount <> 0 " +
				"      GROUP BY PEP.Key; " +
				"      " +
				"    DECLARE upperCursor CURSOR FOR " +
				"		SELECT EP.Key, SUM(CEP.TotalAmount) " +
				"          FROM Entry E " +
				"          JOIN Entry CE ON (CE.ParentEntryKey = E.Key) " +
				"          JOIN EntryPeriods CEP ON (CE.Key = CEP.EntryKey) " +
				"          JOIN Periods P ON (CEP.PeriodKey = P.Key) " +
				"          JOIN EntryPeriods EP ON ((EP.EntryKey = E.Key) AND (EP.PeriodKey = P.Key)) " +
				"         WHERE E.ParentEntryKey is null " +
				"           AND P.PeriodStart <= PPeriodend " +
				"           AND P.PeriodEnd >= PPeriodstart " +
				"      GROUP BY EP.Key; " +
				"      " +
				"	DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = TRUE; " +
				"     " +
				"    /* Calculate the amounts for the lowesr layer of Entryperiods */ " +
				"    UPDATE EntryPeriods EP " +
				"       SET TotalAmount = 0 " +
				"	 WHERE EP.PeriodKey IN (SELECT P.Key " +
				"							  FROM Periods P " +
				"							 WHERE P.PeriodStart <= PPeriodEnd " +
				"							   AND P.PeriodEnd >= PPeriodStart); " +
				"                                  " +
				"	OPEN lowerCursor; " +
				"     " +
				"    lowerCursorLoop: LOOP " +
				"		FETCH lowerCursor INTO ep_key, total_amount; " +
				"		IF done THEN " +
				"			LEAVE lowerCursorLoop; " +
				"		END IF; " +
				"         " +
				"		UPDATE EntryPeriods EP " +
				"           SET TotalAmount = total_amount " +
				"         WHERE EP.Key = ep_key; " +
				"	END LOOP; " +
				"     " +
				"	CLOSE lowerCursor; " +
				" " +
				"	SET done = FALSE; " +
				" " +
				"	OPEN middleCursor; " +
				"     " +
				"    middleCursorLoop: LOOP " +
				"		FETCH middleCursor INTO ep_key, total_amount; " +
				"        IF done THEN " +
				"			LEAVE middleCursorLoop; " +
				"		END IF; " +
				"         " +
				"        UPDATE EntryPeriods EP " +
				"           SET TotalAmount = total_amount " +
				"		 WHERE EP.Key = ep_key; " +
				"    END LOOP; " +
				"     " +
				"    CLOSE middleCursor; " +
				"       " +
				"    SET done = FALSE; " +
				"     " +
				"    OPEN upperCursor; " +
				"     " +
				"    upperCursorLoop: LOOP " +
				"		FETCH upperCursor INTO ep_key, total_amount; " +
				"        IF done THEN " +
				"			LEAVE upperCursorLoop; " +
				"		END IF; " +
				"         " +
				"		UPDATE EntryPeriods EP " +
				"           SET TotalAmount = total_amount " +
				"		 WHERE EP.Key = ep_key; " +
				"    END LOOP; " +
				"     " +
				"    CLOSE upperCursor; " +
				"END ";

			migrationBuilder.Sql(storedProcedure);

			storedProcedure =
				"CREATE PROCEDURE `RESET_TRANSACTIONS` () " +
				"BEGIN " +
				"	UPDATE Transactions T " +
				"       SET T.EntryPeriodKey = NULL; " +
				"        " +
				"	UPDATE EntryPeriods EP " +
				"       SET EP.TotalAmount = 0; " +
				"END ";

			migrationBuilder.Sql(storedProcedure);

			storedProcedure =
				"CREATE PROCEDURE `UPDATE_RULE_PRIORITY`(IN Priority int) " +
				"BEGIN " +
				"	UPDATE ProcessingRules P " +
				"	SET P.Priority = P.Priority + 1 " +
				"	WHERE P.Priority >= Priority " +
				"	ORDER BY P.Priority DESC; " +
				"END ";

			migrationBuilder.Sql(storedProcedure);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			DropProcedures(migrationBuilder);

			migrationBuilder.DropTable(
				name: "ProcessingRules");

			migrationBuilder.DropTable(
				name: "Transactions");

			migrationBuilder.DropTable(
				name: "EntryPeriods");

			migrationBuilder.DropTable(
				name: "Entry");

			migrationBuilder.DropTable(
				name: "Periods");

			migrationBuilder.DropTable(
				name: "Users");
		}

		private void DropProcedures(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql("drop procedure RECALC_AMOUNTS;");
			migrationBuilder.Sql("drop procedure RESET_TRANSACTIONS;");
			migrationBuilder.Sql("drop procedure UPDATE_RULE_PRIORITY;");
		}
	}
}