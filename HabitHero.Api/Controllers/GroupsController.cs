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


        //// Get: api/groups
        //[HttpGet("GetAllGroups/{id}")]
        //public async Task<ActionResult<IEnumerable<ThabitGroup>>> GetGroups()
        //{
        //    return await _db.ThabitGroups.ToListAsync();
        //}


        ////Get: Group ID
        //[HttpGet("GetSingleGroup/{id:int}")]
        //public async Task<ActionResult<ThabitGroup>> GetGroup(int id)
        //{
        //    var group = await _db.ThabitGroups.FindAsync(id);
        //    if (group == null)
        //    {
        //        return NotFound(new { message = "Group not found." });
        //    }

        //    return Ok(group);
        //}


        //Post: Create Group
        [HttpPut("CreateGroup/{groupname}/{groupdescription}")]
        public async Task<IActionResult> CreateGroup([FromRoute] string groupname, [FromRoute] string groupdescription)
        {
            if (!ModelState.IsValid)
            { 
                return BadRequest(ModelState);
            }

            var userGroup = new ThabitGroup
            {
                StrGroupName = groupname,
                StrDescription = groupdescription,
            };

            _db.ThabitGroups.Add(userGroup);
            await _db.SaveChangesAsync();

            return Ok($"New Habit-Group for '{groupname}' successfully created.");
        }


        //Put: Update Group 
        [HttpPut("UpdateGroup/{groupname}/{newgroupname}/{newgroupdescription}")]
        public async Task<ActionResult> UpdateGroup([FromRoute] string groupname, [FromRoute] string newgroupname, [FromRoute] string newgroupdescription)
        {
            if (!ModelState.IsValid)
            { 
                return BadRequest(ModelState);
            }

            var existingGroup = await _db.ThabitGroups.FirstOrDefaultAsync(g => g.StrGroupName == groupname);

            if (existingGroup == null)
            {
                return NotFound(new { message = "Group not found." });
            }

            existingGroup.StrGroupName = newgroupname;
            existingGroup.StrDescription = newgroupdescription;

            await _db.SaveChangesAsync();
            
            return Ok($"'Group '{newgroupname}' successfully Updated.");
        }


        //Delete: Delete Group
        [HttpDelete("DeleteGroup/{id:int}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var group = await _db.ThabitGroups.FindAsync(id);

            if (group == null)
            {
                return NotFound(new { message = "Group not found." });
            }

            _db.ThabitGroups.Remove(group);
            await _db.SaveChangesAsync();

            return Ok($"Group successfully Deleted.");
        }


        private bool GroupExists(int id)
        {
            return _db.ThabitGroups.Any(e => e.IntGroupID == id);
        }
    }
}
