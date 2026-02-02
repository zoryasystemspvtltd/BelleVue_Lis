namespace HIS.Api.Simujlator.Models.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TestResults")]
    public partial class TestResult
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TestResult()
        {
            TestResultDetails = new HashSet<TestResultDetail>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public long Id { get; set; }

        [StringLength(100)]
        public string SampleNo { get; set; }

        [StringLength(100)]
        public string HISTestCode { get; set; }

        [StringLength(100)]
        public string LISTestCode { get; set; }

        [StringLength(100)]
        public string SpecimenCode { get; set; }

        [StringLength(255)]
        public string SpecimenName { get; set; }

        public DateTime ResultDate { get; set; }

        public DateTime SampleCollectionDate { get; set; }

        public DateTime SampleReceivedDate { get; set; }

        public DateTime? AuthorizationDate { get; set; }

        [StringLength(100)]
        public string AuthorizedBy { get; set; }

        public DateTime? ReviewDate { get; set; }

        [StringLength(100)]
        public string ReviewedBy { get; set; }

        [StringLength(1000)]
        public string TechnicianNote { get; set; }

        [StringLength(1000)]
        public string DoctorNote { get; set; }

        [StringLength(100)]
        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public long PatientId { get; set; }

        public long TestRequestId { get; set; }

        public int EquipmentId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TestResultDetail> TestResultDetails { get; set; }
    }
}
