using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace HabitHero.Core.Entities
{
    public class ThabitOccurrence
    {
        public int IntHabitOccurrenceId { get; set; }
        public int IntHabitId { get; set; }
        public int IntQuestHabitId { get; set; }
        public DateTime? DtmCompleted { get; set; }
        public int IntStatusId { get; set; }
        public virtual Thabit IntHabit { get; set; } = null!;
        public virtual TquestHabit IntQuestHabit { get; set; } = null!;
        public virtual Tstatus IntStatus { get; set; } = null!;

        // Nav Props
        public Thabit? Thabit { get; set; }
        public TquestHabit? TquestHabit { get; set; }
        public Tstatus? Tstatus { get; set; }
    }
}
