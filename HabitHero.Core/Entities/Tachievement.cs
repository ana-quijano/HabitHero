using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitHero.Core.Entities
{
    public class Tachievement
    {
        public int IntAchievementId { get; set; }
        public string StrAchievement { get; set; } = null!;

        // Nav Props
        public ICollection<TuserAchievement> TuserAchievements { get; set; } = new List<TuserAchievement>();
    }
}
