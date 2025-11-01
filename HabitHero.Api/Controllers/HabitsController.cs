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

        ///// <summary>
        ///// POST: CREATE NEW HABIT
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <param name="strHabit"></param>
        ///// <param name="strDescription"></param>
        ///// <returns>Updated list of habits</returns>
        //[HttpPost("api/habits/addhabit")]
        //public async Task<IActionResult> AddHabit([FromBody] AddHabitRequest request)
        //{
        //    if (!ModelState.IsValid) return BadRequest(ModelState);

        //    var userExists = await _db.Tusers
        //        .AsNoTracking()
        //        .AnyAsync(u => u.IntUserId == request.IntUserId);

        //    if (!userExists)
        //        return NotFound(new { message = "User not found." });

        //    var newHabit = new Thabit
        //    {
        //        IntUserId = request.IntUserId,
        //        StrHabit = request.StrHabit,
        //        StrDescription = request.StrDescription
        //    };

        //    _db.Thabits.Add(newHabit);
        //    await _db.SaveChangesAsync();

        //    var updatedHabits = await _db.Thabits
        //        .Where(h => h.IntUserId == request.IntUserId)
        //        .Select(h => new
        //        {
        //            h.IntHabitId,
        //            h.StrHabit,
        //            h.IntScheduleId,
        //            h.StrDescription
        //        })
        //        .ToListAsync();

        //    return Ok(updatedHabits);
        //}

        ///// <summary>
        ///// POST: CREATE NEW HABIT
        ///// Creates new THabit and THabitOccurence for current date
        ///// </summary>
        ///// <returns>Updated list of current date habit occurences</returns>
        [HttpPost("api/habits/add")]
        public async Task<IActionResult> AddHabit([FromBody] AddHabitRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userExists = await _db.Tusers
                .AsNoTracking()
                .AnyAsync(u => u.IntUserId == request.IntUserId);
            if (!userExists) return NotFound(new { message = "User not found." });

            // Create the habit on THabits
            var newHabit = new Thabit
            {
                IntUserId = request.IntUserId,
                StrHabit = request.StrHabit.Trim(),
                StrDescription = request.StrDescription.Trim(),
            };
            _db.Thabits.Add(newHabit);
            await _db.SaveChangesAsync(); // get IntHabitId

            // Create habit occurence of new habit with today's date
            var today = DateTime.Today;
            var occurrence = new ThabitOccurrence
            {
                IntHabitId = newHabit.IntHabitId,
                DtmDate = today,
                IntStatusId = 1 // To Do
            };
            _db.ThabitOccurrences.Add(occurrence);
            await _db.SaveChangesAsync();

            // Get and return habit occurences for today
            var start = today;
            var end = today.AddDays(1);

            var todaysHabits = await _db.ThabitOccurrences
                .AsNoTracking()
                .Where(o => o.DtmDate >= start && o.DtmDate < end
                            && _db.Thabits.Any(h => h.IntHabitId == o.IntHabitId && h.IntUserId == request.IntUserId))
                .Join(_db.Thabits,
                      o => o.IntHabitId,
                      h => h.IntHabitId,
                      (o, h) => new { o, h })
                .Join(_db.Tstatuses,
                      oh => oh.o.IntStatusId,
                      s => s.IntStatusId,
                      (oh, s) => new
                      {
                          oh.h.IntHabitId,
                          oh.h.StrHabit,
                          oh.h.StrDescription,
                          oh.o.DtmDate,
                          s.StrStatus
                      })
                .ToListAsync();

            return Ok(todaysHabits);
        }
    }
}
