using server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PostsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Posts
        // Henter alle innlegg
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            var posts = await _context.Posts.Include(p => p.User).ToListAsync();
            return Ok(posts);
        }

        // GET: api/Posts/{id}
        // Henter et enkelt innlegg basert på ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var post = await _context.Posts.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            return Ok(post);
        }

        // POST: api/Posts
        // Oppretter et nytt innlegg
        [HttpPost]
        public async Task<ActionResult<Post>> CreatePost([FromBody] Post newPost)
        {
            // Validerer at brukeren som lager innlegget finnes
            var user = await _context.Users.FindAsync(newPost.UserId);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            // Oppretter innlegget
            _context.Posts.Add(newPost);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPost), new { id = newPost.Id }, newPost);
        }

        // PUT: api/Posts/{id}
        // Oppdaterer et innlegg basert på ID
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] Post updatedPost)
        {
            if (id != updatedPost.Id)
            {
                return BadRequest("Post ID mismatch");
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            // Oppdaterer felt
            post.Caption = updatedPost.Caption;
            post.ImageUrl = updatedPost.ImageUrl;

            // Lagrer oppdateringene
            _context.Entry(post).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Posts.Any(p => p.Id == id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Posts/{id}
        // Sletter et innlegg basert på ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            // Fjerner innlegget fra databasen
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
