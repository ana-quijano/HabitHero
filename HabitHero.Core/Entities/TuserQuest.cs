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

        public Tuser? Tuser { get; set; }
        public Tquest? Tquest { get; set; }
    }
}
