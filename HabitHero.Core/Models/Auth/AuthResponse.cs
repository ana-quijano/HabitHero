using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitHero.Core.Models.Auth
{
    public class AuthResponse
    {
        public string StrAccessToken { get; set; } = null!;
        public DateTime DtmExpiresAt { get; set; }
        public object ObjUser { get; set; } = null!;
    }
}
