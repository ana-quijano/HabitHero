using HabitHero.Core.Models.Ai;
using HabitHero.Core.Services.Ai;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HabitHero.Infrastructure.Services.Ai
{
    public class GptService : IGptService
    {
        private readonly string _apiKey;

        public GptService(IConfiguration configuration)
        {
            _apiKey = configuration["OpenAI:ApiKey"]
                      ?? throw new InvalidOperationException("OpenAI API key not configured.");
        }


        public async Task<HabitSuggestionsPayload> GenerateHabitsAsync(string goal, CancellationToken ct = default)
        {
            using var http = new System.Net.Http.HttpClient();
            http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);


            var systemPrompt =
                "You generate concise, actionable daily habit ideas from a user's goal. " +
                "All habits must be designed for daily practice. Each should be atomic, specific, measurable, " +
                "and feasible within about 30 minutes unless user constraints specify otherwise. " +
                "Each habit must describe what to do, how to measure it, and why it helps." +
                "Avoid overcomplex or vague habits. Maintain a straight-to-the-point, simple, and encouraging tone. Mirror the user’s locale and measurement units in outputs." +
                "Return STRICT JSON with this schema: " +
                "{ \"Parsed_Goal\": string, \"Habits\": [ { \"StrHabit\": string, \"StrDescription\": string } ] }. " +
                "Use 5-8 habits, 1-2 short sentences per description (not more than 255 characters)";

            var body = new
            {
                model = "gpt-4.1-mini",
                response_format = new { type = "json_object" },
                messages = new object[]
                {
            new { role = "system", content = systemPrompt },
            new { role = "user", content = $"User goal: {goal}" }
                },
                temperature = 0.7
            };

            var json = System.Text.Json.JsonSerializer.Serialize(
                body,
                new System.Text.Json.JsonSerializerOptions { PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase });

            var resp = await http.PostAsync(
                "https://api.openai.com/v1/chat/completions",
                new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json"),
                ct);

            var respText = await resp.Content.ReadAsStringAsync(ct);
            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"OpenAI error {resp.StatusCode}: {respText}");

            using var doc = System.Text.Json.JsonDocument.Parse(respText);
            var content = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            if (string.IsNullOrWhiteSpace(content))
                throw new InvalidOperationException("OpenAI returned empty content.");

            var payload = System.Text.Json.JsonSerializer.Deserialize<HabitSuggestionsPayload>(content)
                          ?? throw new InvalidOperationException("Failed to parse AI JSON payload.");

            // quick cleanup
            payload.Parsed_Goal = payload.Parsed_Goal?.Trim() ?? "";
            payload.Habits = payload.Habits?.FindAll(h => !string.IsNullOrWhiteSpace(h.StrHabit)) ?? new();
            return payload;
        }
    }
}