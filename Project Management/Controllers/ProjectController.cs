using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Management.Database;
using Project_Management.Models.DatabaseModel;
using ModelTask = Project_Management.Models.DatabaseModel.Task;

namespace Project_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly ILogger _logger;
        public ProjectController(DatabaseContext context, ILogger<ProjectController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Project
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetAllProjects()
        {
            if (_context.Project == null)
            {
                return NotFound();
            }
            return Ok(await _context.Project.ToListAsync());
        }

        // GET: api/Project/5
        [HttpGet("{id}")]
        
        public async Task<ActionResult<Project>> GetProjectByID(string id)
        {
            if (_context.Project == null)
            {
                return NotFound();
            }
            var project = await _context.Project.Include(project => project.Creator).Include(project => project.Leader).FirstOrDefaultAsync(project => project.ID == id);

            if (project == null)
            {
                return NotFound();
            }

            return Ok(project);
        }

        [HttpGet("uid/{uid}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserProject>>> GetUserProjectsDetails(string uid)
        {
            try
            {
                var userProjectsWithDetails = new List<UserProject>();

                userProjectsWithDetails = await (from userProject in _context.UserProject
                                                 .Include(userProject => userProject.Project).ThenInclude(project => project.Leader)
                                                                            .Include(userProject => userProject.Project).ThenInclude(project => project.Creator)
                                                 let project = userProject.Project
                                                 where userProject.UserID == uid && (project.IsDeleted == false)
                                                 select userProject
                                  ).ToListAsync();
                return Ok(userProjectsWithDetails);
            }
            catch (Exception e)
            {
                _logger.LogError("Get Project Error", e);
                return BadRequest(e.Message);
            }
        }

        // POST: api/Project
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserProject>> CreateProject([FromBody] Project project)
        {
            if (_context.Project == null)
            {
                return Problem("Entity set 'DatabaseContext.Project'  is null.");
            }
            try
            {
                if (project.ID != null && ProjectExists(project.ID))
                {
                    _context.Entry(project).State = EntityState.Modified;
                }
                else
                {
                    await _context.Project.AddAsync(project);
                    var userProject = new UserProject()
                    {
                        ProjectID = project.ID,
                        UserID = project.CreatorID,
                    };
                    await _context.SaveChangesAsync();

                    await _context.UserProject.AddAsync(userProject);
                }
                await _context.SaveChangesAsync();
                var updatedUserProject = _context.UserProject
                    .Include(up => up.Project).ThenInclude(p => p.Leader)
                    .Include(up => up.Project).ThenInclude(p => p.Creator).Where(up => up.ProjectID == project.ID ).FirstOrDefault();
            
                return Ok(updatedUserProject);
            }
            catch (Exception err)
            {
                _logger.LogError("Create Project", err);
                System.Diagnostics.Debug.WriteLine(err.Message);
                return Problem(err.ToString());
            }
        }

        [HttpPut]
        public async Task<ActionResult<Project>> UpdateProjectDetails(Project updatedProject)
        {
            if (_context.Project == null)
            {
                return Problem("Entity set 'DatabaseContext.Project'  is null.");
            }
            try
            {
                if (updatedProject.ID != null && ProjectExists(updatedProject.ID))
                {
                    _context.Entry(updatedProject).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    var project = _context.Project.Include(pj => pj.Creator).Include(pj => pj.Leader).Where(pj => pj.ID == updatedProject.ID).FirstOrDefault();
                    return Ok(project);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception err)
            {
                return Problem("Error: ", err.Message);
            }
        }


        // DELETE: api/Project/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(string id)
        {
            if (_context.Project == null)
            {
                return NotFound();
            }
            try
            {
                var project = await _context.Project.FindAsync(id);
                if (project == null)
                {
                    return NotFound();
                }

                _context.Project.Remove(project);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch(Exception err)
            {
                _logger.LogError("Error deleting project: ", err);
                return Problem(err.ToString());
            }
  
        }

        [HttpPost("updateUserProject")]

        public async Task<ActionResult> UpdateUserProject([FromBody] UserProject userProject)
        {
            try
            {
                if (!UserProjectExists(userProject.ID!))
                {
                    _logger.LogError("userproject id not exist");
                    return BadRequest();
                }
                _context.Entry(userProject).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception err) {
                _logger.LogError("Error update userproject: ", err);
                return BadRequest(err.Message);
            }
        }
   
        [HttpGet("tasks/{projectId}")]
        public async Task<ActionResult<IEnumerable<ModelTask>>> GetProjectTasks(string projectId)
        {
            if (_context.Task == null)
            {
               return Problem("Entity set 'DatabaseContext.Task'  is null.");
            }

            try
            {
                var tasks = await (from task in _context.Task.Include(task => task.Assignee).Include(task => task.Creator)
                             where task.ProjectID == projectId
                             select task
                    ).ToListAsync();
                return Ok(tasks);
            }
            catch (Exception err)
            {
                _logger.LogError("Error getting task", err);
                return Problem(err.Message);
            }

        }

        [HttpGet("members/{projectId}")]
        public async Task<ActionResult<IEnumerable<User>>> GetProjectMembers(string projectId) {
            if (_context.UserProject == null)
            {
                return Problem("Entity set 'DatabaseContext.UserProject'  is null.");
            }
            try
            {
                var users = await (from userProject in _context.UserProject 
                             join user in _context.User on userProject.UserID equals user.ID
                             where userProject.ProjectID == projectId
                             select user
                             ).ToListAsync();

                return Ok(users);
            }catch (Exception err)
            {
                return Problem(err.ToString());
            }
        }

        private bool UserProjectExists(string id)
        {
            return (_context.UserProject?.Any(up => up.ID == id)).GetValueOrDefault();
        }

        private bool ProjectExists(string id)
        {
            return (_context.Project?.Any(e => e.ID == id)).GetValueOrDefault();
        }


    }
}
