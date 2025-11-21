using HabitHero.Core.Entities;
using HabitHero.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HabitHero.Api.Controllers
{
	[ApiController]
	[Route("api/leaderboard")]
	public class LeaderboardController : ControllerBase 
	{
		private readonly HabitHeroDbContext _db;
		public LeaderboardController(HabitHeroDbContext db) => _db = db;

		// ---------------------------------------------------------------------
		// GET: top 10 users with the most DecPoints,
		// returned ASCENDING by points (if fewer than 10 users, return all).
		// ---------------------------------------------------------------------
		[HttpGet("/getTopUsers")]
		public async Task<IActionResult> GetTopUsers() 
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var topDescending = await _db.Tusers
				.AsNoTracking()
				.OrderByDescending(u => u.IntPoints)   // highest first
				.ThenBy(u => u.StrUsername)            // stable tie-breaker
				.Select(u => new {
					username = u.StrUsername,
					totalPoints = u.IntPoints
				})
				.Take(10)
				.ToListAsync();

			return Ok(topDescending);
		}
	}
}
