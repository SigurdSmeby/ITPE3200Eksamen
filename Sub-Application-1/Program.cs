using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Sub_Application_1.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Sub_Application_1.Models;
using Sub_Application_1.Repositories;
using Sub_Application_1.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDefaultIdentity<User>().AddEntityFrameworkStores<AppDbContext>();

// Configure Entity Framework Core with SQLite
builder.Services.AddSession();



builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// adding repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ILikeRepository, LikeRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Configure middleware for handling HTTP requests in the application.
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();   
app.MapDefaultControllerRoute();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
    SeedData.Initialize(context);
}

app.Run();