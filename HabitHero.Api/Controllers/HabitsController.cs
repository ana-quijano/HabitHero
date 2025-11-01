using HabitHero.Core.Entities;
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
        /// GET: User Habits
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("api/menu/{userId}")] 
        public async Task<IActionResult> GetHabits([FromRoute] int userId)
        {
            var userExists = await _db.Tusers
                .AsNoTracking()
                .AnyAsync(u => u.IntUserId == userId);

            if (!userExists)
                return NotFound(new { message = "User not found." });

            var habits = await _db.Thabits
                .AsNoTracking()
                .Where(h => h.IntUserId == userId)
                .Select(h => new
                {
                    h.IntHabitId,
                    h.StrHabit,      
                    h.StrDescription
                })
                .ToListAsync();

            return Ok(new
            {
                userId,
                habits
            });
        }

        /// <summary>
        /// POST: CREATE NEW HABIT
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="strHabit"></param>
        /// <param name="strDescription"></param>
        /// <returns>Updated list of habits</returns>
        [HttpGet("api/habits/{userId}/{strHabit}/{strDescription}")]
        public async Task<IActionResult> CreateHabit([FromRoute] int userId, [FromRoute] string strHabit, [FromRoute] string strDescription)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userExists = await _db.Tusers
                .AsNoTracking()
                .AnyAsync(u => u.IntUserId == userId);

            if (!userExists)
                return NotFound(new { message = "User not found." });

            var newHabit = new Thabit
            {
                IntUserId = userId,
                StrHabit = strHabit,
                IntScheduleId = 1,
                StrDescription = strDescription
            };

            _db.Thabits.Add(newHabit);
            await _db.SaveChangesAsync();

            var updatedHabits = await _db.Thabits
                .AsNoTracking()
                .Where(h => h.IntUserId == userId)
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
