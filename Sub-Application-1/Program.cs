using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BCrypt.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages(); // Add Razor Pages services
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));



// Add authorization
builder.Services.AddAuthorization();

var app = builder.Build();



app.UseHttpsRedirection();
app.UseStaticFiles(); // Enable serving static files
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages(); // Map Razor Pages

app.Run();
