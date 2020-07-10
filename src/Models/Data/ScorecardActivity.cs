using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BES.Models.Data
{
    public class ScorecardActivity
    {
        [Key]
        public short ScorecardActivityID { get; set; }
        public short ActivityNo { get; set; }
        [DisplayName("Component")]
        [ForeignKey("SectionID")]
        public short ComponentID { get; set; }
        [DisplayName("Activity Name")]
        public string ActivityName { get; set; }
        public string SerialNo { get; set; }
        public string Target { get; set; }
        public string Achived { get; set; }
        public string Responsibility { get; set; }
        [ForeignKey("SectionID")]
        public short SectionID { get; set; }
        [DisplayName("Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DisplayName("End Date")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
        public string Comments { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("Baseline Start Date")]
        public DateTime? BaselineStartDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("Baseline End Date")]
        public DateTime? BaselineEndDate { get; set; }
        public short? Duration { get; set; }
        public short? Progress { get; set; }
        public short? Predecessor { get; set; }
        [DisplayName("Parent")]
        public short? ParentID { get; set; }
        public bool IsHead { get; set; }        
        public short Status { get; set; }
        public bool? IsUpdated { get; set; }
        public string meetingStatusList { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem> ParentNode { get; set; }

        public virtual Section Section { get; set; }
        public virtual Component Component { get; set; }
    }
}
