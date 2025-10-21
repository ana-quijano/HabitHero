using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitHero.Core.Entities
{
    public class Tuser
    {
        public int IntUserId { get; set; }                 
        public string StrUsername { get; set; } = null!;   
        public string StrEmail { get; set; } = null!;      
        public string StrPassword { get; set; } = null!;
        public decimal DecPoints { get; set; }             
        public decimal MonCash { get; set; }               
        public bool BlnAppRestriction { get; set; }        
        public int? IntAvatarId { get; set; }

        // Nav Props
        public Tavatar? Tavatar { get; set; }
        public TappRestriction? TappRestriction { get; set; }
        public ICollection<Thabit> Thabits { get; set; } = new List<Thabit>();
        public ICollection<TuserQuest> TuserQuests { get; set; } = new List<TuserQuest>();
        public ICollection<TuserAchievement> TuserAchievements { get; set; } = new List<TuserAchievement>();
    }
}
