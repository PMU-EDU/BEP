using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BES.Models.Data
{
    public class ScoreCardMeetingSchedule
    {
        [Key]
        public short ScoreCardMeetingScheduleID { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("Last Meeting Attend On")]
        public DateTime LastMeetingDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("Date Of Reminder")]
        public DateTime DateOfReminderStart { get; set; }
        public short Frequency { get; set; }
        public short ReminderSofar { get; set; }
        public DateTime CurrentDate { get; set; }
    }
}
