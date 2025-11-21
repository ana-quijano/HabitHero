using HabitHero.Core.Entities;
using HabitHero.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HabitHero.Api.Controllers {
	[ApiController]
	[Route("api/avatar")]
	public class AvatarNameController : ControllerBase {
		private readonly HabitHeroDbContext _db;

		public AvatarNameController(HabitHeroDbContext db) => _db = db;

		// Pool of names by avatar type/key (these should match what you use on the UI)
		private static readonly Dictionary<string, string[]> AvatarNamePool =
			new(StringComparer.OrdinalIgnoreCase) 
			{
				["Dragon"] = new[]
				{
					"Ember", "Blaze", "Smolder", "Nova", "Storm",
					"Inferno", "Pyro", "Draco", "Ash", "Shadow"
				},
				["Dinosaur"] = new[]
				{
					"Rex", "Spike", "Chomper", "Fossil", "Crunch",
					"Raptor", "Stomper", "Boulder", "Tiny", "Roary"
				},
				["Turtle"] = new[]
				{
					"Shelly", "Turbo", "Myrtle", "Snap", "Pebble",
					"Honu", "Drift", "SlowMo", "Tide", "Puddle"
				},
				["Fox"] = new[]
				{
					"Rusty", "Vixen", "Swift", "Copper", "Comet",
					"Flick", "Sly", "Flare", "Maple", "Rust"
				},
				["Lion"] = new[]
				{
					"Leo", "Simba", "Nova", "Roar", "Mane",
					"Crown", "Regal", "Brave", "Sahara", "Kovu"
				},
				["Panda"] = new[]
				{
					"Bamboo", "Puff", "Bao", "Mochi", "PandaPop",
					"Yuan", "Cookie", "Oreo", "Biscuit", "Chonky"
				},
				["Chinchilla"] = new[]
				{
					"Chilli", "Fuzz", "Nimbus", "Pebbles", "Dusty",
					"Fluff", "Puffball", "Cloud", "Nugget", "Sprout"
				},
				["Penguin"] = new[]
				{
					"Waddles", "Pip", "Iceberg", "Tux", "Pebble",
					"Frost", "Pingu", "Flipper", "Chill", "Blizzard"
				},
				["Axolotl"] = new[]
				{
					"Bubbles", "Gummy", "Lotl", "Sprinkle", "Marsh",
					"Noodle", "Squish", "Pinky", "Ripple", "Beans"
				},
				["Octopus"] = new[]
				{
					"Inky", "Suction", "Wiggle", "Coral", "Squish",
					"Tentacle", "Echo", "Bubble", "Kraken Jr", "Tango"
				},
				["Tiger"] = new[]
				{
					"Stripe", "Blaze", "Fang", "Karma", "Shadow",
					"Nova", "Zest", "Roar", "Saffron", "Hunter"
				},
				["Blob"] = new[]
				{
					"Goo", "Jelly", "Melt", "Squish", "Pudding",
					"Blobbo", "Sploot", "Gloop", "Slimey", "Boop"
				}
			};

		/// <summary>
		/// When the user selects an avatar, call this to get 5 random name suggestions.
		/// Example: GET api/avatar/Dragon/names?count=5
		/// </summary>
		[HttpGet("{avatarKey}/names")]
		public IActionResult GetAvatarNamesForSingle(
			[FromRoute] string avatarKey,
			[FromQuery] int count = 5) {
			if (!AvatarNamePool.TryGetValue(avatarKey, out var pool)) {
				return NotFound(new { message = "Unknown avatar type." });
			}

			if (count <= 0)
				count = 5;

			var rng = new Random();

			var names = pool
				.OrderBy(_ => rng.Next())                    // shuffle
				.Take(Math.Min(count, pool.Length))          // take up to 5
				.ToArray();

			return Ok(new {
				avatar = avatarKey,
				suggestions = names
			});
		}
	}
}
