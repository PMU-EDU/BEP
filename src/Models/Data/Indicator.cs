using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BES.Models.Data
{
    [Table("Indicator", Schema = "Proj")]
    public class Indicator
    {
        [Key]
        public int IndicatorID { get; set; }
        public short PartnerID { get; set; }
        public string IndicatorName { get; set; }
        public string Description { get; set; }
        public short SequenceNo { get; set; }
        public bool IsEvidenceRequire { get; set; }
        public bool IsPotential { get; set; }
        public bool IsFeeder { get; set; }
        public bool IsNextLevel { get; set; }
        public string EvidanceType { get; set; }
        public bool IsSummary { get; set; }

        public short bifurcationToApp { get; set; }
        public bool IsEvidenceFromApp { get; set; }

        public virtual Partner Partner { get; set; }
    }
}
