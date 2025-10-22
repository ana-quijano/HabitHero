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

        // Get: api/groups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ThabitGroup>>> GetGroups()
        {
            return await _db.ThabitGroups.ToListAsync();
        }




        //Get: Group ID
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ThabitGroup>> GetGroup(int id)
        {
            var group = await _db.ThabitGroups.FindAsync(id);
            if (group == null)
            {
                return NotFound(new { message = "Group not found." });
            }

            return Ok(group);
        }

        //Post: Create Group
        [HttpPut]
        public async Task<ActionResult> CreateGroup([FromBody] ThabitGroup group)
        {
            if (!ModelState.IsValid)
            { 
                return BadRequest(ModelState);
            }

            _db.ThabitGroups.Add(group);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGroup), new { id = group.IntGroupID }, group);
        }

        //Put: Update Group 
        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateGroup(int id, [FromBody] ThabitGroup group)
        {
            if (id != group.IntGroupID)
            {
                return BadRequest("Group ID not found.");
            }

            _db.Entry(group).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(id))
                {
                    return NotFound(new { message = "Group not found." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        //Delete: Delete Group
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var group = await _db.ThabitGroups.FindAsync(id);

            if (group == null)
            {
                return NotFound(new { message = "Group not found." });
            }

            _db.ThabitGroups.Remove(group);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private bool GroupExists(int id)
        {
            return _db.ThabitGroups.Any(e => e.IntGroupID == id);
        }
    }
}
