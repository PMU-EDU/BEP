using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BES.Areas.Procurement.Models.ModelViews
{
    public class LotInfo
    {        
        public short ActivityID { get; set; }
        public string StepReferenceNo { get; set; }
        public string lotDescription { get; set; }
        public short ItemTotal { get; set; }
        public int lotId { get; set; }
        [DisplayName("Lot-No")]
        public short lotno { get; set; }  
        public string Attachment { get; set; }
        public short ContractorID { get; set; }
        public string CompanyName { get; set; }   
        [DisplayName("Expiry")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? ExpiryDate { get; set; }      
        public int? ActualCost { get; set; }
        public int? EstimatedCost { get; set; }
        public virtual Lot Lot { get; set; }
        public virtual Activity Activity { get; set; }
        public virtual BES.Models.Data.Contractor Contractor { get; set; }
    }
}
