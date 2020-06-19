using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BES.Models.Data
{
    [Table("Tool5Detail", Schema = "Proj")]
    public class Tool5Detail
    {
        [Key]
        public int SchoolID { get; set; }
        [Key]
        public short Quarter { get; set; }
        [Key]
        [DisplayName("Class")]
        public short ClassID { get; set; }
        [DisplayName("New Enroll Girls")]
        public short NewEnrolltGirls { get; set; }
        [DisplayName("New Enroll Boys")]
        public short NewEnrollBoys { get; set; }
        [DisplayName("Attend Reg Girls")]
        public short AttendRegGirls { get; set; }
        [DisplayName("Attend Reg Boys")]
        public short AttendRegBoys { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public School School { get; set; }
      //  public SchoolClass SchoolClass { get; set; }
    }
}
