using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BCrypt.Net;

// Set up the builder
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Handle potential circular references
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Configure SQLite database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure authentication and JWT Bearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false, // Set to true and specify valid issuer in production
        ValidateAudience = false, // Set to true and specify valid audience in production
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero, // Remove default 5-minute clock skew
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])) // Replace with your secret key
    };
});

//Cors for å tillate react client til å sende ajax kall
builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowReactClient",
                policy => policy.WithOrigins("http://localhost:3000")
                                .AllowAnyHeader()
                                .AllowAnyMethod());
        });

// Add authorization
builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();
//Cors for å tillate react client til å sende ajax kall
app.UseCors("AllowReactClient");
// Seed the database with a sample user and two posts
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Automatically apply any pending migrations (for simplicity in testing)
    dbContext.Database.Migrate();

    // Check if there are any users in the database
    if (!dbContext.Users.Any())
    {
        // Add a sample user
        var sampleUser = new User
        {
            Username = "TestUser",
            Email = "testuser@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            ProfilePictureUrl = "https://picsum.photos/seed/1/200",
            Bio = "This is a sample bio for TestUser, an avid user of the platform.",
            DateJoined = DateTime.UtcNow

        };

        dbContext.Users.Add(sampleUser);
        dbContext.SaveChanges();

        // Add two posts for the sample user
        var random = new Random();
        var post1 = new Post
        {
            //ImageUrl = "https://picsum.photos/seed/1/600",  // Example image URL
            DateUploaded = DateTime.UtcNow.AddDays(-random.Next(0, 365 * 5)),
            UserId = sampleUser.UserId // Link the post to the TestUser
        };

        var post2 = new Post
        {
            //ImageUrl = "https://picsum.photos/seed/2/600",  // Example image URL
            DateUploaded = DateTime.UtcNow.AddDays(-random.Next(0, 365 * 5)),
            UserId = sampleUser.UserId // Link the post to the TestUser
        };

        dbContext.Posts.AddRange(post1, post2);
        dbContext.SaveChanges();
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles();

app.Run();