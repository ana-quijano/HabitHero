using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitHero.Core.Entities
{
    public class Tschedule
    {
        public int IntScheduleId { get; set; }
        public string StrSchedule { get; set; } = null!;

        // Nav Props
        public ICollection<Thabit> Thabits { get; set; } = new List<Thabit>();
    }
}

