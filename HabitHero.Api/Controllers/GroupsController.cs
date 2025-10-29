using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HabitHero.Infrastructure.Data;
using HabitHero.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HabitHero.Api.Controllers
{
    [ApiController]
    [Route("api/groups")]
    public class GroupsController : ControllerBase
    {
        private readonly HabitHeroDbContext _db;
        public GroupsController(HabitHeroDbContext db) => _db = db;


        //Post: Create Group
        [HttpGet("CreateGroup/{strQuestName}/{monMoneyPot}/{decPointsPot}/{dtmStartDate}/{dtmEndDate}")]
        public async Task<IActionResult> CreateGroup([FromRoute] string strQuestName, [FromRoute] decimal monMoneyPot, [FromRoute] decimal decPointsPot, [FromRoute] DateTime dtmStartDate, [FromRoute] DateTime dtmEndDate)
        {
            if (!ModelState.IsValid)
            { 
                return BadRequest(ModelState);
            }

            var existingQuest = await _db.Tquests.FirstOrDefaultAsync(q => q.StrQuestName == strQuestName);

            if (existingQuest != null)
            {
                return Conflict(new { message = $"Group-Quest '{strQuestName}' already exist." });
            }

            var groupQuest = new Tquest
            {
                StrQuestName = strQuestName,
                MonMoneyPot = monMoneyPot,
                DecPointsPot = decPointsPot,
                DtmStartDate = dtmStartDate,
                DtmEndDate = dtmEndDate
            };

            _db.Tquests.Add(groupQuest);
            await _db.SaveChangesAsync();

            var userId = groupQuest.IntQuestId;

            return Ok($"New Group-Quest for '{userId}' successfully created.");
        }


        //Put: Update Group 
        [HttpPost("UpdateGroup/{intQuestId}/{strQuestName}/{monMoneyPot}/{decPointsPot}/{dtmStartDate}/{dtmEndDate}")]
        public async Task<ActionResult> UpdateGroup([FromRoute] int intQuestId, [FromRoute] string strQuestName, [FromRoute] decimal monMoneyPot, [FromRoute] decimal decPointsPot, [FromRoute] DateTime dtmStartDate, [FromRoute] DateTime dtmEndDate)
        {
            if (!ModelState.IsValid)
            { 
                return BadRequest(ModelState);
            }

            var existingGroup = await _db.Tquests.FirstOrDefaultAsync(g => g.IntQuestId == intQuestId);

            if (existingGroup == null)
            {
                return NotFound(new { message = "QuestID not found." });
            }

            existingGroup.StrQuestName = strQuestName;
            existingGroup.MonMoneyPot = monMoneyPot;
            existingGroup.DecPointsPot = decPointsPot;
            existingGroup.DtmStartDate = dtmStartDate;
            existingGroup.DtmEndDate = dtmEndDate;

            await _db.SaveChangesAsync();

            return Ok($"QuestID '{intQuestId}' succesfully uploaded to '{strQuestName}'.");
        }


        //Delete: Delete Group
        [HttpPost("DeleteGroup/{id:int}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var group = await _db.Tquests.FindAsync(id);

            if (group == null)
            {
                return NotFound(new { message = "Group not found." });
            }

            _db.Tquests.Remove(group);
            await _db.SaveChangesAsync();

            return Ok($"Group successfully Deleted.");
        }


        private bool GroupExists(int id)
        {
            return _db.Tquests.Any(e => e.IntQuestId == id);
        }
    }
}
