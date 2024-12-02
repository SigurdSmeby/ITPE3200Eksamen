using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BCrypt.Net;
using Serilog;
using Serilog.Events;


// Configure application builder and add necessary services
var builder = WebApplication.CreateBuilder(args);

// Add Logger Configuration
var loggerConfiguration = new LoggerConfiguration()
	.MinimumLevel.Debug()
	.WriteTo.File($"Logs/app_{DateTime.Now:yyyyMMdd_HHmmss}.log");

Log.Logger = loggerConfiguration.CreateLogger();

builder.Logging.AddSerilog(Log.Logger);

builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
	});

// Set up SQLite database connection
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure authentication and JWT Bearer tokens
builder.Services.AddAuthentication(options =>
{
	//Authentication
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	//JWT Bearer tokens
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = false,
		ValidateAudience = false,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ClockSkew = TimeSpan.Zero,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is missing.")))
	};
});

// Enable CORS to allow requests from the React client
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowReactClient", policy =>
		policy.WithOrigins("http://localhost:3000")
		.AllowAnyHeader()
		.AllowAnyMethod());
});

// Add authorization support
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors("AllowReactClient");


// Set up database with seed data
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var context = services.GetRequiredService<AppDbContext>();
	context.Database.EnsureDeleted();
	context.Database.EnsureCreated();
	SeedData.Initialize(context);
}

// Configure routes and endpoints
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles();
app.Run();
