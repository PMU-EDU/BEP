using BES.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BES.Areas.Procurement.Models
{
    [Table("WorkSchool", Schema = "Proc")]
    public class WorkSchool
    {
        [Key]
        public short WorkSchoolID { get; set; }
        public short ActivityDetailWorkID { get; set; }
        public int SchoolID { get; set; }
        [DisplayName("Estimated Cost")]
        public Int64? EstimatedCost { get; set; }
        [DisplayName("Actual Cost")]
        public Int64? ActualCost { get; set; }
        public DateTime CurrentDate { get; set; }

        public virtual ActivityDetailWork ActivityDetailWork { get; set; }
        public virtual School School { get; set; }
    }
}
