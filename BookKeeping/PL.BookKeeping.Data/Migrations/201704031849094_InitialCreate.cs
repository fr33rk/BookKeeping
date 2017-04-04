namespace PL.BookKeeping.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Entry",
                c => new
                    {
                        Key = c.Int(nullable: false, identity: true),
                        Description = c.String(maxLength: 40, storeType: "nvarchar"),
                        ParentEntryKey = c.Int(),
                        CreationDT = c.DateTime(nullable: false, precision: 0),
                        CreatorKey = c.Int(),
                    })
                .PrimaryKey(t => t.Key)
                .ForeignKey("dbo.Entry", t => t.ParentEntryKey)
                .ForeignKey("dbo.Users", t => t.CreatorKey)
                .Index(t => t.ParentEntryKey)
                .Index(t => t.CreatorKey);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Key = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        CreationDT = c.DateTime(nullable: false, precision: 0),
                        CreatorKey = c.Int(),
                    })
                .PrimaryKey(t => t.Key)
                .ForeignKey("dbo.Users", t => t.CreatorKey)
                .Index(t => t.CreatorKey);
            
            CreateTable(
                "dbo.EntryPeriods",
                c => new
                    {
                        Key = c.Int(nullable: false, identity: true),
                        EntryKey = c.Int(nullable: false),
                        PeriodKey = c.Int(nullable: false),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreationDT = c.DateTime(nullable: false, precision: 0),
                        CreatorKey = c.Int(),
                    })
                .PrimaryKey(t => t.Key)
                .ForeignKey("dbo.Users", t => t.CreatorKey)
                .ForeignKey("dbo.Entry", t => t.EntryKey, cascadeDelete: true)
                .ForeignKey("dbo.Periods", t => t.PeriodKey, cascadeDelete: true)
                .Index(t => t.EntryKey)
                .Index(t => t.PeriodKey)
                .Index(t => t.CreatorKey);
            
            CreateTable(
                "dbo.Periods",
                c => new
                    {
                        Key = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 10, storeType: "nvarchar"),
                        PeriodStart = c.DateTime(nullable: false, precision: 0),
                        PeriodEnd = c.DateTime(nullable: false, precision: 0),
                        Year = c.Int(nullable: false),
                        CreationDT = c.DateTime(nullable: false, precision: 0),
                        CreatorKey = c.Int(),
                    })
                .PrimaryKey(t => t.Key)
                .ForeignKey("dbo.Users", t => t.CreatorKey)
                .Index(t => t.CreatorKey);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        Key = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false, precision: 0),
                        Name = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        Account = c.String(nullable: false, maxLength: 18, storeType: "nvarchar"),
                        CounterAccount = c.String(maxLength: 27, storeType: "nvarchar"),
                        Code = c.String(maxLength: 2, storeType: "nvarchar"),
                        MutationType = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MutationKind = c.String(maxLength: 20, storeType: "nvarchar"),
                        Remarks = c.String(maxLength: 500, storeType: "nvarchar"),
                        EntryPeriodKey = c.Int(),
                        FingerPrint = c.Int(nullable: false),
                        CreationDT = c.DateTime(nullable: false, precision: 0),
                        CreatorKey = c.Int(),
                    })
                .PrimaryKey(t => t.Key)
                .ForeignKey("dbo.Users", t => t.CreatorKey)
                .ForeignKey("dbo.EntryPeriods", t => t.EntryPeriodKey)
                .Index(t => t.EntryPeriodKey)
                .Index(t => t.CreatorKey);
            
            CreateTable(
                "dbo.ProcessingRules",
                c => new
                    {
                        Key = c.Int(nullable: false, identity: true),
                        EntryKey = c.Int(),
                        Priority = c.Int(nullable: false),
                        NameRule = c.String(unicode: false),
                        AccountRule = c.String(unicode: false),
                        CounterAccountRule = c.String(unicode: false),
                        CodeRule = c.String(unicode: false),
                        MutationTypeRule = c.Int(),
                        AmountRule = c.String(unicode: false),
                        MutationKindRule = c.String(unicode: false),
                        RemarksRule = c.String(unicode: false),
                        CreationDT = c.DateTime(nullable: false, precision: 0),
                        CreatorKey = c.Int(),
                    })
                .PrimaryKey(t => t.Key)
                .ForeignKey("dbo.Users", t => t.CreatorKey)
                .ForeignKey("dbo.Entry", t => t.EntryKey)
                .Index(t => t.EntryKey)
                .Index(t => t.CreatorKey);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProcessingRules", "EntryKey", "dbo.Entry");
            DropForeignKey("dbo.ProcessingRules", "CreatorKey", "dbo.Users");
            DropForeignKey("dbo.Transactions", "EntryPeriodKey", "dbo.EntryPeriods");
            DropForeignKey("dbo.Transactions", "CreatorKey", "dbo.Users");
            DropForeignKey("dbo.EntryPeriods", "PeriodKey", "dbo.Periods");
            DropForeignKey("dbo.Periods", "CreatorKey", "dbo.Users");
            DropForeignKey("dbo.EntryPeriods", "EntryKey", "dbo.Entry");
            DropForeignKey("dbo.EntryPeriods", "CreatorKey", "dbo.Users");
            DropForeignKey("dbo.Entry", "CreatorKey", "dbo.Users");
            DropForeignKey("dbo.Users", "CreatorKey", "dbo.Users");
            DropForeignKey("dbo.Entry", "ParentEntryKey", "dbo.Entry");
            DropIndex("dbo.ProcessingRules", new[] { "CreatorKey" });
            DropIndex("dbo.ProcessingRules", new[] { "EntryKey" });
            DropIndex("dbo.Transactions", new[] { "CreatorKey" });
            DropIndex("dbo.Transactions", new[] { "EntryPeriodKey" });
            DropIndex("dbo.Periods", new[] { "CreatorKey" });
            DropIndex("dbo.EntryPeriods", new[] { "CreatorKey" });
            DropIndex("dbo.EntryPeriods", new[] { "PeriodKey" });
            DropIndex("dbo.EntryPeriods", new[] { "EntryKey" });
            DropIndex("dbo.Users", new[] { "CreatorKey" });
            DropIndex("dbo.Entry", new[] { "CreatorKey" });
            DropIndex("dbo.Entry", new[] { "ParentEntryKey" });
            DropTable("dbo.ProcessingRules");
            DropTable("dbo.Transactions");
            DropTable("dbo.Periods");
            DropTable("dbo.EntryPeriods");
            DropTable("dbo.Users");
            DropTable("dbo.Entry");
        }
    }
}
