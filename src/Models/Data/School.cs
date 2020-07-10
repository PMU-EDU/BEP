using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BES.Models.Data
{
    public class School
    {
        [Key]
        public int SchoolID { get; set; }

        [ForeignKey("UC")]
        [Required]
        [DisplayName("UC Name")]
        public int UCID { get; set; }

        [Required]
        [DisplayName("School Name")]
        public string SName { get; set; }

        public short ProjectID { get; set; }

        //[DisplayName("School Code")]
        //public string SCode { get; set; }
        public string BEMIS { get; set; }
        public Int16 SLevel { get; set; }

        //[DataType(DataType.Custom, ErrorMessage = "Not a number")]
        // [RegularExpression(@"^[0-9]*(\.[0-9]{1,2})?$", ErrorMessage = " Latitude Decimal degits should be at Least 5")]
        //[Required(ErrorMessage = "{0} is required")]
        //[Range(25.000000, 33.000000, ErrorMessage = "Please use values between 25 to 33")]
        public string Latitude { get; set; }

        // [RegularExpression(@"^\d+\.\d{1,9}$", ErrorMessage = "Longitude Decimal degits should be at Least 5")]
        //[Required(ErrorMessage = "{0} is required")]
        //[Range(60.000000, 71.000000, ErrorMessage = "Please use values between 60 to 71")]
        public string Longitude { get; set; }
        // public bool Status { get; set; }

        [DisplayName("Zone")]
        public bool? Zone { get; set; }
        // public bool? Onboard { get; set; }
        //public DateTime? DateOpen { get; set; }

        public bool Abandon { get; set; }

        public string Upgradation { get; set; }
        //public DateTime? ConstructionStartDate { get; set; }
        public int? Budget { get; set; }
        public short? type { get; set; }
        public int ClusterBEMIS { get; set; }
        //public bool? PTSMC { get; set; }
        //public bool? LandMutation { get; set; }
        // public int? InitialEnrollment { get; set; }
        //public string Construction { get; set; }
        //public string Password { get; set; }

        [DisplayName("Selected Status")]
        public bool? SelectedStatus { get; set; }

        [DisplayName("Dropped Remarks")]
        public string Remarks { get; set; }

       public int EstimatedCost { get; set; }
       public int ActucalCost         {get;set;}
       public short NewRooms            {get;set;}
       public short RepairRooms         {get;set;}
       public short NewToilets          {get;set;}
       public short RepairToilets       {get;set;}
       public bool NewConstruction     {get;set;}
       public bool RepairRennovation   {get;set;}
       public short? CurrentStage   {get;set;}
       public short? RepairRennovationStatus { get;set;}
       public bool ExternalDevelopment {get;set;}

        //[DisplayName("Notification Date")]
        // [DataType(DataType.Date)]
        // [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        // public DateTime? NotificationDate { get; set; }
        // public string Phase { get; set; }
        [NotMapped]
        public string DisName { get; set; }
        [NotMapped]
        public string RegName { get; set; }

        public virtual UC UC { get; set; }
    }

    public class SchoolDevIndicator
    {
        [Key]
        public int SchoolID { get; set; }
        public short NewRooms { get; set; }
        public short RepairRooms { get; set; }
        public short NewToilets { get; set; }
        public short RepairToilets { get; set; }
    }
    public class SchoolMap
    {
        [Key]
        public int SchoolID { get; set; }

        public string UC { get; set; }
        public string Tehsil { get; set; }
        public string District { get; set; }
        public int Region { get; set; }

        public string School { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public Int16 Level { get; set; }
        public bool Abandon { get; set; }


    }



    public class Api2School
    {


        public Api2School(int schoolid, int ucID, string sname, int? bemis, Int16 level, string password, string latitude, string longitude)
        {
            this.SchoolID = schoolid;
            this.UcID = ucID;
            this.SName = sname;
            this.BEMIS = bemis;
            this.Level = level;
            this.Password = password;
            this.Latitude = latitude;
            this.Longitude = longitude;
        }




        public int SchoolID { get; set; }

        public int UcID { get; set; }


        public string SName { get; set; }


        public int? BEMIS { get; set; }

        public Int16 Level { get; set; }

        public string Password { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }
    }

    public class SchIndicatorStatus
    {
        public int RegionID { get; set; }
        public string DistrictName { get; set; }
                 [Key]
         public int SchoolID { get; set; }
        public string SName { get; set; }
         public int ClusterBEMIS { get; set; }
        public string BEMIS { get; set; }
        public string SLevel { get; set; }
        public string Type { get; set; }
        public int IsUploaded { get; set; }
    }
}