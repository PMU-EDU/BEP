using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BES.Models.Reports
{
    public class IndicatorsSummary
    {
        [Key]
        public int IndicatorID { get; set; }
        public short? PartnerID { get; set; }
        public string IndicatorName { get; set; }

        public int PotentailTarget { get; set; }
        public int? PotentailAchieve { get; set; }
        //public float PotentailPercent { get; set; }

        public int FeederTarget { get; set; }
        public int? FeederAchieve { get; set; }
      //  public int FeederPercent { get; set; }

        public int NLTarget { get; set; }
        public int? NLAchieve { get; set; }
       // public int NLPercent { get; set; }

        public int TotalTarget { get; set; }
        public int? TotalAchieve { get; set; }
        //public int TotalPercent { get; set; }
       // public short Type { get; set; }
    }
}
