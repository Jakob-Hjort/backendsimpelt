using GetEFWorking.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers (så swagger finder dine Controllers/*Controller.cs)
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core DbContext (du har OnConfiguring i QueueContext, så det her er nok)
builder.Services.AddDbContext<QueueContext>();

var app = builder.Build();



// Swagger UI i development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map controllers endpoints
app.MapControllers();

app.Run();