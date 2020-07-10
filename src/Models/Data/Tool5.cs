using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BES.Models.Data
{

    [Table("Tool5", Schema = "Proj")]

    public class Tool5
    {
        [Key]
        [DisplayName("School ID")]
        public int SchoolID { get; set; }
        [Key]
        public short Quarter { get; set; }
        public short Year { get; set; }
        public short ProjectYear { get; set; }

        [DisplayName("Data Collection Date")]
        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [DisplayName("Data Collector Name")]
        public string VisitorName { get; set; }
        public bool ReCollectData { get; set; }
        public bool Verified { get; set; }
        public string VerifiedBy { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual School School { get; set; }
    }
}
