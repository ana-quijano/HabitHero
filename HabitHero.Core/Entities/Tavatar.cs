using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitHero.Core.Entities
{
    public class Tavatar
    {
        public int IntAvatarId { get; set; }
        public string StrAvatar { get; set; } = null!;

        // Nav Props
        public ICollection<Tuser> Tusers { get; set; } = new List<Tuser>();
        public ICollection<TavatarItem> TavatarItems { get; set; } = new List<TavatarItem>(); // 👈 Add this

    }
}
