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

        public class DeleteQuestRequest
        {
            public int IntQuestId { get; set; }
            public int IntUserId { get; set; }
        }

        public class AcceptRejectInviteRequest
        {
            public int IntUserQuestId { get; set; }
            public int IntUserId { get; set; }
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
                .Where(uq => uq.IntUserId == userId
                        && uq.BlnAccepted)
                .Select(uq => uq.Tquest)
                .ToListAsync();

            var userInvites = await _db.TuserQuests
                .AsNoTracking()
                .Where(uq => uq.IntUserId == userId && !uq.BlnAccepted)
                .Select(uq => new
                {
                    intUserQuestId = uq.IntUserQuestId,
                    intQuestId = uq.IntQuestId,
                    strQuestName = uq.Tquest.StrQuestName,
                    decPoints = uq.Tquest.DecPointsPot
                })
                .ToListAsync();

            return Ok(new
            {
                quests = userQuests,
                points = user.IntPoints,
                invites = userInvites
            });
        }

        [HttpGet("api/getuserinvites/{userId}")]
        public async Task<IActionResult> GetUserInvites([FromRoute] int userId)
        {
            var user = await _db.Tusers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IntUserId == userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var userInvites = await _db.TuserQuests
                .AsNoTracking()
                .Where(uq => uq.IntUserId == userId && !uq.BlnAccepted)
                .Select(uq => new
                {
                    intUserQuestId = uq.IntUserQuestId,
                    intQuestId = uq.IntQuestId,
                    strQuestName = uq.Tquest.StrQuestName,
                    decPoints = uq.Tquest.DecPointsPot
                })
                .ToListAsync();

            return Ok(new
            {
                invites = userInvites
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
            var userCount = users.Count;

            // Get quest habits
            var questHabits = await _db.TquestHabits
                .Where(qh => qh.IntQuestId == questId)
                .ToListAsync();

            
            return Ok(new
            {
                intQuestId = questId,
                strQuestName = quest.StrQuestName,
                intPointsPot = (int)quest.DecPointsPot * userCount,
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
                    IntQuestId = newQuest.IntQuestId,
                    BlnAccepted = true
                };
                _db.TuserQuests.Add(newUserQuest);
                await _db.SaveChangesAsync();

                // Subtract stake from user
                var user = await _db.Tusers
                    .FirstOrDefaultAsync(u => u.IntUserId == request.IntUserId);

                if (user == null)
                {
                    return BadRequest();
                }

                if (request.DecPointsPot <= user.IntPoints)
                {
                    user.IntPoints = user.IntPoints - (int)request.DecPointsPot;
                    await _db.SaveChangesAsync();
                }
                else
                {
                    return BadRequest("Sorry, you don't have enough points to join this quest.");
                }

                return Ok(new
                {
                    points = user.IntPoints
                });
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

        /// <summary>
        /// Invite User to Quest
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("api/quests/inviteuserquest")]
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

            var quest = await _db.Tquests
                .AsNoTracking()
                .FirstOrDefaultAsync(q => q.IntQuestId == request.IntQuestId);

            if (quest == null)
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

            // AFTER saving to DB, try to send push notification
            if (!string.IsNullOrWhiteSpace(user.StrPushToken))
            {
                try
                {
                    await SendQuestInviteNotificationAsync(
                        user.StrPushToken,
                        quest.StrQuestName,
                        request.StrUserName
                    );
                }
                catch (Exception ex)
                {
                    // log error; don't fail the invite just because notification failed
                    Console.WriteLine($"Error sending push notification: {ex.Message}");
                }
            }

            return Ok(new
            {
                userQuestId = userQuest.IntUserQuestId
            });
        }

        /// <summary>
        /// Post: Accept Quest Invite
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("api/quests/acceptinvite")]
        public async Task<IActionResult> AcceptQuestInvite([FromBody] AcceptRejectInviteRequest request)
        {
            var user = await _db.Tusers
                .FirstOrDefaultAsync(u => u.IntUserId == request.IntUserId);

            if (user == null)
            {
                return NotFound();
            }

            var userQuest = await _db.TuserQuests
                .FirstOrDefaultAsync(uq => uq.IntUserQuestId == request.IntUserQuestId
                                    && uq.IntUserId == request.IntUserId);

            if (userQuest == null)
            {
                return BadRequest();
            }

            var quest = await _db.Tquests
                .FirstOrDefaultAsync(q => q.IntQuestId == userQuest.IntQuestId);
            
            userQuest.BlnAccepted = true;
            if (quest.DecPointsPot <= user.IntPoints)
            {
                user.IntPoints -= (int)quest.DecPointsPot;
            }
            else
            {
                return BadRequest("Sorry, you don't have enough points to join this quest.");
            }
            await _db.SaveChangesAsync();

            var updatedUserQuests = await _db.TuserQuests
                .AsNoTracking()
                .Where(uq => uq.IntUserId == request.IntUserId
                        && uq.BlnAccepted)
                .Select(uq => uq.Tquest)
                .ToListAsync();

            return Ok(new
            {
                quests = updatedUserQuests,
                points = user.IntPoints,
            });
        }

        [HttpPost("api/quest/rejectquestinvite")]
        public async Task<IActionResult> RejectQuestInvite([FromBody] AcceptRejectInviteRequest request)
        {
            var user = await _db.Tusers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IntUserId == request.IntUserId);

            if (user == null)
            {
                return NotFound();
            }

            var userQuest = await _db.TuserQuests
                .FirstOrDefaultAsync(uq => uq.IntUserQuestId == request.IntUserQuestId
                                    && uq.IntUserId == request.IntUserId);

            if (userQuest == null)
            {
                return BadRequest();
            }

            _db.TuserQuests.Remove(userQuest);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Quest deleted successfully." });
        }

        /// <summary>
        /// Delete quest
        /// </summary>
        /// <param name="expoPushToken"></param>
        /// <returns></returns>
        [HttpPost("api/quests/deletequest")]
        public async Task<IActionResult> DeleteQuest([FromBody] DeleteQuestRequest request)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                // Delete quest habits by questID
                var questHabits = _db.TquestHabits
                    .Where(qh => qh.IntQuestId == request.IntQuestId);

                if (questHabits.Any())
                {
                    _db.TquestHabits.RemoveRange(questHabits);
                }

                // Delete user quests  by questID
                var userQuests = _db.TuserQuests
                    .Where(uq => uq.IntQuestId == request.IntQuestId);

                if (userQuests.Any())
                { 
                    _db.TuserQuests.RemoveRange(userQuests);
                }

                // Delete quest
                var quest = await _db.Tquests
                    .FirstOrDefaultAsync(q => q.IntQuestId == request.IntQuestId);
                if (quest != null)
                {
                    _db.Tquests.Remove(quest);
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Quest deleted successfully." });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "An error occurred while deleting the quest.");
            }

            
        }

        

        /// <summary>
        /// Notification
        /// </summary>
        /// <param name="expoPushToken"></param>
        /// <param name="questName"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        private async Task SendQuestInviteNotificationAsync(string expoPushToken, string questName, string username)
        {
            using var client = new HttpClient();

            var payload = new[]
            {
                new
                {
                    to = expoPushToken,
                    title = "New Quest Invite ✨",
                    body = $"You’ve been invited to join \"{questName}\".",
                    data = new
                    {
                        questName,
                        type = "questInvite"
                    }
                }
            };

            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://exp.host/--/api/v2/push/send", content);

            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Expo push response: {response.StatusCode} - {responseBody}");
        }
    }
}
