using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitHero.Core.Entities
{
    public class Tstatus
    {
        public int IntStatusId { get; set; }
        public string StrStatus { get; set; } = null!;

        // Nav Props
        public ICollection<ThabitOccurrence> ThabitOccurrences { get; set; } = new List<ThabitOccurrence>();
    }
}
