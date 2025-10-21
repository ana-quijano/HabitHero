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
        public virtual Tuser IntUser { get; set; } = null!;
        public virtual Tschedule IntSchedule { get; set; } = null!;

        // Nav Props
        public Tuser? Tuser { get; set; }
        public Tschedule? Tschedule { get; set; }
        public ICollection<ThabitOccurrence> ThabitOccurrences { get; set; } = new List<ThabitOccurrence>();
    }
}
