using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitHero.Core.Models.Auth
{
    public class RegisterPushTokenRequest
    {
        public int IntUserId { get; set; }
        public string StrPushToken { get; set; } = null!;
    }

}
