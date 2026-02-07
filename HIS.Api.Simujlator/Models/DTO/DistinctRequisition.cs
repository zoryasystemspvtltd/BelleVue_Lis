using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HIS.Api.Simujlator.Models.DTO
{
    public class DistinctRequisition
    {
        [Required]
        [StringLength(30)]
        public string RequesitionId { get; set; }
        [Required]
        [StringLength(30)]
        public string RequesitionNumber { get; set; }
    }
}