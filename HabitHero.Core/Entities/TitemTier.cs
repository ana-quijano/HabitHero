using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitHero.Core.Entities
{
    public class TitemTier
    {
        public int IntItemTierId { get; set; }
        public string StrItemTier { get; set; } = null!;

        // Nav Props
        public ICollection<Titem> Titems { get; set; } = new List<Titem>();
    }
}
