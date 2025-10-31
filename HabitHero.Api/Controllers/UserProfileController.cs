using HabitHero.Core.Entities;
using HabitHero.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HabitHero.Api.Controllers 
{
	[ApiController]
	[Route("api/userprofile")]
	public class UserProfileController : ControllerBase 
	{
		private readonly HabitHeroDbContext _db;
		public UserProfileController(HabitHeroDbContext db) => _db = db;

		// ---------------------------------------------------------------------
		// GET: profile basics for a user
		// ---------------------------------------------------------------------
		[HttpGet("/getProfile/{userId:int}")]
		public async Task<IActionResult> GetProfile([FromRoute] int userId) 
		{
			var user = await _db.Tusers
				.AsNoTracking()
				.Where(u => u.IntUserId == userId)
				.Select(u => new {
					u.IntUserId,
					u.StrUsername,
					u.StrEmail,
					u.StrPassword,
					u.DecPoints,
					u.MonCash,
					u.IntAvatarId,
					u.IntAppRestrictionId
				})
				.FirstOrDefaultAsync();

			if (user == null)
				return NotFound(new { message = "User not found." });

			return Ok(user);
		}

		// ---------------------------------------------------------------------
		// GET: update username and email
		// ---------------------------------------------------------------------
		[HttpGet("/updateUsername/{userId:int}/{username}/{email}")]
		public async Task<IActionResult> UpdateAccount([FromRoute] int userId,[FromRoute] string username,[FromRoute] string email)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var user = await _db.Tusers.FirstOrDefaultAsync(u => u.IntUserId == userId);
			if (user == null)
				return NotFound(new { message = "User not found." });

			var emailExists = await _db.Tusers
				.AnyAsync(u => u.StrEmail == email && u.IntUserId != userId);
			if (emailExists)
				return Conflict(new { message = "Email already in use. Please try again." });

			user.StrUsername = username;
			user.StrEmail = email;

			await _db.SaveChangesAsync();
			return Ok(new { message = "Account updated.", userId = user.IntUserId });
		}

		// ---------------------------------------------------------------------
		// GET: change password
		// ---------------------------------------------------------------------
		[HttpGet("/updatePassword/{userId:int}/{oldPassword}/{newPassword}/{confirmPassword}")]
		public async Task<IActionResult> ChangePassword([FromRoute] int userId,[FromRoute] string oldPassword,[FromRoute] string newPassword,[FromRoute] string confirmPassword) 
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var user = await _db.Tusers.FirstOrDefaultAsync(u => u.IntUserId == userId);
			if (user == null)
				return NotFound(new { message = "User not found." });

			if (user.StrPassword != oldPassword)
				return Unauthorized(new { message = "Old password is incorrect." });

			if (newPassword != confirmPassword)
				return Conflict(new { message = "Passwords do not match. Please try again." });

			user.StrPassword = newPassword;
			await _db.SaveChangesAsync();

			return Ok(new { message = "Password changed successfully." });
		}

		// ---------------------------------------------------------------------
		// POST: set/clear app restriction 
		// ---------------------------------------------------------------------
		//[HttpPost("restriction/{userId:int}/{appRestrictionId:int}")]
		//public async Task<IActionResult> SetAppRestriction(
		//	[FromRoute] int userId,
		//	[FromRoute] int appRestrictionId) {
		//	var user = await _db.Tusers.FirstOrDefaultAsync(u => u.IntUserId == userId);
		//	if (user == null)
		//		return NotFound(new { message = "User not found." });

		//	if (appRestrictionId == 0) {
		//		user.IntAppRestrictionId = null;
		//	}
		//	else {
		//		var exists = await _db.TappRestrictions
		//			.AnyAsync(a => a.IntAppRestrictionId == appRestrictionId);
		//		if (!exists)
		//			return NotFound(new { message = "App restriction not found." });

		//		user.IntAppRestrictionId = appRestrictionId;
		//	}

		//	await _db.SaveChangesAsync();
		//	return Ok(new {
		//		message = "Restriction updated.",
		//		userId = user.IntUserId,
		//		appRestrictionId = user.IntAppRestrictionId
		//	});
		//}

		// ---------------------------------------------------------------------
		// GET: delete an account (and related data) after password check
		// ---------------------------------------------------------------------
		[HttpGet("deleteAccount/{userId:int}/{password}")]
		public async Task<IActionResult> DeleteAccount([FromRoute] int userId,[FromRoute] string password) 
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var user = await _db.Tusers.FirstOrDefaultAsync(u => u.IntUserId == userId);
			if (user == null)
				return NotFound(new { message = "User not found." });

			// simple password check to match your current pattern
			if (user.StrPassword != password)
				return Unauthorized(new { message = "Password is incorrect." });

			using var tx = await _db.Database.BeginTransactionAsync();
			try 
			{
				//// 1) Habit occurrences -> for this user's habits
				//var habitIds = await _db.Thabits
				//	.Where(h => h.IntUserId == userId)
				//	.Select(h => h.IntHabitId)
				//	.ToListAsync();

				//if (habitIds.Count > 0) {
				//	var occurrences = await _db.ThabitOccurrences
				//		.Where(o => habitIds.Contains(o.IntHabitId))
				//		.ToListAsync();
				//	if (occurrences.Count > 0) {
				//		_db.ThabitOccurrences.RemoveRange(occurrences);
				//		await _db.SaveChangesAsync();
				//	}
				//}

				//// 2) Habits
				//var habits = await _db.Thabits.Where(h => h.IntUserId == userId).ToListAsync();
				//if (habits.Count > 0) {
				//	_db.Thabits.RemoveRange(habits);
				//	await _db.SaveChangesAsync();
				//}

				//// 3) User achievements
				//var userAchievements = await _db.TuserAchievements
				//	.Where(ua => ua.IntUserId == userId)
				//	.ToListAsync();
				//if (userAchievements.Count > 0) {
				//	_db.TuserAchievements.RemoveRange(userAchievements);
				//	await _db.SaveChangesAsync();
				//}

				//// 4) User quests
				//var userQuests = await _db.TuserQuests
				//	.Where(uq => uq.IntUserId == userId)
				//	.ToListAsync();
				//if (userQuests.Count > 0) {
				//	_db.TuserQuests.RemoveRange(userQuests);
				//	await _db.SaveChangesAsync();
				//}

				// 5) Finally, delete the user
				_db.Tusers.Remove(user);
				await _db.SaveChangesAsync();

				await tx.CommitAsync();
				return Ok(new { message = "Account permanently deleted.", userId });
			}
			catch {
				await tx.RollbackAsync();
				return StatusCode(500, new { message = "Failed to delete account. Please try again." });
			}
		}

	}
}
