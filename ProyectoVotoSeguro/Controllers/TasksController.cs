using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoVotoSeguro.DTOs;
using ProyectoVotoSeguro.Services;

namespace ProyectoVotoSeguro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        // GET: api/Tasks
        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            try
            {
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userRole == "admin")
                {
                    // Admin ve todas las tareas
                    var allTasks = await _taskService.GetAllTasks();
                    return Ok(allTasks);
                }
                else
                {
                    // Usuario normal solo ve sus tareas
                    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userId))
                    {
                        return Unauthorized(new { error = "Token inválido" });
                    }

                    var userTasks = await _taskService.GetTasksByUserId(userId);
                    return Ok(userTasks);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET: api/Tasks/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(string id)
        {
            try
            {
                var task = await _taskService.GetTaskById(id);

                if (task == null)
                {
                    return NotFound(new { error = "Tarea no encontrada" });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                // Verificar permisos
                if (userRole != "admin" && task.AssignedToUserId != userId)
                {
                    return Forbid();
                }

                return Ok(task);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // POST: api/Tasks
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userName))
                {
                    return Unauthorized(new { error = "Token inválido" });
                }

                var task = await _taskService.CreateTask(createTaskDto, userId, userName);
                return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // PUT: api/Tasks/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateTask(string id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            try
            {
                var task = await _taskService.UpdateTask(id, updateTaskDto);

                if (task == null)
                {
                    return NotFound(new { error = "Tarea no encontrada" });
                }

                return Ok(task);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // DELETE: api/Tasks/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteTask(string id)
        {
            try
            {
                var result = await _taskService.DeleteTask(id);

                if (!result)
                {
                    return NotFound(new { error = "Tarea no encontrada" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // POST: api/Tasks/{id}/complete
        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteTask(string id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { error = "Token inválido" });
                }

                var task = await _taskService.CompleteTask(id, userId);

                if (task == null)
                {
                    return NotFound(new { error = "Tarea no encontrada" });
                }

                return Ok(task);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET: api/Tasks/user/{userId}
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetTasksByUser(string userId)
        {
            try
            {
                var tasks = await _taskService.GetTasksByUserId(userId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}