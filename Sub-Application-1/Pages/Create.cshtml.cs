using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using server.Data;
using server.Models;
using System.Threading.Tasks;

public class CreateModel : PageModel
{
    private readonly AppDbContext _context;

    [BindProperty]
    public Post Post { get; set; }

    public CreateModel(AppDbContext context)
    {
        _context = context;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        Post.DateUploaded = DateTime.UtcNow; // Set upload time
        _context.Posts.Add(Post);
        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}
