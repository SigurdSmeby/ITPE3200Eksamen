using Microsoft.AspNetCore.Mvc.RazorPages;
using server.Data;
using server.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    // Define Posts property
    public List<Post> Posts { get; set; }

    public async Task OnGetAsync()
    {
        // Fetch posts from the database
        Posts = await _context.Posts
            .Include(p => p.User) // Include related User entity
            .Include(p => p.Likes) // Include related Likes
            .OrderByDescending(p => p.DateUploaded) // Sort by newest
            .ToListAsync();
    }
}
