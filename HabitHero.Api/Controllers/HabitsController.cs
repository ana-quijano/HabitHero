using HabitHero.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HabitHero.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HabitsController : ControllerBase
{
    private readonly HabitHeroDbContext _db;
    public HabitsController(HabitHeroDbContext db) => _db = db;

    // Simple: raw rows
    [HttpGet]
    public async Task<IActionResult> Get() =>
        Ok(await _db.Habits.AsNoTracking().ToListAsync());

    // Rich: includes user name + schedule name
    [HttpGet("detailed")]
    public async Task<IActionResult> GetDetailed()
    {
        var list = await _db.Habits
            .AsNoTracking()
            .Include(h => h.User)
            .Include(h => h.Schedule)
            .Select(h => new
            {
                HabitId = h.Id,
                Habit = h.Name,
                Description = h.Description,
                UserId = h.UserId,
                UserName = h.User != null ? h.User.UserName : null,
                ScheduleId = h.ScheduleId,
                Schedule = h.Schedule != null ? h.Schedule.Name : null,
                StartDate = h.StartDate,
                EndDate = h.EndDate,
                ReminderTime = h.ReminderTime
            })
            .ToListAsync();

        return Ok(list);
    }

    [HttpGet("debug/db")]
    public async Task<IActionResult> DebugDb([FromServices] HabitHeroDbContext db)
    {
        try
        {
            var canConnect = await db.Database.CanConnectAsync();
            var habitCount = await db.Habits.CountAsync();
            return Ok(new { canConnect, habitCount });
        }
        catch (Exception ex)
        {
            // TEMP: return full exception so we see what's wrong
            return Problem(title: "DB error", detail: ex.ToString());
        }
    }
}
