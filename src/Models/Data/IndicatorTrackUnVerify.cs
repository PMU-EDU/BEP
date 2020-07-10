using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BES.Models.Data
{
    [Table("IndicatorTrackUnVerify", Schema = "Proj")]
    public class IndicatorTrackUnVerify
    {
        [Key]
        public int Id { get; set; }
        public int SchoolID { get; set; }
        public int IndicatorID { get; set; }
        public DateTime Datetime { get; set; }
        public string MachineName { get; set; }
        public string MacAddress { get; set; }
    }
}
