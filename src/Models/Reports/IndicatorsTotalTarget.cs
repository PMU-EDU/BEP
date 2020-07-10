using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BES.Models.Reports
{
    public class IndicatorsTotalTarget
    {
        [Key]
        public int Potential { get; set; }
     public int Feeder { get; set; }
     public int NextLevel       {get;set;}
     public int PotentialNew    {get;set;}
     public int FeederNew       {get;set;}
     public int NextLevelNew    {get;set;}
     public int PotentialRepair { get;set;}
     public int FeederRepair  {get;set;}
     public int NextLevelRepair {get;set;}
     public int TotalNew { get;set;}
     public int TotalRepair { get;set;}
     public int TotalTarget { get;set; }

        public int PotentialDev { get; set; }
        public int FeederDev { get; set; }
        public int NextLevelDev { get; set; }
    }
}
