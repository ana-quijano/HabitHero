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



        /// <summary>
        /// POST: CREATE NEW HABIT
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="strHabit"></param>
        /// <param name="strDescription"></param>
        /// <returns>Updated list of habits</returns>
        //[HttpGet("api/habits/{userId}/{strHabit}/{strDescription}")]
        //public async Task<IActionResult> CreateHabit([FromRoute] int userId, [FromRoute] string strHabit, [FromRoute] string strDescription)
        //{
        //    if (!ModelState.IsValid) return BadRequest(ModelState);

        //    var userExists = await _db.Tusers
        //        .AsNoTracking()
        //        .AnyAsync(u => u.IntUserId == userId);

        //    if (!userExists)
        //        return NotFound(new { message = "User not found." });

        //    var newHabit = new Thabit
        //    {
        //        IntUserId = userId,
        //        StrHabit = strHabit,
        //        IntScheduleId = 1,
        //        StrDescription = strDescription
        //    };

        //    _db.Thabits.Add(newHabit);
        //    await _db.SaveChangesAsync();

        //    var updatedHabits = await _db.Thabits
        //        .AsNoTracking()
        //        .Where(h => h.IntUserId == userId)
        //        .Select(h => new
        //        {
        //            h.IntHabitId,
        //            h.StrHabit,
        //            h.IntScheduleId,
        //            h.StrDescription
        //        })
        //        .ToListAsync();

        //    return Ok(updatedHabits);
        //}
    }
}
