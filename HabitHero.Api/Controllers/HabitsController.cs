using HabitHero.Core.Entities;
using HabitHero.Core.Models.Auth;
using HabitHero.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HabitHero.Api.Controllers
{
    public class HabitsController : Controller
    {
        private readonly HabitHeroDbContext _db;
        public HabitsController(HabitHeroDbContext db) => _db = db;

        /// <summary>
        /// GET: User's Habit Occurrences for Today
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("api/menu/{userId}")]
        public async Task<IActionResult> GetHabits([FromRoute] int userId)
        {
            // Check if user exists
            var userExists = await _db.Tusers
                .AsNoTracking()
                .AnyAsync(u => u.IntUserId == userId);

            if (!userExists)
                return NotFound(new { message = "User not found." });

            // Get today's date (without time)
            var today = DateTime.Today;

            // Join THabitOccurrences with THabits for this user and today's date
            var habitsToday = await _db.ThabitOccurrences
                .AsNoTracking()
                .Where(o =>
                    o.DtmDate >= today &&
                    o.DtmDate < today.AddDays(1) &&
                    _db.Thabits.Any(h => h.IntHabitId == o.IntHabitId && h.IntUserId == userId))
                .Join(_db.Thabits,
                    o => o.IntHabitId,
                    h => h.IntHabitId,
                    (o, h) => new
                    {
                        h.IntHabitId,
                        h.StrHabit,
                        h.StrDescription,
                        o.DtmDate,
                        o.IntStatusId
                    })
                .ToListAsync();

            return Ok(new
            {
                userId,
                habits = habitsToday
            });
        }

        /// <summary>
        /// POST: CREATE NEW HABIT
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="strHabit"></param>
        /// <param name="strDescription"></param>
        /// <returns>Updated list of habits</returns>
        [HttpPost("api/habits/addhabit")]
        public async Task<IActionResult> AddHabit([FromBody] AddHabitRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userExists = await _db.Tusers
                .AsNoTracking()
                .AnyAsync(u => u.IntUserId == request.IntUserId);

            if (!userExists)
                return NotFound(new { message = "User not found." });

            var newHabit = new Thabit
            {
                IntUserId = request.IntUserId,
                StrHabit = request.StrHabit,
                StrDescription = request.StrDescription
            };

            _db.Thabits.Add(newHabit);
            await _db.SaveChangesAsync();

            var updatedHabits = await _db.Thabits
                .Where(h => h.IntUserId == request.IntUserId)
                .Select(h => new
                {
                    h.IntHabitId,
                    h.StrHabit,
                    h.IntScheduleId,
                    h.StrDescription
                })
                .ToListAsync();

            return Ok(updatedHabits);
        }

        /// <summary>
        /// GET: EDIT EXISTING HABIT
        /// </summary>
        /// <param name="habitId"></param>
        /// <returns>Updated list of habits for the owner</returns>
        [HttpGet("api/habits/{habitId}/{strHabit}/{strDescription}/{intScheduleId}")]
        public async Task<IActionResult> EditHabit([FromRoute] int habitId, [FromRoute] string strHabit, [FromRoute] string strDescription, [FromRoute] int intScheduleId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Load the habit
            var habit = await _db.Thabits.FirstOrDefaultAsync(h => h.IntHabitId == habitId);
            if (habit == null)
                return NotFound(new { message = "Habit not found." });

            // Apply only provided fields
            if (strHabit != null)
            {
                if (string.IsNullOrWhiteSpace(strHabit))
                    return BadRequest(new { message = "Habit cannot be empty." });

                habit.StrHabit = strHabit.Trim();
            }

            if (strDescription != null)
            { 
                habit.StrDescription = strDescription.Trim();
            }

            if (intScheduleId != null)
            { 
                habit.IntScheduleId = intScheduleId;
            }
            await _db.SaveChangesAsync();

            // Return the updated list for this user
            var updatedHabits = await _db.Thabits
                .AsNoTracking()
                .Where(h => h.IntUserId == habit.IntUserId)
                .Select(h => new
                {
                    h.IntHabitId,
                    h.StrHabit,
                    h.IntScheduleId,
                    h.StrDescription
                })
                .ToListAsync();

            return Ok(updatedHabits);
        }
    }
}
