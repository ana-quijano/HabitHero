using Azure.Core;
using HabitHero.Core.Entities;
using HabitHero.Core.Models.Auth;
using HabitHero.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HabitHero.Api.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly HabitHeroDbContext _db;
        public DashboardController(HabitHeroDbContext db) => _db = db;

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var exists = await _db.Tusers.AnyAsync(u => u.StrEmail == request.StrEmail);

            if (exists)
            {
                return Conflict(new { message = "Email already in use. Please try again." });
            };

            if (request.StrConfirmPassword != request.StrPassword)
            { 
                return Conflict(new { message = "Passwords do not match. Please try again." });
            };

            var user = new Tuser
            {
                StrUsername = request.StrUsername,
                StrEmail = request.StrEmail,
                StrPassword = request.StrPassword,
            };

            _db.Tusers.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new AuthResponse
            {
                ObjUser = user,
                StrAccessToken = string.Empty,
                DtmExpiresAt = System.DateTime.UtcNow
            });
        }

        /// POST ACTION: LOGIN (AUTHENTICATE)
        /// <summary>
        /// Authenticates a user by verifying their username and password.
        /// </summary>
        /// <param name="request">Contains the username and password submitted by the user.</param>
        /// <returns>
        /// Returns the user’s <c>IntUserId</c> if the credentials are valid.
        /// Returns <c>401 Unauthorized</c> if the credentials do not match any record.
        /// </returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _db.Tusers
                .FirstOrDefaultAsync(u => u.StrUsername == request.StrUsername
                                    && u.StrPassword == request.StrPassword);

            if (user == null)
            {
                return Unauthorized(new 
                    { message = "Invalid username or password." });
            }

            return Ok(new
                { IntUserId = user.IntUserId });

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
                user.DecPoints,
                user.MonCash,
                user.BlnAppRestriction
            });
        }
    }
}

