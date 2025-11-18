using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HabitHero.Core.Entities;
using HabitHero.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HabitHero.Api.Controllers {
	[ApiController]
	[Route("api/streaks")]
	public class StreaksController : ControllerBase 
		{
		private readonly HabitHeroDbContext _db;

		public StreaksController(HabitHeroDbContext db) => _db = db;

		/// <summary>
		/// Gets current streaks and completion counts for all habits for a user.
		/// </summary>
		[HttpGet("{userId:int}")]
		public async Task<IActionResult> GetUserStreaks([FromRoute] int userId) 
			
		{
			// 1) Make sure the user exists
			var userExists = await _db.Tusers
				.AsNoTracking()
				.AnyAsync(u => u.IntUserId == userId);

			if (!userExists)
				return NotFound(new { message = "User not found." });

			// 2) Get "Completed" status id
			var completedStatusId = await _db.Tstatuses
				.AsNoTracking()
				.Where(s => s.StrStatus == "Completed")
				.Select(s => s.IntStatusId)
				.FirstOrDefaultAsync();

			if (completedStatusId == 0)
				return BadRequest(new { message = "Completed status not configured in TStatuses." });

			// 3) Get user's habits
			var habits = await _db.Thabits
				.AsNoTracking()
				.Where(h => h.IntUserId == userId)
				.Select(h => new {
					h.IntHabitId,
					h.StrHabit,
					h.StrDescription
				})
				.ToListAsync();

			if (habits.Count == 0) 
				{
				return Ok(new {
					userId,
					today = DateTime.UtcNow.Date,
					totalHabits = 0,
					totalHabitsCompletedEver = 0,
					totalHabitsCompletedToday = 0,
					totalCompletedOccurrencesEver = 0,
					totalCompletedOccurrencesToday = 0,
					habits = new List<object>()
				});
			}

			var habitIds = habits.Select(h => h.IntHabitId).ToList();
			var today = DateTime.UtcNow.Date;

			// 4) Get all COMPLETED occurrences for these habits
			var occurrences = await _db.ThabitOccurrences
				.AsNoTracking()
				.Where(o => habitIds.Contains(o.IntHabitId)
							&& o.IntStatusId == completedStatusId
							&& o.DtmCompleted != null)
				.Select(o => new {
					o.IntHabitId,
					CompletedDate = o.DtmCompleted.Value.Date
				})
				.ToListAsync();

			// For counting raw occurrences (how many times completed)
			var totalCompletedOccurrencesEver = occurrences.Count;
			var totalCompletedOccurrencesToday = occurrences.Count(o => o.CompletedDate == today);

			// 5) Group dates by habit (for streaks + per-habit flags)
			var byHabit = occurrences
				.GroupBy(o => o.IntHabitId)
				.ToDictionary(
					g => g.Key,
					g => g.Select(x => x.CompletedDate).Distinct().ToHashSet()
				);

			var results = new List<object>();
			int habitsCompletedToday = 0;

			foreach (var habit in habits) {
				int habitId = habit.IntHabitId;
				int streak = 0;
				bool completedToday = false;

				if (byHabit.TryGetValue(habitId, out var completedDates)) {
					// Completed today?
					if (completedDates.Contains(today)) {
						completedToday = true;
						habitsCompletedToday++;
					}

					// Count consecutive days from today backwards
					var currentDay = today;
					while (completedDates.Contains(currentDay)) {
						streak++;
						currentDay = currentDay.AddDays(-1);
					}
				}

				results.Add(new {
					habitId,
					habitName = habit.StrHabit,
					description = habit.StrDescription,
					completedToday,
					currentStreakDays = streak
				});
			}

			// Number of habits that have EVER been completed:
			int totalHabitsCompletedEver = byHabit.Count;

			// Sort by longest streak, then alphabetically
			var ordered = results
				.OrderByDescending(h => ((dynamic)h).currentStreakDays)
				.ThenBy(h => ((dynamic)h).habitName)
				.ToList();

			return Ok(new {
				userId,
				today,
				totalHabits = habits.Count,
				totalHabitsCompletedEver,
				totalHabitsCompletedToday = habitsCompletedToday,
				totalCompletedOccurrencesEver,
				totalCompletedOccurrencesToday,
				habits = ordered
			});
		}
	}
}
