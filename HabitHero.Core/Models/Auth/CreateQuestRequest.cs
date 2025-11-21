using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitHero.Core.Models.Auth
{
    public class CreateQuestRequest
    {
        public int IntUserId { get; set; }
        public string StrQuestName { get; set; } = null!;
        public decimal DecPointsPot { get; set; }
    }
}
