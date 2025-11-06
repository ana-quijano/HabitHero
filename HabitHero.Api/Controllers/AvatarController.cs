using HabitHero.Core.Entities;
using HabitHero.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HabitHero.Api.Controllers
{
    public class AvatarConroller : Controller
    {
        private readonly HabitHeroDbContext _db;
        public AvatarConroller(HabitHeroDbContext db) => _db = db;

        /// <summary>
        /// GET: User Inventory
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("api/avatar/getInventory/{userId}")]
        public async Task<IActionResult> GetInventory([FromRoute] int userId)
        {
            // 1. Check if user exists
            var userExists = await _db.Tusers
                .AsNoTracking()
                .AnyAsync(u => u.IntUserId == userId);

            if (!userExists)
                return NotFound(new { message = "User not found." });

            // 2️. Join TUsers → TAvatars → TAvatarItems → TItems
            var items = await _db.Tusers
                .AsNoTracking()
                .Where(u => u.IntUserId == userId)
                .Join(_db.Tavatars,
                    user => user.IntAvatarId,
                    avatar => avatar.IntAvatarId,
                    (user, avatar) => new { user, avatar })
                .Join(_db.TavatarItems,
                    ua => ua.avatar.IntAvatarId,
                    avatarItem => avatarItem.IntAvatarId,
                    (ua, avatarItem) => new { ua.user, ua.avatar, avatarItem })
                .Join(_db.Titems,
                    uaai => uaai.avatarItem.IntItemId,
                    item => item.IntItemId,
                    (uaai, item) => new
                    {
                        id = item.IntItemId,
                        name = item.StrItem,
                        slug = !string.IsNullOrWhiteSpace(item.StrSlug)
                            ? item.StrSlug
                            : item.StrItem
                                .Trim()
                                .ToLower()
                                .Replace(" ", "_")
                                .Replace("-", "_"),
                        quantity = uaai.avatarItem.IntQuantity
                    })
                .ToListAsync();

            // 3. Return results in desired structure
            return Ok(new
            {
                items
            });
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
