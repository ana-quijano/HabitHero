using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitHero.Core.Entities
{
    public class TappRestriction
    {
        public int IntAppRestrictionId { get; set; }
        public string StrAppName { get; set; } = null!;
        public bool BlnRestricted { get; set; }

        // Nav Props
        public ICollection<Tuser> Tusers { get; set; } = new List<Tuser>();
    }
}
