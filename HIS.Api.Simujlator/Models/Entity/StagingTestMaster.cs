namespace HIS.Api.Simujlator.Models.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Staging_TestMaster")]
    public partial class StagingTestMaster
    {
        [Key]
        [StringLength(8)]
        public string TestId { get; set; }

        [Required]
        [StringLength(30)]
        public string TestAlias { get; set; }

        [Required]
        [StringLength(100)]
        public string TestName { get; set; }

        [Required]
        [StringLength(8)]
        public string SampleId { get; set; }

        [Required]
        [StringLength(100)]
        public string Sample { get; set; }
    }
}
