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
        public async Task<IActionResult> UpdateInventory([FromRoute] int userId, [FromRoute] string itemSlug)
        {
            // 1. Verify user exists and get their avatar ID
            var user = await _db.Tusers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IntUserId == userId);

            if (user == null)
                return NotFound(new { message = $"User with ID {userId} not found." });

            if (user.IntAvatarId == null)
                return NotFound(new { message = "User does not have an assigned avatar." });

            var avatarId = user.IntAvatarId.Value;

            // 2. Get the item by slug
            var item = await _db.Titems
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.StrSlug == itemSlug);

            if (item == null)
                return NotFound(new { message = $"Item with slug '{itemSlug}' not found." });

            // 3. Get the avatar's inventory entry
            var avatarItem = await _db.TavatarItems
                .FirstOrDefaultAsync(ai => ai.IntAvatarId == avatarId && ai.IntItemId == item.IntItemId);

            if (avatarItem == null)
                return NotFound(new { message = $"No '{itemSlug}' found in avatar inventory." });

            // 4. Remove one quantity, or delete the row if it reaches zero
            avatarItem.IntQuantity -= 1;

            if (avatarItem.IntQuantity <= 0)
                _db.TavatarItems.Remove(avatarItem);
            else
                _db.TavatarItems.Update(avatarItem);

            await _db.SaveChangesAsync();

            // 5. Return updated inventory for this avatar
            var inventory = await _db.TavatarItems
                .Where(ai => ai.IntAvatarId == avatarId)
                .Join(_db.Titems,
                    ai => ai.IntItemId,
                    i => i.IntItemId,
                    (ai, i) => new
                    {
                        id = i.IntItemId,
                        name = i.StrItem,
                        slug = i.StrSlug,
                        quantity = ai.IntQuantity
                    })
                .ToListAsync();

            return Ok(new { items = inventory });
        }

    }
}
