using BES.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BES.Areas.LMS.Models
{
    [Table("Employees", Schema = "HR")]
    public class Employee
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeID { get; set; }
        [DisplayName("Employee Code")]
        public string EmpCode { get; set; }
        [DisplayName("Full Name")]
        public string Name { get; set; }
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public string Designation { get; set; }
        public string Gender { get; set; }
        [Display(Name = "Mobile Number:")]
        [Required(ErrorMessage = "Mobile Number is required.")]
        [RegularExpression(@"^([0-9]{12})$", ErrorMessage = "Invalid Mobile Number.")]
        public string ContactNo { get; set; }
        [DisplayName("Supervisor/Section Head")]
        public int SupervisorID { get; set; }
        public bool IsSectionHead { get; set; }
        [DisplayName("Joining Date")]
        [DataType(DataType.Date)]
        public DateTime? JoiningDate { get; set; }
        [DisplayName("Contract Start Date")]
        [DataType(DataType.Date)]
        public DateTime? ContractStartDate { get; set; }
        [DisplayName("Contract End Date")]
        [DataType(DataType.Date)]
        public DateTime? ContractEndDate { get; set; }

        [DisplayName("SectionID")]
        [ForeignKey("SectionID")]
        public short SectionID { get; set; }
        public virtual Section Section { get; set; }
    }
}
