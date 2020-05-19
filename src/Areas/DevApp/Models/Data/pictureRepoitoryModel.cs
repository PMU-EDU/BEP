using BES.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BES.Areas.DevApp.Models
{
    //public class IndicatorTracking
   //   [Table("IncdicatorTracking", Schema = "Proj")]

    public class pictureRepoitoryModel
    {
        [Key]

        public int pr_id { get; set; }
        public int indicatorID { get; set; }
        public int school_id { get; set; }
        public short picture_count { get; set; }
        public string current_date { get; set; }
      

      //  public List<repositoryDetailList> repositoryDetailList { get; set; }

     //   public virtual School School { get; set; }

    }

    public class repositoryDetailList
    {
        public string current_date { get; set; }
        public string picture_comment { get; set; }
        public double picture_logitude { get; set; }
        public double picture_latitude { get; set; }
        //Image64 
        public string picture_path { get; set; }
        public sbyte[] picture_data { get; set; }
    }

    public class PictureRepository
    {
        public pictureRepoitoryModel pictureRepoitoryModel { get; set; }
        public List<repositoryDetailList> repositoryDetailList { get; set; }
    }

    public class schoolIndicatorPictureRepository
    {
        public List<PictureRepository> schoolIndicatorPictureRepositoryList { get; set; }
    }
}
