using HabitHero.Core.Entities;
using HabitHero.Core.Models.Auth;
using HabitHero.Core.Services.Ai;
using HabitHero.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace HabitHero.Api.Controllers
{
    public class QuestsController : Controller
    {
        private readonly HabitHeroDbContext _db;
        private readonly IGptService _gpt;
        public QuestsController(HabitHeroDbContext db, IGptService gpt)
        {
            _db = db;
            _gpt = gpt;
        }

        [HttpGet("api/getuserquests/{userId}")]
        public async Task<IActionResult> GetUserQuests([FromRoute] int userId)
        {
            var user = await _db.Tusers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IntUserId == userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var userQuests = await _db.TuserQuests
                .AsNoTracking()
                .Where(uq => uq.IntUserId == userId)
                .Select(uq => uq.Tquest)
                .ToListAsync();

            return Ok(new
            {
                quests = userQuests
            });
        }

        [HttpGet("api/getquestdetails/{questId}")]
        public async Task<IActionResult> GetQuestDetails([FromRoute] int questId)
        {
            // Get quest
            var quest = await _db.Tquests
                .Where(q => q.IntQuestId == questId)
                .FirstOrDefaultAsync();

            if (quest == null)
            {
                return BadRequest("Quest not found.");
            }

            // Get users in quest
            var userQuests = await _db.TuserQuests
                .Where(uq => uq.IntQuestId == questId)
                .ToListAsync();
            var userIds = userQuests
                .Select(uq => uq.IntUserId)
                .ToList();
            var users = await _db.Tusers
                .Where(u => userIds.Contains(u.IntUserId))
                .ToListAsync();

            // Get quest habits
            var questHabits = await _db.TquestHabits
                .Where(qh => qh.IntQuestId == questId)
                .ToListAsync();

            return Ok(new
            {
                strQuestName = quest.StrQuestName,
                intPointsPot = (int)quest.DecPointsPot,
                users = users.Select(u => new {
                    intUserId = u.IntUserId,
                    strUsername = u.StrUsername
                }),
                habits = questHabits.Select(qh => new
                {
                    strHabitName = qh.StrHabitName,
                    strDescription = qh.StrDescription
                })
            });
        }

        [HttpPost("api/quests/createquest")]
        public async Task<IActionResult> CreateQuest([FromBody] CreateQuestRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.StrQuestName))
            {
                return BadRequest("Quest name is required.");
            }

            if (request.DecPointsPot < 0)
            {
                return BadRequest("Points Pot must be greater than or equal to 0.");
            }

            try
            {
                // Create new quest
                var newQuest = new Tquest
                {
                    StrQuestName = request.StrQuestName,
                    DecPointsPot = request.DecPointsPot
                };
                _db.Tquests.Add(newQuest);
                await _db.SaveChangesAsync();

                // Create new user quest
                var newUserQuest = new TuserQuest
                {
                    IntUserId = request.IntUserId,
                    IntQuestId = newQuest.IntQuestId
                };
                _db.TuserQuests.Add(newUserQuest);
                await _db.SaveChangesAsync();

                // Subtract stake from user
                var user = await _db.Tusers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IntUserId == request.IntUserId);

                if (user != null)
                {
                    user.IntPoints = user.IntPoints - (int)request.DecPointsPot;
                    await _db.SaveChangesAsync();
                }

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("api/quests/addquesthabits")]
        public async Task<IActionResult> AddQuestHabit([FromBody] AddQuestHabitRequest request)
        {
            if (request == null)
            {
                return BadRequest();    
            }

            if (string.IsNullOrWhiteSpace(request.StrHabitName) || string.IsNullOrWhiteSpace(request.StrDescription))
            {
                return BadRequest();
            }

            // Create new habit
            var questHabit = new TquestHabit
            {
                IntQuestId = request.IntQuestId,
                StrHabitName = request.StrHabitName,
                StrDescription = request.StrDescription,
                IntScheduleId = 1
            };
            _db.TquestHabits.Add(questHabit);
            await _db.SaveChangesAsync();

            // Get and return all quest habits
            var allQuestHabits = await _db.TquestHabits
                .AsNoTracking()
                .Where(qh => qh.IntQuestId == request.IntQuestId)
                .ToListAsync();

            return Ok(allQuestHabits);
        }

        [HttpPost("api/quests/adduserquest")]
        public async Task<IActionResult> InviteUserToQuest([FromBody] InviteUserToQuest request)
        {
            if (string.IsNullOrWhiteSpace(request.StrUserName))
            {
                return BadRequest("Username is required.");
            }

            if (request.IntQuestId <= 0)
            {
                return BadRequest("Invalid quest ID.");
            }

            var questExists = await _db.Tquests
                .AsNoTracking()
                .AnyAsync(q => q.IntQuestId == request.IntQuestId);

            if (!questExists)
            {
                return NotFound("Quest not found.");
            }

            var user = await _db.Tusers
                .FirstOrDefaultAsync(u => u.StrUsername == request.StrUserName);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Check that user is not already in quest
            var alreadyInQuest = await _db.TuserQuests
                .AnyAsync(uq => uq.IntUserId == user.IntUserId && uq.IntQuestId == request.IntQuestId);

            if (alreadyInQuest)
            {
                return Conflict("User is already in this quest.");
            }

            // Create new user quest with false accepted
            var userQuest = new TuserQuest
            {
                IntUserId = user.IntUserId,
                IntQuestId = request.IntQuestId,
                BlnAccepted = false
            };

            _db.TuserQuests.Add(userQuest);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                userQuestId = userQuest.IntUserQuestId
            });
        }
    }
}
