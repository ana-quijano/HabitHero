using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitHero.Core.Entities
{
    public class TquestHabit
    {
        public int IntQuestHabitId { get; set; }
        public int IntQuestId { get; set; }
        public int IntScheduleId { get; set; }
        public string StrHabitName { get; set; } = null!;
        public TimeSpan? DtmReminderTime { get; set; }
        public DateTime? DtmStartDate { get; set; }
        public DateTime? DtmEndDate { get; set; }
        public string? StrDescription { get; set; }

        // Nav Props
        public Tquest? Tquest { get; set; }
        public Tschedule? Tschedule { get; set; }
        public ICollection<ThabitOccurrence> ThabitOccurrences { get; set; } = new List<ThabitOccurrence>();
    }
}
