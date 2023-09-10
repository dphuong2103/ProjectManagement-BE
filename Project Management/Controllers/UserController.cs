using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Management.Database;
using Project_Management.Models.DatabaseModel;
using Microsoft.AspNetCore.Authorization;
namespace Project_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<UserController> _logger;
        public UserController(DatabaseContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/User
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            if (_context.User == null)
            {
                return NotFound();
            }
            return await _context.User.ToListAsync();
        }

        [HttpGet("searchuser/{searchValue}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<User>>> SearchUser(string searchValue)
        {
            if (_context.User == null)
            {
                return NotFound();
            }
            try
            {
                var searchedUsers = await _context.User.Where(user => (user.DisplayName.Contains(searchValue) || user.Email.Contains(searchValue))).ToListAsync();
                return Ok(searchedUsers);
            }
            catch (Exception err)
            {
                _logger.LogError("Error searching user: ", err);
                return Problem(err.ToString());
            }
        }
        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            if (_context.User == null)
            {
                return NotFound();
            }
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> UpdateUserProfile([FromBody] User user)
        {
            if (_context.User == null)
            {
                return Problem("Entity set 'DatabaseContext.User'  is null.");
            }

            try
            {

                if (UserExists(user.ID))
                {
                    _context.Entry(user).State = EntityState.Modified;
                }
                else
                {
                    await _context.User.AddAsync(user);
                }

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError("Error saving user: ", e);
                return BadRequest();
            }

        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (_context.User == null)
            {
                return NotFound();
            }
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(string id)
        {
            return (_context.User?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
