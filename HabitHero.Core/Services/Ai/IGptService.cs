using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitHero.Core.Services.Ai
{
    using System.Threading;
    using System.Threading.Tasks;
    using HabitHero.Core.Models.Ai;

    public interface IGptService
    {
        Task<HabitSuggestionsPayload> GenerateHabitsAsync(string goal, CancellationToken ct = default);
    }
}
