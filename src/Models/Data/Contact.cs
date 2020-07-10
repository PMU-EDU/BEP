using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BES.Models.Data
{
    public class Contact
    {
        [Key]
        public short ContactID { get; set; }
        
        [DisplayName("Name")]
        public string Name { get; set; }
        [DisplayName("Contact Number")]
        public string ContactNumber { get; set; }
        [DisplayName("Section")]
        public short SectionID { get; set; }
        [DisplayName("Is SMT Member")]
        public bool IsSMTMember { get; set; }
        [DisplayName("Is Active")]
        public bool IsActive { get; set; }

        public virtual Section Section { get; set; }
    }
}
