using BES.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BES.Areas.Procurement.Models
{
    [Table("AddendumWorkDetail", Schema ="Proc")]
    public class AddendumWorkDetail
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short AddendumWorkDetailId { get; set; }                
        public int Amount { get; set; }
        public Int64 ActualCost { get; set; }
        [Column(TypeName ="varchar(1)")]
        public string Sign { get; set; }

        [ForeignKey("AddendumId")]
        public short AddendumId { get; set; }
        public virtual AddendumWorks AddendumWorks { get; set; }
        [ForeignKey("SchoolId")]
        public int SchoolId { get; set; }
        public virtual School School { get; set; }
    }
}
