namespace HIS.Api.Simujlator.Models.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TestResultDetails")]
    public partial class TestResultDetail
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public long Id { get; set; }

        [StringLength(100)]
        public string LISParamCode { get; set; }

        [StringLength(100)]
        public string LISParamValue { get; set; }

        [StringLength(200)]
        public string LISParamUnit { get; set; }

        [StringLength(100)]
        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public long TestResultId { get; set; }

        public virtual TestResult TestResult { get; set; }
    }
}
