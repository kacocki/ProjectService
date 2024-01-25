using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectService.WebAPI.Data;
using ProjectService.WebAPI.Services;

namespace ProjectService.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IUsersService _usersService;
        private readonly IProjectsService _projectsService;
        public ProjectsController(
            ILogger<WeatherForecastController> logger,
            IUsersService usersService,
            IProjectsService projectsService)
        {
            _logger = logger;
            _usersService = usersService;
            _projectsService = projectsService;
        }

        [HttpPost("addProject")]
        public async Task<IActionResult> Add([FromBody] Project project)
        {
            try
            {
                await _projectsService.Add(project);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{projectId}")]
        public async Task<IActionResult> Delete(int projectId)
        {
            try
            {
                var project = (await _projectsService.Get(new int[] { projectId })).FirstOrDefault();
                if (project is null)
                {
                    return NotFound($"Project with ID {projectId} not found.");
                }

                var users = await _usersService.Get(projectId, null);
                if (users != null)
                {
                    foreach (var user in users)
                    {
                        await _usersService.Delete(user);
                    };                
                }
                await _projectsService.Delete(project);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("{projectId}/users")]
        public async Task<IActionResult> Add(int projectId, [FromBody] User user)
        {
            try
            {
                var project = await _projectsService.Get(new int[] { projectId });
                if (!project.Any())
                {
                    return NotFound($"Project with ID {projectId} not found.");
                }

                user.ProjectId = projectId;
                var addedUser = await _usersService.Add(user);

                return StatusCode(201, new { addedUser.Id, addedUser.Name, addedUser.AddedDate, addedUser.ProjectId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{projectId}/users")]
        public async Task<IActionResult> Get(int projectId)
        {
            try
            {
                var project = (await _projectsService.Get(new int[] { projectId })).FirstOrDefault();

                if (project is null)
                {
                    return NotFound($"Project with ID {projectId} not found.");
                }

                var users = (await _usersService.Get(projectId, null))
                    .Select(x => new { x.Id, x.Name, x.AddedDate, x.ProjectId });

                return StatusCode(200, users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}