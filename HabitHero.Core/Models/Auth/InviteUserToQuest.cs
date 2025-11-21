using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitHero.Core.Models.Auth
{
    public class InviteUserToQuest
    {
        public int IntQuestId { get; set; }
        public string StrUserName { get; set; } = null!;
    }
}
