using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BES.Areas.LMS.Models
{
    [Table("LeaveTypes", Schema ="HR")]
    public class LeaveType
    {
        [Key]
        public short LeaveTypeID { get; set; }        
        public string Name { get; set; }
        public double Total { get; set; }
    }
}
