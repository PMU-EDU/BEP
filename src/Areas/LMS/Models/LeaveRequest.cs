using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BES.Areas.LMS.Models
{
    [Table("LeaveRequests",Schema ="HR")]
    public class LeaveRequest
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LeaveRequestID { get; set; }
        [DisplayName("Date From")]
        [DataType(DataType.Date)]
        public DateTime DateFrom { get; set; }
        [DisplayName("Date To")]
        [DataType(DataType.Date)]        
        public DateTime DateTo { get; set; }
        [DisplayName("Days")]
        public short TotalDays { get; set; }
        [DisplayName("Remarks")]
        public string Remarks { get; set; }

        [DisplayName("Clearence By HR")]
        public short ApprovedByHR { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ApprovedByHRDate { get; set; }
        [DisplayName("Remarks(if any)")]
        public string HRRemarks { get; set; }        

        [DisplayName("Approved By Supervisor")]
        public short ApprovedBySection { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ApprovedBySectionDate { get; set; }
        [DisplayName("Remarks(if any)")]
        public string SupervisorRemarks { get; set; }
        [DisplayName("Approved By PD")]
        public short ApprovedByPD { get; set; }
        [DataType(DataType.Date)]        
        public DateTime? ApprovedByPDDate { get; set; }
        [DisplayName("Remarks(if any)")]
        public string PDRemarks { get; set; }

        [DisplayName("Nomination")]
        public string Nomination { get; set; }        
        public int? NominatedID { get; set; }
        [DisplayName("On Behalf Of")]
        public string OnBehalfOf { get; set; }
        public bool IsMedicalCertificateRequired { get; set; }
        public string MedicalCertificatePath { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("Applied On")]
        public DateTime AppliedDate { get; set; }

        [ForeignKey("EmployeeID")]
        public int EmployeeID { get; set; }
        public virtual Employee Employee { get; set; }
        [ForeignKey("LeaveTypeID")]
        [DisplayName("Leave")]
        public short LeaveTypeID { get; set; }
        public virtual LeaveType LeaveType { get; set; }
    }
}
