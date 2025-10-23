using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitHero.Core.Entities
{
    public class TitemType
    {
        public int IntItemTypeId { get; set; }
        public string StrItemType { get; set; } = null!;

        // Nav Props
        public ICollection<Titem> Titems { get; set; } = new List<Titem>();
    }
}
