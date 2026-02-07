namespace HIS.Api.Simujlator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TestResultDetails",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        LISParamCode = c.String(maxLength: 100),
                        LISParamValue = c.String(maxLength: 100),
                        LISParamUnit = c.String(maxLength: 200),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedOn = c.DateTime(nullable: false),
                        TestResultId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TestResults", t => t.TestResultId)
                .Index(t => t.TestResultId);
            
            CreateTable(
                "dbo.TestResults",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        SampleNo = c.String(maxLength: 100),
                        HISTestCode = c.String(maxLength: 100),
                        LISTestCode = c.String(maxLength: 100),
                        SpecimenCode = c.String(maxLength: 100),
                        SpecimenName = c.String(maxLength: 255),
                        ResultDate = c.DateTime(nullable: false),
                        SampleCollectionDate = c.DateTime(nullable: false),
                        SampleReceivedDate = c.DateTime(nullable: false),
                        AuthorizationDate = c.DateTime(),
                        AuthorizedBy = c.String(maxLength: 100),
                        ReviewDate = c.DateTime(),
                        ReviewedBy = c.String(maxLength: 100),
                        TechnicianNote = c.String(maxLength: 1000),
                        DoctorNote = c.String(maxLength: 1000),
                        CreatedBy = c.String(maxLength: 100),
                        CreatedOn = c.DateTime(nullable: false),
                        PatientId = c.Long(nullable: false),
                        TestRequestId = c.Long(nullable: false),
                        EquipmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AlterColumn("dbo.Staging_Testparameter", "AgeFrom", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Staging_Testparameter", "AgeTo", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TestResultDetails", "TestResultId", "dbo.TestResults");
            DropIndex("dbo.TestResultDetails", new[] { "TestResultId" });
            AlterColumn("dbo.Staging_Testparameter", "AgeTo", c => c.String(nullable: false, maxLength: 15, unicode: false));
            AlterColumn("dbo.Staging_Testparameter", "AgeFrom", c => c.String(nullable: false, maxLength: 15, unicode: false));
            DropTable("dbo.TestResults");
            DropTable("dbo.TestResultDetails");
        }
    }
}
