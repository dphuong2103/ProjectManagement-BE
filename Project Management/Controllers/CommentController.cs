
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Management.Database;
using Project_Management.Models.DatabaseModel;
using Microsoft.AspNetCore.Authorization;
namespace Project_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<CommentController> _logger;

        public CommentController(DatabaseContext context, ILogger<CommentController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Comment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComment()
        {
            if (_context.Comment == null)
            {
                return NotFound();
            }
            return await _context.Comment.ToListAsync();
        }

        // GET: api/Comment/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(string id)
        {
            if (_context.Comment == null)
            {
                return NotFound();
            }
            var comment = await _context.Comment.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            return comment;
        }

        [HttpGet("task/{taskId}")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentsByTaskId(string taskId)
        {
            if (_context.Comment == null)
            {
                return NotFound();
            }
            try
            {
                var comments = await (from comment in _context.Comment.Include(comment => comment.Creator)
                                      where comment.TaskID == taskId
                                      select comment).ToListAsync();
                return Ok(comments);

            }catch (Exception err)
            {
                _logger.LogError("Error getting comments: ", err);
                return Problem("Error getting comments: ", err.ToString());
            }
     
        }

        // POST: api/Comment
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Comment>> UpdateCommentDetails(Comment comment)

        {
            if (_context.Task == null)
            {
                return Problem("Entity set 'DatabaseContext.Task'  is null.");
            }
            try
            {
                if (comment.ID is not null && CommentExists(comment.ID))
                {
                    _context.Entry(comment).State = EntityState.Modified;
                }
                else
                {
                    await _context.Comment.AddAsync(comment);
                }
                await _context.SaveChangesAsync();
                var updatedComment = _context.Comment.Include(cm => cm.Creator).Where(cm => cm.ID == comment.ID).FirstOrDefault();

                return Ok(updatedComment);
            }
            catch (Exception err)
            {
                _logger.LogError("Error creating comment: ", err);
                return BadRequest(err.Message);
            }
        }

        // DELETE: api/Comment/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(string id)
        {
            if (_context.Comment == null)
            {
                return NotFound();
            }
            var comment = await _context.Comment.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comment.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CommentExists(string id)
        {
            return (_context.Comment?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
