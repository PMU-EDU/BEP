using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BES.Models.Data
{
    [Table("IncdicatorTracking", Schema = "Proj")]

    public class IndicatorTracking
    {
        [Key]
        public int IndicatorID { get; set; }
        [Key]
        public int? SchoolID { get; set; }
        public string ImageURL { get; set; }
        public bool? IsUpload { get; set; }
        public short? TotalFilesUploaded { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date of Completion")]
        public DateTime? DateOfUpload { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? ReUpload { get; set; }

        public bool DevVerified { get; set; }
        public string DevVerifiedBy { get; set; }
        public DateTime? DevVerifiedDate { get; set; }

        public bool? Verified { get; set; }
        public string VerifiedBy { get; set; }
        public DateTime? VerifiedDate { get; set; }

        public bool ReVerified { get; set; }
        public string ReVerifiedBy { get; set; }
        public DateTime? ReVerifiedDate { get; set; }

        [NotMapped]
        public string Indicator { get; set; }
        [NotMapped]
        public bool? isEvidence { get; set; }
        [NotMapped]
        public string EvidanceType { get; set; }
        [NotMapped]
        public bool? isPotential { get; set; }
        [NotMapped]
        public bool? isFeeder { get; set; }
        [NotMapped]
        public bool? isNextLevel { get; set; }


        public virtual School School { get; set; }

    }
}
