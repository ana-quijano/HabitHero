using HabitHero.Core.Entities;
using HabitHero.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HabitHero.Api.Controllers
{
    public class AvatarController : Controller
    {
        private readonly HabitHeroDbContext _db;
        public AvatarController(HabitHeroDbContext db) => _db = db;

        /// <summary>
        /// GET: User Inventory
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("api/avatar/getInventory/{userId}")]
        public async Task<IActionResult> GetInventory([FromRoute] int userId)
        {
            // 1. Check if user exists
            var user = await _db.Tusers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IntUserId == userId);

            if (user == null)
                return NotFound(new { message = "User not found." });

            if (user.IntAvatarId == null)
                return NotFound(new { message = "User does not have an avatar assigned." });

            var avatarId = user.IntAvatarId.Value;

            // 2. Join Items + AvatarItems (to get quantity) filtered by avatarId
            var items = await _db.TavatarItems
                .AsNoTracking()
                .Where(ai => ai.IntAvatarId == avatarId)
                .Join(_db.Titems,
                    ai => ai.IntItemId,
                    item => item.IntItemId,
                    (ai, item) => new
                    {
                        id = item.IntItemId,
                        name = item.StrItem,
                        slug = item.StrSlug,
                        quantity = ai.IntQuantity
                    })
                .ToListAsync();

            // 3. Return JSON
            return Ok(new { items });
        }

        [HttpGet("api/avatar/updateInventory/{userId}/{itemSlug}")]
        public async Task<IActionResult> UpdateInventory(int userId, string itemSlug)
        {
            // 1. Validate user
            var user = await _db.Tusers
                .Include(u => u.Tavatar)
                .FirstOrDefaultAsync(u => u.IntUserId == userId);

            if (user == null)
                return NotFound(new { message = $"User {userId} not found." });

            if (user.IntAvatarId == null)
                return BadRequest(new { message = "User has no avatar." });

            var avatar = user.Tavatar;

            // 2. Find item by slug
            var item = await _db.Titems
                .FirstOrDefaultAsync(i => i.StrSlug == itemSlug);

            if (item == null)
                return NotFound(new { message = $"Item '{itemSlug}' not found." });

            // 3. Find avatar's item entry
            var avatarItem = await _db.TavatarItems
                .FirstOrDefaultAsync(ai => ai.IntAvatarId == avatar.IntAvatarId &&
                                           ai.IntItemId == item.IntItemId);

            if (avatarItem == null)
                return NotFound(new { message = $"Avatar does not have '{itemSlug}'." });

            // 4. Remove 1 quantity
            avatarItem.IntQuantity -= 1;
            if (avatarItem.IntQuantity <= 0)
                _db.TavatarItems.Remove(avatarItem);
            else
                _db.TavatarItems.Update(avatarItem);

            // 5. ⭐ Update avatar health
            //    Item cost = direct health gain
            avatar.IntHealth += item.IntPrice;
            avatar.IntHealth = Math.Min(100, avatar.IntHealth); // cap at 100

            // 6. ⭐ Update happiness
            //    Increase happiness if health > 50
            if (avatar.IntHealth > 50)
            {
                avatar.IntHappiness += 2;  // adjust this amount however you want
                avatar.IntHappiness = Math.Min(100, avatar.IntHappiness);
            }

            _db.Tavatars.Update(avatar);

            await _db.SaveChangesAsync();

            // 7. Return inventory + updated stats
            var inventory = await _db.TavatarItems
                .Where(ai => ai.IntAvatarId == avatar.IntAvatarId)
                .Join(_db.Titems,
                    ai => ai.IntItemId,
                    i => i.IntItemId,
                    (ai, i) => new {
                        id = i.IntItemId,
                        name = i.StrItem,
                        slug = i.StrSlug,
                        quantity = ai.IntQuantity
                    })
                .ToListAsync();

            return Ok(new
            {
                items = inventory,
                health = avatar.IntHealth,
                happiness = avatar.IntHappiness
            });
        }


    }
}
