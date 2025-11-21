using HabitHero.Core.Entities;
using HabitHero.Core.Models.Auth;
using HabitHero.Core.Services.Ai;
using HabitHero.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet("api/quests/{userId}")]
        public async Task<IActionResult> GetQuests([FromRoute] int userId)
        {
            var user = await _db.Tusers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IntUserId == userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var quests = await _db.TuserQuests
                .AsNoTracking()
                .Where(uq => uq.IntUserId == userId)
                .Select(uq => uq.Tquest)
                .ToListAsync();

            return Ok(quests);
        }

        [HttpPost("api/quests/create")]
        public async Task<IActionResult> CreateQuest([FromBody] CreateQuestRequest request)
        {
            if (request.StrQuestName == null)
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

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }

    

}
