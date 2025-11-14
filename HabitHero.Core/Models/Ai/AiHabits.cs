using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitHero.Core.Models.Ai
{
    
    /// User enters goal for GPT to generate habit suggestions
    public class GenerateHabitsRequest
    {
        public string StrGoal { get; set; } = string.Empty;
    }

    /// Creates single habit suggestion produced by the GPT
    public class HabitSuggestion
    {
        public string StrHabit { get; set; } = "";
        public string StrDescription { get; set; } = "";
    }

    /// Structured payload returned by the AI habit generator.
    public class HabitSuggestionsPayload
    {
        public string Parsed_Goal { get; set; } = "";
        public List<HabitSuggestion> Habits { get; set; } = new();
    }

}
