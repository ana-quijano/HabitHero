using Microsoft.EntityFrameworkCore;
using HabitHero.Infrastructure.Data;
using Microsoft.Data.SqlClient; 


var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = new[] {
    "http://localhost:19006", // Expo web (Metro)
    "http://127.0.0.1:19006",
    "http://localhost:5173",  // Vite (if you use it later)
    "http://localhost:3000",  // Common dev port
    "https://habitheroapi-bdcbazebb5czbkc5.centralus-01.azurewebsites.net" // Deployed API
};
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowExpo",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

//var db = "dbServerConnection";
//var local = "DefaultConnection";        // LOCAL
var local = "dbServerConnection";     // SERVER

var conn = builder.Configuration.GetConnectionString(local); // Set in secrets.json
builder.Services.AddDbContext<HabitHeroDbContext>(o => o.UseSqlServer(conn));

var csb = new SqlConnectionStringBuilder(conn);
Console.WriteLine($"[DB CHECK] Using '{local}' → Server={csb.DataSource}; Database={csb.InitialCatalog}");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.Urls.Clear();
    app.Urls.Add("http://0.0.0.0:7098");   // reachable from your phone
    app.Urls.Add("http://localhost:7098");
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowExpo");
app.MapControllers();

app.Run();
