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
    public class UserProjectController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public UserProjectController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/UserProject
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserProject>>> GetUserProject()
        {
            if (_context.UserProject == null)
            {
                return NotFound();
            }
            return await _context.UserProject.ToListAsync();
        }

        // GET: api/UserProject/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserProject>> GetUserProject(string id)
        {
            if (_context.UserProject == null)
            {
                return NotFound();
            }
            var userProject = await _context.UserProject.FindAsync(id);

            if (userProject == null)
            {
                return NotFound();
            }

            return userProject;
        }

        // PUT: api/UserProject/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserProject(string id, UserProject userProject)
        {
            if (id != userProject.ID)
            {
                return BadRequest();
            }

            _context.Entry(userProject).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserProjectExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UserProject
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserProject>> UpdateUserProject(UserProject userProject)
        {
            if (_context.UserProject == null)
            {
                return Problem("Entity set 'DatabaseContext.UserProject'  is null.");
            }
            if (userProject.ID is not null && UserProjectExists(userProject.ID))
            {
                _context.Entry(userProject).State = EntityState.Modified;
            }
            else
            {
                _context.UserProject.Add(userProject);
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/UserProject/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserProject(string id)
        {
            if (_context.UserProject == null)
            {
                return NotFound();
            }
            var userProject = await _context.UserProject.FindAsync(id);
            if (userProject == null)
            {
                return NotFound();
            }

            _context.UserProject.Remove(userProject);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserProjectExists(string id)
        {
            return (_context.UserProject?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
