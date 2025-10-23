using HabitHero.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace HabitHero.Api.Controllers {
	public class UserProfileController : Controller {

		private readonly HabitHeroDbContext _db;
		public UserProfileController(HabitHeroDbContext db) => _db = db;
		public IActionResult Index() {
			return View();
		}
	}
}
