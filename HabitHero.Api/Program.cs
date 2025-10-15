using Microsoft.EntityFrameworkCore;
using HabitHero.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var conn = builder.Configuration.GetConnectionString("DefaultConnection"); // Set in secrets.json
builder.Services.AddDbContext<HabitHeroDbContext>(o => o.UseSqlServer(conn));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
