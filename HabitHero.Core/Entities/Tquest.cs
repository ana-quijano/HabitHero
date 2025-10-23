using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitHero.Core.Entities
{
    public class Tquest
    {
        public int IntQuestId { get; set; }
        public string StrQuestName { get; set; } = null!;
        public decimal? MonMoneyPot { get; set; }
        public decimal? DecPointsPot { get; set; }
        public DateTime? DtmStartDate { get; set; }
        public DateTime? DtmEndDate { get; set; }

        // Nav Props
        public ICollection<TuserQuest> TuserQuests { get; set; } = new List<TuserQuest>();
        public ICollection<TquestHabit> TquestHabits { get; set; } = new List<TquestHabit>();
    }
}
