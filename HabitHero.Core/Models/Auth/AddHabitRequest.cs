using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitHero.Core.Models.Auth
{
    public class AddHabitRequest
    {
        public int IntUserId { get; set; }
        public string StrHabit { get; set; } = null!;
        public string StrDescription { get; set; } = null!;
    }
}
