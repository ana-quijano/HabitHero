using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitHero.Core.Models.Auth
{
    public class DeleteHabitRequest
    {
        public int IntUserId { get; set; }
        public int IntHabitId { get; set; }
    }

}
