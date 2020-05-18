using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BES.Models.Data
{
    public class ESSChecklist
    {

        [Key]
        public Int16 ESMPSitingID { get; set; }

        [Required]
        [DisplayName("School ID")]
        public int SchoolID { get; set; }


        [DisplayName("Land Mutation")]
        public string a1Landmutation { get; set; }

        [DisplayName("Soil Erosion")]
        public bool a2IsSoilErosion { get; set; }

        [DisplayName("Flood Path")]
        public bool a3IsFloodPath { get; set; }

        [DisplayName("Sline Land")]
        public bool a4IsSalineLand { get; set; }

        [DisplayName("Close to Cluster of Trees ")]
        public bool b1ClusterTrees { get; set; }

        [DisplayName("How Many Trees")]
        public int? b2HowManyTrees { get; set; }

        [DisplayName("500 feet away from the Highway")]
        public bool c1IsHighway { get; set; }

        [DisplayName("Transmission Line Passing ")]
        public bool c2IsTransmissionLine { get; set; }

        [DisplayName("Type of TransmissionLine")]
        public string c3TypeTransmissionLine { get; set; }

        [DisplayName("Height of TransmissionLine")]
        public int c4HeightTransmissionLine { get; set; }

        [DisplayName("Electricity Pole In Permises")]
        public bool c5ElectricityPolPermises { get; set; }

        [DisplayName("Type of Pole ")]
        public string c6TypeOfPole { get; set; }


        [DisplayName("Function Handwash")]
        public bool c7FunctionHandwash { get; set; }

        [DisplayName("Washing Repairs")]
        public string c8WashingRepairs { get; set; }

        [DisplayName("Water source available in School Premises")]
        public bool d1WaterSchoolPremises { get; set; }

        [DisplayName("Water Source Type")]
        public string d2WaterSourceType { get; set; }

        [DisplayName("Water Source Vicinity")]
        public bool d3WaterSourceVicinity { get; set; }

        [DisplayName("Type Water Vicinity")]
        public string d4TypeWaterVicinity { get; set; }

        [DisplayName("Distance Water Source")]
        public int d5DistanceWaterSource { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Date")]
        public DateTime d5Date { get; set; }

        [DisplayName("Verified")]
        public bool Verified { get; set; }

        [DisplayName("Verifiedby")]
        public string Verifiedby { get; set; }

        public virtual School School { get; set; }
    }
}
