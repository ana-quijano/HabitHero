using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitHero.Core.Entities
{
    public class ThabitGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IntGroupID { get; set; }

        [Required]
        [MaxLength(100)]
        public string StrGroupName { get; set; }

        [MaxLength(255)]
        public string? StrDescription { get; set; }

        public DateTime DtmCreatedAt { get; set; } = DateTime.UtcNow;
    }
}
