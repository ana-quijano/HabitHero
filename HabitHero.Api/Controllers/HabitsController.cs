using HabitHero.Core.Entities;
using HabitHero.Core.Models.Ai;
using HabitHero.Core.Models.Auth;
using HabitHero.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HabitHero.Core.Services.Ai;

namespace HabitHero.Api.Controllers
{
    public class HabitsController : Controller
    {
        private readonly HabitHeroDbContext _db;
        private readonly IGptService _gpt;
        public HabitsController(HabitHeroDbContext db, IGptService gpt)
        {
            _db = db;
            _gpt = gpt;
        }
        /// <summary>
        /// GET: User's Habit Occurrences for Today
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("api/menu/{userId}")]
        public async Task<IActionResult> GetHabits([FromRoute] int userId)
        {
            var user = await _db.Tusers.AsNoTracking()
                .FirstOrDefaultAsync(u => u.IntUserId == userId);
            if (user == null) return NotFound(new { message = "User not found." });

            var today = DateTime.Today;

            var habitsToday = await _db.ThabitOccurrences
                .AsNoTracking()
                .Where(o => o.DtmDate >= today && o.DtmDate < today.AddDays(1)
                            && _db.Thabits.Any(h => h.IntHabitId == o.IntHabitId && h.IntUserId == userId))
                .Join(_db.Thabits, o => o.IntHabitId, h => h.IntHabitId, (o, h) => new { o, h })
                .Join(_db.Tstatuses, oh => oh.o.IntStatusId, s => s.IntStatusId, (oh, s) => new
                {
                    oh.o.IntHabitOccurrenceId,   
                    oh.h.IntHabitId,
                    oh.h.StrHabit,
                    oh.h.StrDescription,
                    oh.o.DtmDate,
                    StrStatus = s.StrStatus      
                })
                .ToListAsync();

            return Ok(new
            {
                userId,
                points = user.IntPoints,   
                habits = habitsToday
            });
        }

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
            await _db.SaveChangesAsync();

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
                          oh.o.IntHabitOccurrenceId,
                          oh.h.IntHabitId,
                          oh.h.StrHabit,
                          oh.h.StrDescription,
                          oh.o.DtmDate,
                          s.StrStatus
                      })
                .ToListAsync();

            return Ok(todaysHabits);
        }

        /// <summary>
        /// Deletes habit and all corresponding habit occurences
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("api/habits/delete")]
        public async Task<IActionResult> DeleteHabit([FromBody] DeleteHabitRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userExists = await _db.Tusers
                .AsNoTracking()
                .AnyAsync(u => u.IntUserId == request.IntUserId);
            if (!userExists) return NotFound(new { message = "User not found." });

            var habit = await _db.Thabits
                .FirstOrDefaultAsync(h => h.IntHabitId == request.IntHabitId && h.IntUserId == request.IntUserId);

            if (habit == null) return NotFound(new { message = "Habit not found for this user." });

            using var tx = await _db.Database.BeginTransactionAsync();

            try
            {
                // Delete all occurrences for this habit
                await _db.ThabitOccurrences
                    .Where(o => o.IntHabitId == request.IntHabitId)
                    .ExecuteDeleteAsync();

                // Delete the habit
                _db.Thabits.Remove(habit);

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                // Get and return today's occurrences
                var today = DateTime.Today;
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
                              oh.o.IntHabitOccurrenceId,
                              oh.h.IntHabitId,
                              oh.h.StrHabit,
                              oh.h.StrDescription,
                              oh.o.DtmDate,
                              s.StrStatus
                          })
                    .ToListAsync();

                return Ok(todaysHabits);
            }
            catch
            {
                await tx.RollbackAsync();
                throw; 
            }
        }

        [HttpPost("api/habits/updateoccurence")]
        public async Task<IActionResult> MarkHabitOccurrence([FromBody] UpdateOccurrenceStatusRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Validate user
            var user = await _db.Tusers.FirstOrDefaultAsync(u => u.IntUserId == request.IntUserId);
            if (user == null) return NotFound(new { message = "User not found." });

            // Validate habit belongs to user
            var habit = await _db.Thabits
                .FirstOrDefaultAsync(h => h.IntHabitId == request.IntHabitId && h.IntUserId == request.IntUserId);
            if (habit == null) return NotFound(new { message = "Habit not found for this user." });

            // Find the occurrence to update
            var occurrence = await _db.ThabitOccurrences
                .FirstOrDefaultAsync(o => o.IntHabitOccurrenceId == request.IntHabitOccurrenceId &&
                                          o.IntHabitId == request.IntHabitId);
            if (occurrence == null)
                return NotFound(new { message = "Habit occurrence not found." });

            using var tx = await _db.Database.BeginTransactionAsync();

            try
            {
                // Track the previous status
                var prevStatus = occurrence.IntStatusId;

                // Update status
                occurrence.IntStatusId = request.IntStatusId;

                // Award points only when transitioning from not Done → Done
                if (prevStatus != 2 && request.IntStatusId == 2)
                {
                    user.IntPoints += 10;
                }

                await _db.SaveChangesAsync();

                // Get today's updated list
                var today = DateTime.Today;
                var start = today;
                var end = today.AddDays(1);

                var todaysHabits = await _db.ThabitOccurrences
                    .AsNoTracking()
                    .Where(o => o.DtmDate >= start && o.DtmDate < end
                                && _db.Thabits.Any(h => h.IntHabitId == o.IntHabitId && h.IntUserId == request.IntUserId))
                    .Join(_db.Thabits, o => o.IntHabitId, h => h.IntHabitId, (o, h) => new { o, h })
                    .Join(_db.Tstatuses, oh => oh.o.IntStatusId, s => s.IntStatusId, (oh, s) => new
                    {
                        oh.o.IntHabitOccurrenceId,
                        oh.h.IntHabitId,
                        oh.h.StrHabit,
                        oh.h.StrDescription,
                        oh.o.DtmDate,
                        s.StrStatus
                    })
                    .ToListAsync();

                await tx.CommitAsync();

                return Ok(new
                {
                    habits = todaysHabits,
                    points = user.IntPoints
                });
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        [HttpPost("api/habits/generate-ai")]
        public async Task<IActionResult> GenerateAIHabits([FromBody] GenerateAIHabitsRequest request, CancellationToken ct)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.StrGoal))
                return BadRequest(new { message = "Goal is required." });

            var payload = await _gpt.GenerateHabitsAsync(request.StrGoal.Trim(), ct);

            return Ok(payload);
        }
    }
}
