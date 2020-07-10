using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BES.Models.Data
{
    [Table("IndicatorDevApp", Schema = "Proj")]
    public class IndicatorDevApp
    {
        [Key]
        public short ImageID { get; set; }
        [Key]
        public int SchoolID { get; set; }
        [Key]
        public int IndicatorID { get; set; }
        public string ImagePath { get; set; }
        public string ImageByte { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime SyncDate { get; set; }
        public string Remarks { get; set; }
        public bool VerifyRE { get; set; }
        public DateTime? VerifyREDate { get; set; }
        public string VerifyREBy { get; set; }
        public bool? VerifySDE { get; set; }
        public DateTime? VerifySDEDate { get; set; }
        public string VerifySDEBy { get; set; }

    }
}
