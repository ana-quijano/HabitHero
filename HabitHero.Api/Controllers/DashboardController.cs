using Azure.Core;
using HabitHero.Core.Entities;
using HabitHero.Core.Models.Auth;
using HabitHero.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace HabitHero.Api.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly HabitHeroDbContext _db;
        public DashboardController(HabitHeroDbContext db) => _db = db;

        /// <summary>
        /// POST: CREATE NEW USER (SIGN UP)
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="confirmpassword"></param>
        /// <returns></returns>
        [HttpGet("signup/{username}/{email}/{password}/{confirmpassword}")]
        public async Task<IActionResult> Signup([FromRoute] string username, [FromRoute] string email,[FromRoute] string password, [FromRoute] string confirmpassword)
        {
            
            var emailExists = await _db.Tusers.AnyAsync(u => u.StrEmail == email);

            if (emailExists)
            {
                return Conflict(new { message = "Email already in use. Please try again." });
            };

            if (password != confirmpassword)
            { 
                return Conflict(new { message = "Passwords do not match. Please try again." });
            };

            var avatar = new Tavatar
            {
                StrImageName = "blob.png",
                StrAvatarName = "My Pet Blob",
                IntHappiness = 70,
                IntHealth = 70
            };

            _db.Tavatars.Add(avatar);
            await _db.SaveChangesAsync();

            var user = new Tuser
            {
                StrUsername = username,
                StrEmail = email,
                StrPassword = password,
                IntAvatarId = avatar.IntAvatarId
            };

            _db.Tusers.Add(user);
            await _db.SaveChangesAsync();

            var userId = user.IntUserId;

            return Ok(new
            {
                intUserId = user.IntUserId,
                strUsername = user.StrUsername,
                strPassword = user.StrPassword,
                strEmail = user.StrEmail,
                intAvatarId = user.IntAvatarId
                
            });
        }

        /// <summary>
        /// POST: USER LOGIN
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("signin")]
        public async Task<IActionResult> Signin([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _db.Tusers
                .AsNoTracking()
                .FirstOrDefaultAsync(u =>
                    u.StrUsername == request.StrUsername &&
                    u.StrPassword == request.StrPassword);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            // Return user info in JSON so the app can use it
            return Ok(new
            {
                intUserId = user.IntUserId,
                strUsername = user.StrUsername,
                strEmail = user.StrEmail,
                intPoints = user.IntPoints
            });
        }

        /// GET ACTION: GET USER BY ID
        /// <summary>
        /// Retrieves detailed information for a specific user by their ID.
        /// </summary>
        /// <param name="id">The unique identifier (<c>IntUserId</c>) of the user.</param>
        /// <returns>
        /// Returns user details including username, email, avatar ID, and account data.
        /// Returns <c>404 Not Found</c> if no user exists with the provided ID.
        /// </returns>
        [HttpGet("user/{id:int}")]
        public async Task<IActionResult> GetUserById([FromRoute] int id)
        {
            var user = await _db.Tusers.FindAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found." });

            return Ok(new
            {
                user.IntUserId,
                user.StrUsername,
                user.StrEmail,
                user.IntAvatarId,
                user.IntPoints,
                user.IntTotalPoints,
                user.MonCash,
                user.IntAppRestrictionId
            });
        }

        /// <summary>
        /// Get User's Quest Invites
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("user/questinvites/{userId}")]
        public async Task<IActionResult> GetUserQuestInvites([FromRoute] int userId)
        {
            var user = await _db.Tusers.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var userInvites = await _db.TuserQuests
                .AsNoTracking()
                .Where(uq => uq.IntUserId == userId && uq.BlnAccepted == false)
                .ToListAsync();

            if (userInvites.Count > 0 )
            {
                return Ok(userInvites);

            }
            else
            {
                return Ok();
            }
        }

        /// <summary>
        /// User Accepts Quest Invite
        /// </summary>
        /// <param name="userQuestId"></param>
        /// <returns></returns>
        [HttpPost("user/acceptquest/{userQuestId}")]
        public async Task<IActionResult> AcceptQuestInvite([FromRoute] int userQuestId)
        {
            var userQuest = await _db.TuserQuests
                .FindAsync(userQuestId);
          
            if (userQuest != null)
            {
                //Mark user quest as accepted
                if (userQuest.BlnAccepted == false)
                {
                    userQuest.BlnAccepted = true;

                    await _db.SaveChangesAsync();

                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return NotFound();
            }

        }

        [HttpPost("api/users/registerpushtoken")]
        public async Task<IActionResult> RegisterPushToken([FromBody] RegisterPushTokenRequest request)
        {
            if (request.IntUserId <= 0 || string.IsNullOrWhiteSpace(request.StrPushToken))
            {
                return BadRequest("Invalid user or token.");
            }

            var user = await _db.Tusers.FirstOrDefaultAsync(u => u.IntUserId == request.IntUserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.StrPushToken = request.StrPushToken;
            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}

