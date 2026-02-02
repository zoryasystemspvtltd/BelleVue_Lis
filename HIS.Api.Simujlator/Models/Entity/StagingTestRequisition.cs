namespace HIS.Api.Simujlator.Models.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Staging_TestReq")]
    public partial class StagingTestRequisition
    {
        [StringLength(3)]
        [Column("TYP")]
        public string Type { get; set; }

        [StringLength(9)]
        [Column("CANCELLED_HDR")]
        public string CancelledHeader { get; set; }

        [StringLength(9)]
        [Column("CANCELLED_DTL")]
        public string CancelledDetail { get; set; }

        [StringLength(50)]
        [Column("IPNO")]
        public string IpNo { get; set; }

        [StringLength(50)]
        [Column("BEDNO")]
        public string BedNumber { get; set; }

        [StringLength(40)]
        [Column("MRNO")]
        public string MRNumber { get; set; }

        [Key]
        [Required]
        [Column("REQID", Order = 0)]
        [StringLength(30)]
        public string RequesitionId { get; set; }

        [Required]
        [StringLength(30)]
        [Column("REQNO")]
        public string RequesitionNumber { get; set; }

        [StringLength(42)]
        [Column("DEPTNM")]
        public string DepartmentName { get; set; }

        [Key]
        [Required]
        [Column("TESTID", Order = 1)]
        [StringLength(8)]
        public string TestId { get; set; }

        [StringLength(10)]
        [Column("GROUPID")]
        public string GroupId { get; set; }

        [StringLength(50)]
        [Column("GROUPNM")]
        public string GroupName { get; set; }

        [StringLength(8)]
        [Column("DEPTID")]
        public string DepartmentId { get; set; }

        [StringLength(100)]
        [Column("TESTNM")]
        public string TestName { get; set; }

        [Required]
        [StringLength(100)]
        [Column("PATIENTNM")]
        public string PatientName { get; set; }

        [Column("AGE", TypeName = "numeric")]
        public decimal? Age { get; set; }

        [StringLength(2)]
        [Column("YMD")]
        public string YMD { get; set; }

        [StringLength(3)]
        [Column("SX")]
        public string Sex { get; set; }

        [Required]
        [Column("REQDTTM", TypeName = "datetime2")]
        public DateTime? RequisitionDate { get; set; }

        [StringLength(12)]
        [Column("RCDATE")]
        public string RCDate { get; set; }

        [StringLength(12)]
        [Column("SADATE")]
        public string SADate { get; set; }

        [StringLength(12)]
        [Column("COLDATE")]
        public string CollectionDate { get; set; }

        [Column("COLLTIME", TypeName = "datetime2")]
        public DateTime? CollectionTime { get; set; }

        [Column("PRINTDT", TypeName = "datetime2")]
        public DateTime? PrintDate { get; set; }

        [Column("PRINTTM", TypeName = "datetime2")]
        public DateTime? PrintTime { get; set; }

        [StringLength(22)]
        [Column("PRINTDTTM")]
        public string PrintDateTime { get; set; }

        [Column("APPROVEDDT", TypeName = "datetime2")]
        public DateTime? ApprovedDate { get; set; }

        [Column("APPROVEDTM", TypeName = "datetime2")]
        public DateTime? ApprovedTime { get; set; }

        [StringLength(22)]
        [Column("APPROVEDTTM")]
        public string ApprovedDateTime { get; set; }

        [Column("PERFORMEDDT", TypeName = "datetime2")]
        public DateTime? PerformDate { get; set; }

        [Column("PERFORMEDTM", TypeName = "datetime2")]
        public DateTime? PerformDateTime { get; set; }

        [StringLength(100)]
        [Column("DRNAME")]
        public string DoctorName { get; set; }

        [StringLength(75)]
        [Column("IPOPDOCNM")]
        public string IPopDocName { get; set; }

        [Required]        
        public int Modified { get; set; }
        [Required]
        public int Acknowledged { get; set; }
        [Column("EDCount")]
        public int EDCount { get; set; }
    }
}
