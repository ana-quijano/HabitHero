using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitHero.Core.Models.Auth
{
    public class LoginRequest
    {
        public string StrUsername { get; set; } = null!;
        public string StrPassword { get; set; } = null!;
    }
}
