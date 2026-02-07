using HIS.Api.Simujlator.Models.Entity;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace HIS.Api.Simujlator.DataAccess.Config
{
    /// <summary>
    /// Application DB Context Class,Inherits DbContext
    /// </summary>
    [DbConfigurationType(typeof(EntityFrameworkConfiguration))]
    public class ApplicationDBContext : DbContext
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public ApplicationDBContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<ApplicationDBContext>(new CreateDatabaseIfNotExists<ApplicationDBContext>());
        }

        public static ApplicationDBContext Create()
        {
            return new ApplicationDBContext();
        }

        public virtual DbSet<StagingTestRequisition> StagingTestRequisition { get; set; }
        public virtual DbSet<StagingTestMaster> StagingTestMaster { get; set; }
        public virtual DbSet<StagingTestparameter> StagingTestparameter { get; set; }

        public virtual DbSet<TestResultDetail> TestResultDetails { get; set; }
        public virtual DbSet<TestResult> TestResults { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (modelBuilder != null)
            {
                modelBuilder.Entity<StagingTestRequisition>()
                .Property(e => e.Type)
                .IsUnicode(false);

                modelBuilder.Entity<StagingTestRequisition>()
                    .Property(e => e.CancelledHeader)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestRequisition>()
                    .Property(e => e.CancelledDetail)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestRequisition>()
                    .Property(e => e.IpNo)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestRequisition>()
                    .Property(e => e.BedNumber)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestRequisition>()
                    .Property(e => e.MRNumber)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestRequisition>()
                    .Property(e => e.RequesitionId)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestRequisition>()
                    .Property(e => e.RequesitionNumber)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestRequisition>()
                    .Property(e => e.DepartmentName)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestRequisition>()
                    .Property(e => e.TestId)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestRequisition>()
                    .Property(e => e.GroupId)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestRequisition>()
                    .Property(e => e.GroupName)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestRequisition>()
                    .Property(e => e.DepartmentId)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestRequisition>()
                    .Property(e => e.TestName)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestRequisition>()
                    .Property(e => e.PatientName)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestRequisition>()
                    .Property(e => e.Age)
                    .HasPrecision(7, 2);

                modelBuilder.Entity<StagingTestRequisition>()
                    .Property(e => e.YMD)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestRequisition>()
                    .Property(e => e.Sex)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestRequisition>()
                    .Property(e => e.RCDate)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestRequisition>()
                    .Property(e => e.SADate)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestRequisition>()
                    .Property(e => e.CollectionDate)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestRequisition>()
                    .Property(e => e.PrintDateTime)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestRequisition>()
                    .Property(e => e.ApprovedDateTime)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestRequisition>()
                    .Property(e => e.DoctorName)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestRequisition>()
                    .Property(e => e.IPopDocName)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestMaster>()
                .Property(e => e.TestId)
                .IsUnicode(false);

                modelBuilder.Entity<StagingTestMaster>()
                    .Property(e => e.TestAlias)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestMaster>()
                    .Property(e => e.TestName)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestMaster>()
                    .Property(e => e.SampleId)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestMaster>()
                    .Property(e => e.Sample)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestparameter>()
                    .Property(e => e.TestId)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestparameter>()
                    .Property(e => e.TestAlias)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestparameter>()
                    .Property(e => e.TestName)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestparameter>()
                    .Property(e => e.ParameterCode)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestparameter>()
                    .Property(e => e.Parameter)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestparameter>()
                    .Property(e => e.MethodName)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestparameter>()
                    .Property(e => e.Gender)
                    .IsUnicode(false);               

                modelBuilder.Entity<StagingTestparameter>()
                    .Property(e => e.AgeType)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestparameter>()
                    .Property(e => e.MinValue)
                    .IsUnicode(false);

                modelBuilder.Entity<StagingTestparameter>()
                    .Property(e => e.MaxValue)
                    .IsUnicode(false);

                modelBuilder.Entity<TestResult>()
                .HasMany(e => e.TestResultDetails)
                .WithRequired(e => e.TestResult)
                .WillCascadeOnDelete(false);

                modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
                base.OnModelCreating(modelBuilder);
            }
        }
    }
}
