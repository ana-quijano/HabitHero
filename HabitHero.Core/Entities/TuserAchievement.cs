using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitHero.Core.Entities
{
    public class TuserAchievement
    {
        public int IntUserAchievementId { get; set; }
        public int IntUserId { get; set; }
        public int IntAchievementId { get; set; }
        public virtual Tuser IntUser { get; set; } = null!;
        public virtual Tachievement IntAchievement { get; set; } = null!;

        // Nav Props
        public Tuser? Tuser { get; set; }
        public Tachievement? Tachievement { get; set; }
    }
}
