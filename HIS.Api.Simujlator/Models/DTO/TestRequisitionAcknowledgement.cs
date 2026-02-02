using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace HIS.Api.Simujlator.Models.DTO
{
    [Bind(Include = "RequesitionId,RequesitionNumber,TestId")]
    public class TestRequisitionAcknowledgement
    {
        [Required]
        [StringLength(30)]
        public string RequesitionId { get; set; }
        [Required]
        [StringLength(30)]
        public string RequesitionNumber { get; set; }
        [Required]
        [StringLength(8)]
        public string TestId { get; set; }
    }
}