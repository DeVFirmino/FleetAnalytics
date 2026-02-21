using FleetAnalytics.Application.Interfaces;
using FleetAnalytics.Application.Services;
using FleetAnalytics.Domain.Interfaces;
using FleetAnalytics.Infrastructure.Data;
using FleetAnalytics.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

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

// JWT Authentication
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"] ?? "superSecretKey1234567890123456789012");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "FleetAnalyticsApi",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "FleetAnalyticsUsers",
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

app.UseMiddleware<FleetAnalytics.Api.Middleware.GlobalExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
