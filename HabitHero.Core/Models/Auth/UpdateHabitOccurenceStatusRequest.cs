using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitHero.Core.Models.Auth
{
    public class UpdateOccurrenceStatusRequest
    {
        public int IntUserId { get; set; }
        public int IntHabitId { get; set; }
        public int IntHabitOccurrenceId { get; set; }
        public int IntStatusId { get; set; } // 1 = To Do, 2 = Done
    }
}
