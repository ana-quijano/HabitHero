using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitHero.Core.Entities
{
    public class Thabit
    {
        public int IntHabitId { get; set; }
        public int IntUserId { get; set; }
        public int IntScheduleId { get; set; }
        public string StrHabit { get; set; } = null!;
        public string? StrDescription { get; set; }
        public DateTime? DtmStartDate { get; set; }
        public DateTime? DtmEndDate { get; set; }
        public TimeSpan? DtmReminderTime { get; set; }

        // Nav Props
        public Tuser IntUser { get; set; } = null!;
        public Tschedule IntSchedule { get; set; } = null!;
    }
}
