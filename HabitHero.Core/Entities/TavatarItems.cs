using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

namespace HabitHero.Core.Entities
{
    public class TavatarItem
    {
        public int IntAvatarItemId { get; set; }
        public int IntAvatarId { get; set; }
        public int IntItemId { get; set; }
        public int IntQuantity { get; set; }

        // Navigation Properties
        public Tavatar Tavatar { get; set; } = null!;
        public Titem Titem { get; set; } = null!;
    }
}
