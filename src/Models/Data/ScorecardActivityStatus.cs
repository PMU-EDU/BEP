using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BES.Models.Data
{
    public class ScorecardActivityStatus
    {
        [Key]
        public int ScorecardActivityStatusID { get; set; }
        public short ScoreCardMeetingScheduleID { get; set; }
        public short ScorecardActivityID { get; set; }
        public short CurrentStatus { get; set; }
        public string Comments { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CurrentDate { get; set; }

        public virtual ScoreCardMeetingSchedule ScoreCardMeetingSchedule { get; set; }
        public virtual ScorecardActivity ScorecardActivity { get; set; }
    }
}
