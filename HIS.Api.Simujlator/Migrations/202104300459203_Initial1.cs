namespace HIS.Api.Simujlator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Staging_TestReq",
                c => new
                    {
                        REQID = c.String(nullable: false, maxLength: 30, unicode: false),
                        TESTID = c.String(nullable: false, maxLength: 8, unicode: false),
                        TYP = c.String(maxLength: 3, unicode: false),
                        CANCELLED_HDR = c.String(maxLength: 9, unicode: false),
                        CANCELLED_DTL = c.String(maxLength: 9, unicode: false),
                        IPNO = c.String(maxLength: 50, unicode: false),
                        BEDNO = c.String(maxLength: 50, unicode: false),
                        MRNO = c.String(maxLength: 40, unicode: false),
                        REQNO = c.String(nullable: false, maxLength: 30, unicode: false),
                        DEPTNM = c.String(maxLength: 42, unicode: false),
                        GROUPID = c.String(maxLength: 10, unicode: false),
                        GROUPNM = c.String(maxLength: 50, unicode: false),
                        DEPTID = c.String(maxLength: 8, unicode: false),
                        TESTNM = c.String(maxLength: 100, unicode: false),
                        PATIENTNM = c.String(nullable: false, maxLength: 100, unicode: false),
                        AGE = c.Decimal(precision: 7, scale: 2, storeType: "numeric"),
                        YMD = c.String(maxLength: 2, unicode: false),
                        SX = c.String(maxLength: 3, unicode: false),
                        REQDTTM = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        RCDATE = c.String(maxLength: 12, unicode: false),
                        SADATE = c.String(maxLength: 12, unicode: false),
                        COLDATE = c.String(maxLength: 12, unicode: false),
                        COLLTIME = c.DateTime(precision: 7, storeType: "datetime2"),
                        PRINTDT = c.DateTime(precision: 7, storeType: "datetime2"),
                        PRINTTM = c.DateTime(precision: 7, storeType: "datetime2"),
                        PRINTDTTM = c.String(maxLength: 22, unicode: false),
                        APPROVEDDT = c.DateTime(precision: 7, storeType: "datetime2"),
                        APPROVEDTM = c.DateTime(precision: 7, storeType: "datetime2"),
                        APPROVEDTTM = c.String(maxLength: 22, unicode: false),
                        PERFORMEDDT = c.DateTime(precision: 7, storeType: "datetime2"),
                        PERFORMEDTM = c.DateTime(precision: 7, storeType: "datetime2"),
                        DRNAME = c.String(maxLength: 100, unicode: false),
                        IPOPDOCNM = c.String(maxLength: 75, unicode: false),
                        Modified = c.Int(nullable: false, defaultValue: 1),
                    })
                .PrimaryKey(t => new { t.REQID, t.TESTID });
            
            DropTable("dbo.Staging_TestReq");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Staging_TestReq",
                c => new
                    {
                        REQID = c.String(nullable: false, maxLength: 30, unicode: false),
                        TESTID = c.String(nullable: false, maxLength: 8, unicode: false),
                        TYP = c.String(maxLength: 3, unicode: false),
                        CANCELLED_HDR = c.String(maxLength: 9, unicode: false),
                        CANCELLED_DTL = c.String(maxLength: 9, unicode: false),
                        IPNO = c.String(maxLength: 50, unicode: false),
                        BEDNO = c.String(maxLength: 50, unicode: false),
                        MRNO = c.String(maxLength: 40, unicode: false),
                        REQNO = c.String(maxLength: 30, unicode: false),
                        DEPTNM = c.String(maxLength: 42, unicode: false),
                        GROUPID = c.String(maxLength: 10, unicode: false),
                        GROUPNM = c.String(maxLength: 50, unicode: false),
                        DEPTID = c.String(maxLength: 8, unicode: false),
                        TESTNM = c.String(maxLength: 100, unicode: false),
                        PATIENTNM = c.String(maxLength: 100, unicode: false),
                        AGE = c.Decimal(precision: 7, scale: 2, storeType: "numeric"),
                        YMD = c.String(maxLength: 2, unicode: false),
                        SX = c.String(maxLength: 3, unicode: false),
                        REQDTTM = c.DateTime(precision: 7, storeType: "datetime2"),
                        RCDATE = c.String(maxLength: 12, unicode: false),
                        SADATE = c.String(maxLength: 12, unicode: false),
                        COLDATE = c.String(maxLength: 12, unicode: false),
                        COLLTIME = c.DateTime(precision: 7, storeType: "datetime2"),
                        PRINTDT = c.DateTime(precision: 7, storeType: "datetime2"),
                        PRINTTM = c.DateTime(precision: 7, storeType: "datetime2"),
                        PRINTDTTM = c.String(maxLength: 22, unicode: false),
                        APPROVEDDT = c.DateTime(precision: 7, storeType: "datetime2"),
                        APPROVEDTM = c.DateTime(precision: 7, storeType: "datetime2"),
                        APPROVEDTTM = c.String(maxLength: 22, unicode: false),
                        PERFORMEDDT = c.DateTime(precision: 7, storeType: "datetime2"),
                        PERFORMEDTM = c.DateTime(precision: 7, storeType: "datetime2"),
                        DRNAME = c.String(maxLength: 100, unicode: false),
                        IPOPDOCNM = c.String(maxLength: 75, unicode: false),
                    })
                .PrimaryKey(t => new { t.REQID, t.TESTID });           
            
        }
    }
}
