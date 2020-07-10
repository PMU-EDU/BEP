using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BES.Models.Reports
{
    [Table("CumulativeRF", Schema = "Proj")]
    public class CumulativeRF
    {
        
        [Key]
        public short IndiID { get; set; }

        [DisplayName("Indicator")]
        public string IndicatorName { get; set; }

        [DisplayName("Target")]
        public int? GPEOrginalTarget { get; set; }
        
        [DisplayName("Revised Target")]
        public int? GPERevisedTarget { get; set; }

        [DisplayName("Achieved")]
        public int? GPERevisedAchieved { get; set; }

        [DisplayName("Unmet Target")]
        public int? GPEUnMetTarget { get; set; }

        [DisplayName("Target")]
        public int? EUTarget { get; set; }

        [DisplayName("Achieved")]
        public int? EUAchieved { get; set; }

        [DisplayName("Target")]
        public int? CumulativeTarget { get; set; }

    }
}
