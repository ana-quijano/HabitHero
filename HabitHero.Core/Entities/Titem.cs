using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitHero.Core.Entities
{
    public class Titem
    {
        public int IntItemId { get; set; }
        public string StrItem { get; set; } = null!;
        public int IntItemTypeId { get; set; }
        public int IntItemTierId { get; set; }

        // Nav Props
        public TitemType? TitemType { get; set; }
        public TitemTier? TitemTier { get; set; }
    }
}
