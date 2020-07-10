using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BES.Areas.Procurement.Models
{
    [Table("AddendumWorks", Schema = "Proc")]
    public class AddendumWorks
    {
        [Key]
        public short AddendumId { get; set; }
        public short ActivityDetailWorkID { get; set; }
        [DisplayName("Addendum Type")]
        public short AddendumTypeId { get; set; }
        [DisplayName("Contract Agreement")]
        public string Attachment { get; set; }

        [DisplayName("Remarks(If Any)")]
        public string Remarks { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Expiry Date")]
        public DateTime? ExpiryDate { get; set; }        
        public DateTime CurrentDate { get; set; }
        [DisplayName("Actual Amount")]
        public Int64? ActualAmount { get; set; }
        public virtual ActivityDetailWork ActivityDetailWork { get; set; }
        public virtual AddendumType AddendumType { get; set; }
    }
}
