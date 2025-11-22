using HabitHero.Core.Entities;
using HabitHero.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HabitHero.Api.Controllers
{
    public class ShopController : Controller
    {
        private readonly HabitHeroDbContext _db;
        public ShopController(HabitHeroDbContext db) => _db = db;
        public class PurchaseItemDto
        {
            public int Id { get; set; }     // item id
            public int Quantity { get; set; }
        }
        public class PurchaseRequest
        {
            public int UserId { get; set; } // optionally include in request body
            public List<PurchaseItemDto> Items { get; set; } = new();
        }
        [HttpPost("api/shop/purchase")]
        public async Task<IActionResult> Purchase([FromBody] PurchaseRequest request)
        {
            // 1️⃣ Verify user
            var user = await _db.Tusers
                .Include(u => u.Tavatar)
                .FirstOrDefaultAsync(u => u.IntUserId == request.UserId);

            if (user == null)
                return NotFound(new { message = "User not found." });

            if (user.IntAvatarId == null)
                return BadRequest(new { message = "User has no avatar assigned." });

            var avatarId = user.IntAvatarId.Value;

            // 2️⃣ Calculate total cost
            var itemIds = request.Items.Select(i => i.Id).ToList();
            var items = await _db.Titems
                .Where(i => itemIds.Contains(i.IntItemId))
                .ToListAsync();

            decimal totalCost = 0;
            foreach (var p in request.Items)
            {
                var dbItem = items.FirstOrDefault(i => i.IntItemId == p.Id);
                if (dbItem == null)
                    return BadRequest(new { message = $"Item with id {p.Id} not found." });

                totalCost += (dbItem.IntPrice) * p.Quantity;
            }

            // Assume user points stored in user.IntPoints or similar column
            if (user.IntPoints < totalCost)
                return BadRequest(new { message = "Not enough points." });

            // 3️⃣ Deduct points
            user.IntPoints -= (int)totalCost;
            _db.Tusers.Update(user);

            // 4️⃣ Add / update avatar inventory
            foreach (var p in request.Items)
            {
                var existing = await _db.TavatarItems
                    .FirstOrDefaultAsync(ai => ai.IntAvatarId == avatarId && ai.IntItemId == p.Id);

                if (existing != null)
                {
                    existing.IntQuantity += p.Quantity;
                    _db.TavatarItems.Update(existing);
                }
                else
                {
                    _db.TavatarItems.Add(new TavatarItem
                    {
                        IntAvatarId = avatarId,
                        IntItemId = p.Id,
                        IntQuantity = p.Quantity
                    });
                }
            }

            await _db.SaveChangesAsync();

            // 5️⃣ Return updated inventory + points
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

            return Ok(new
            {
                message = "Purchase successful",
                points = user.IntPoints,
                items = inventory
            });
        }
    }
}
