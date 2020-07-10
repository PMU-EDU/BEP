using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BES.Models.Data
{
    public class Section
    {
        [Key]
        [DisplayName("Section")]
        public short SectionID { get; set; }      
        public string Name { get; set; }
    }
}
