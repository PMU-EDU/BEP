using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BES.Models.Data
{
    public class Component
    {
        [Key]
        [DisplayName("Component")]
        public short ComponentID { get; set; }
        [DisplayName("Component Code")]
        public string ComponentCode { get; set; }
        [DisplayName("Component Name")]
        public string ComponentName { get; set; }        
    }
}
