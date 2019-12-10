using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BES.Areas.Procurement.Models
{
    [Table("Activity", Schema = "Proc")]
    public class Activity
    {
        [Key]
        public short ActivityID { get; set; }

        [DisplayName("Procurement Head")]
        public short ProcurementPlanID { get; set; }
        [Required]
        [DisplayName("Step Reference No")]
        public string StepReferenceNo { get; set; }
        [DisplayName("Activity Name")]
        public string Name { get; set; }  
        [DisplayName("Lots Required")]
        public short  LotTotal { get; set; }
        [DisplayName("Total Schools")]
        public short SchoolTotal { get; set; }
        [DisplayName("Procurement For")]
        public short ProcurementFor { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [DisplayName("Estimated Cost")]
        public int? EstimatedCost { get; set; }
        [DisplayName("Actual Cost")]
        public int? ActualCost { get; set; }

        [DisplayName("Method Type")]
        public short MethodID { get; set; }

        [DisplayName("Type")]
        public string ReviewType { get; set; }
        public short Status { get; set; }

        [DisplayName("Cancelled")]
        public bool IsCenceled { get; set; }
        public string Remarks { get; set; }

        [ScaffoldColumn(false)]
        [DisplayName("Created By")]
        public string CreatedBy { get; set; }
        [ScaffoldColumn(false)]
        public DateTime CreatedDate { get; set; }
        [ScaffoldColumn(false)]
        [DisplayName("Updated By")]
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public short ProjectNo { get; set; }
        public virtual ProcurementPlan PProcurementPlan { get; set; }
        public virtual Method Method { get; set; }
        public virtual Project PPProject { get; set; }
    }
}