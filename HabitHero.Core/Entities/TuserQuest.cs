using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitHero.Core.Entities
{
    public class TuserQuest
    {
        public int IntUserQuestId { get; set; }
        public int IntUserId { get; set; }
        public int IntQuestId { get; set; }

        // Nav Props
        public Tuser IntUser { get; set; } = null!;
        public Tquest IntQuest { get; set; } = null!;
    }
}
