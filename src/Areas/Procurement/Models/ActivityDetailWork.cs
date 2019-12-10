using BES.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BES.Areas.Procurement.Models
{
    [Table("ActivityDetailWork", Schema = "Proc")]
    public class ActivityDetailWork
    {
        [Key]
        public short ActivityDetailWorkID { get; set; }
        [DisplayName("Activity")]
        public short ActivityID { get; set; }
        [DisplayName("Total School")]
        public short TotalSchool { get; set; }
        [DisplayName("Contractor")]
        public short? ContractorID { get; set; }
        [DisplayName("Expiry Date")]
        public DateTime? ExpiryDate { get; set; }      

        public virtual Activity Activity { get; set; }
        public virtual Contractor Contractor { get; set; }
    }
}
