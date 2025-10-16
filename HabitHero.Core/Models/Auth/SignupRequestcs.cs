using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitHero.Core.Models.Auth
{
    public class SignupRequest
    {
        public string StrUsername { get; set; } = null!;
        public string StrEmail { get; set; } = null!;
        public string StrPassword { get; set; } = null!;
        public string StrConfirmPassword { get; set; } = null!;
    }
}
