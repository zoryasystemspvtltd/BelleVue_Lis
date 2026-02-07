namespace HIS.Api.Simujlator.Models.Entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Staging_Testparameter")]
    public partial class StagingTestparameter
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(8)]
        public string TestId { get; set; }

        [Required]
        [StringLength(15)]
        public string TestAlias { get; set; }

        [Required]
        [StringLength(100)]
        public string TestName { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(15)]
        public string ParameterCode { get; set; }

        [Required]
        [StringLength(150)]
        public string Parameter { get; set; }

        [Required]
        [StringLength(200)]
        public string MethodName { get; set; }

        [Required]
        [StringLength(150)]
        public string Gender { get; set; }

        [Required]

        public decimal? AgeFrom { get; set; }

        [Required]
        public decimal? AgeTo { get; set; }

        [Required]
        [StringLength(15)]
        public string AgeType { get; set; }

        [Required]
        [StringLength(15)]
        public string MinValue { get; set; }

        [Required]
        [StringLength(20)]
        public string MaxValue { get; set; }
    }
}
