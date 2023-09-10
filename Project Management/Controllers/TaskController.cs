using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Management.Database;
using TaskModel = Project_Management.Models.DatabaseModel.Task;
namespace Project_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<TaskController> _logger;
        public TaskController(DatabaseContext context, ILogger<TaskController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Task
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskModel>>> GetTask()
        {
            if (_context.Task == null)
            {
                return NotFound();
            }
            return await _context.Task.ToListAsync();
        }

        // GET: api/Task/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskModel>> GetTask(string id)
        {
            if (_context.Task == null)
            {
                return NotFound();
            }
            var task = await _context.Task.Include(task => task.Creator).Include(task => task.Assignee).FirstOrDefaultAsync(task => task.ID == id);

            if (task == null)
            {
                return NotFound();
            }

            return task;
        }

        // PUT: api/Task/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTask(string id, TaskModel task)
        {
            if (id != task.ID)
            {
                return BadRequest();
            }

            _context.Entry(task).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
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

        // POST: api/Task
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TaskModel>> UpdateTaskDetail(TaskModel task)
        {
            if (_context.Task == null)
            {
                return Problem("Entity set 'DatabaseContext.Task'  is null.");
            }
            try
            {
                if (task.ID is not null && TaskExists(task.ID))
                {
                    _context.Entry(task).State = EntityState.Modified;
                }
                else
                {
                    task.CreatedTime = DateTime.UtcNow;
                    await _context.Task.AddAsync(task);
                }
                await _context.SaveChangesAsync();
                var updatedTask = (from t in _context.Task.Include(t => t.Creator)
                                     where t.ID == task.ID
                                     select t
                                     ).FirstOrDefault();
                return Ok(updatedTask);
            }
            catch (Exception err)
            {
                _logger.LogError("Error creating task: ", err);
                return BadRequest(err.Message);
            }
        }

        // DELETE: api/Task/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(string id)
        {
            if (_context.Task == null)
            {
                return NotFound();
            }
            var task = await _context.Task.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _context.Task.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaskExists(string id)
        {
            return (_context.Task?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
