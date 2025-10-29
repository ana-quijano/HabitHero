using HabitHero.Infrastructure.Data;
using HabitHero.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace HabitHero.Api.Controllers
{
    [ApiController]
    [Route("api/questhabit")]
    public class QuestHabitController : ControllerBase
    {
        private readonly HabitHeroDbContext _db;
        public QuestHabitController(HabitHeroDbContext db) => _db = db;


            //Create QuestHabit.
        [HttpGet("CreateQuestHabit/{intQuestId:int}/{intScheduleId:int}/{strHabitName}/{dtmReminderTime}/{dtmEndDate}/{strDescription?}")]
        public async Task<IActionResult> CreateQuestHabit([FromRoute] int intQuestId, [FromRoute] int intScheduleId, [FromRoute] string strHabitName, [FromRoute] TimeSpan dtmReminderTime, [FromRoute] DateTime dtmEndDate, [FromRoute] string? strDescription = null)
        {
                if (!ModelState.IsValid)
                { 
                    return BadRequest(ModelState);
                }

                var existingHabit = await _db.TquestHabits.FirstOrDefaultAsync(h => h.IntQuestId == intQuestId && h.StrHabitName == strHabitName);

                if (existingHabit != null)
                {
                    return Conflict(new { message = $"Habit '{strHabitName}' already exists in this quest." });
                }

                var questHabit = new TquestHabit
                {
                    IntQuestId = intQuestId,
                    IntScheduleId = intScheduleId,
                    StrHabitName = strHabitName,
                    DtmReminderTime = dtmReminderTime,
                    DtmStartDate = DateTime.Now,
                    DtmEndDate = dtmEndDate,
                    StrDescription = strDescription
                };

                _db.TquestHabits.Add(questHabit);
                await _db.SaveChangesAsync();

                var questHabitId = questHabit.IntQuestHabitId;

                return Ok($"HabitID '{questHabitId}' successfully created for QuestID {intQuestId}.");
        }


            //Update QuestHabit.
        [HttpPost("UpdateQuestHabit/{intQuestHabitId}/{intQuestId}/{intScheduleId}/{strHabitName}/{dtmReminderTime}/{dtmEndDate}/{strDescription?}")]
        public async Task<IActionResult> UpdateQuestHabit([FromRoute] int intQuestHabitId, [FromRoute] int intQuestId, [FromRoute] int intScheduleId, [FromRoute] string strHabitName, [FromRoute] TimeSpan dtmReminderTime, [FromRoute] DateTime dtmEndDate, [FromRoute] string? strDescription = null)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingHabit = await _db.TquestHabits.FirstOrDefaultAsync(h => h.IntQuestHabitId == intQuestHabitId);

            if (existingHabit == null)
            {
                return NotFound(new { message = $"HabitID '{intQuestHabitId}' not found." });
            }

            existingHabit.IntQuestId = intQuestId;
            existingHabit.IntScheduleId = intScheduleId;
            existingHabit.StrHabitName = strHabitName;
            existingHabit.DtmReminderTime = dtmReminderTime;
            existingHabit.DtmEndDate = dtmEndDate;
            existingHabit.StrDescription = strDescription ?? existingHabit.StrDescription;

            await _db.SaveChangesAsync();

            return Ok($"HabitID '{intQuestHabitId}' succesfully uploaded to '{strHabitName}'.");
        }


        //Delete QuestHabit.
        [HttpPost("DeleteQuestHabit/{intQuestHabitId}")]
        public async Task<IActionResult> DeleteQuestHabit([FromRoute] int intQuestHabitId)
        {
                var existingHabit = await _db.TquestHabits.FirstOrDefaultAsync(h => h.IntQuestHabitId == intQuestHabitId);

                if (existingHabit == null)
                {
                    return NotFound(new { message = $"HabitID '{intQuestHabitId}' not found." });
                }

                _db.TquestHabits.Remove(existingHabit);
                await _db.SaveChangesAsync();

                return Ok($"Habit successfully deleted.");
        }
    }
}
