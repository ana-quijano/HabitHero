using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitHero.Core.Models.Auth
{
    public class AddQuestHabitRequest
    {
        public int IntQuestId { get; set; }
        public string StrHabitName { get; set; } = null!;
        public string StrDescription { get; set; } = null!;
    }
}
