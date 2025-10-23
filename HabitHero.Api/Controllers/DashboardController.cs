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
        [HttpPost("signup/{username}/{email}/{password}/{confirmpassword}")]
        public async Task<IActionResult> Signup([FromRoute] string username, [FromRoute] string email,[FromRoute] string password, [FromRoute] string confirmpassword)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var emailExists = await _db.Tusers.AnyAsync(u => u.StrEmail == email);

            if (emailExists)
            {
                return Conflict(new { message = "Email already in use. Please try again." });
            };

            if (password != confirmpassword)
            { 
                return Conflict(new { message = "Passwords do not match. Please try again." });
            };

            var user = new Tuser
            {
                StrUsername = username,
                StrEmail = email,
                StrPassword = password
            };

            _db.Tusers.Add(user);
            await _db.SaveChangesAsync();

            var userId = user.IntUserId;

            return Ok($"New account for '{username}' with User ID 'userId' successfully created.");
        }

        /// <summary>
        /// GET: USER ID FOR LOG IN
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpGet("login/{username}/{password}")]
        public async Task<IActionResult> Login([FromRoute] string username, [FromRoute] string password)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _db.Tusers
                .FirstOrDefaultAsync(u => u.StrUsername == username
                                    && u.StrPassword == password);

            if (user == null)
            {
                return Unauthorized(new 
                    { message = "Invalid username or password." });
            }

            var userId = user.IntUserId;

            return Ok($"Successfully logged in user with ID {userId}");

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
                user.IntAppRestrictionId
            });
        }
    }
}

