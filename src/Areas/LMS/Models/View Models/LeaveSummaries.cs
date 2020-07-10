using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BES.Areas.LMS.Models.View_Models
{
    [Table("LeaveSummaries", Schema = "HR")]
    public class LeaveSummaries
    {
        [Key]
        public int EmployeeID { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Gender { get; set; }
        public string Section { get; set; }
        [DisplayName("A.Total")]
        public int AnnualTotal { get; set; }
        [DisplayName("A.Availed")]
        public int AnnualAvailed { get; set; }
        [DisplayName("S.Total")]
        public int SickTotal { get; set; }
        [DisplayName("S.Availed")]
        public int SickAvailed { get; set; }
        [DisplayName("P.Total")]
        public int PaternityTotal { get; set; }
        [DisplayName("P.Availed")]
        public int PaternityAvailed { get; set; }
        [DisplayName("O.Total")]
        public int OthersTotal { get; set; }
        [DisplayName("O.Availed")]
        public int OthersAvailed { get; set; }                       
    }
}
