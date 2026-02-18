using FleetAnalytics.Application.Interfaces;
using FleetAnalytics.Application.Services;
using FleetAnalytics.Domain.Interfaces;
using FleetAnalytics.Infrastructure.Data;
using FleetAnalytics.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<FleetDbContext>(options =>
    options.UseSqlite("Data Source=fleet.db"));

// Repositories (Infrastructure)
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<ITripLogRepository, TripLogRepository>();
builder.Services.AddScoped<IAlertRepository, AlertRepository>();
builder.Services.AddScoped<IDriverRepository, DriverRepository>();

// Services (Application)
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<ITripLogService, TripLogService>();
builder.Services.AddScoped<IDriverService, DriverService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
