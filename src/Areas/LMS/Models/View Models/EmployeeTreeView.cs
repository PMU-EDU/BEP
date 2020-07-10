using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BES.Areas.LMS.Models.View_Models
{
    public class EmployeeTreeView
    {
        public short SectionID { get; set; }
        public string Name { get; set; }
        public List<Employee> EmployeeList { get; set; }
    }
}
