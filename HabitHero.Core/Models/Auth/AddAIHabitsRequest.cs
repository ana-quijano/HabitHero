using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitHero.Core.Models.Auth
{
    public class AddAIHabitsRequest
    {
        [Required]
        public int IntUserId { get; set; }

        [Required]
        public List<AiHabitItem> Habits { get; set; } = new();
    }

    public class AiHabitItem
    {
        [Required]
        [MaxLength(200)]
        public string StrHabit { get; set; }

        [MaxLength(500)]
        public string StrDescription { get; set; }
    }
}
