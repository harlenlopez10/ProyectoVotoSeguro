using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoVotoSeguro.Services;

namespace ProyectoVotoSeguro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // GET: api/Reports/statistics
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userRole == "admin")
                {
                    // Admin ve estadísticas globales
                    var statistics = await _reportService.GetTaskStatistics();
                    return Ok(statistics);
                }
                else
                {
                    // Usuario normal ve solo sus estadísticas
                    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userId))
                    {
                        return Unauthorized(new { error = "Token inválido" });
                    }

                    var statistics = await _reportService.GetUserTaskStatistics(userId);
                    return Ok(statistics);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET: api/Reports/user/{userId}/statistics
        [HttpGet("user/{userId}/statistics")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetUserStatistics(string userId)
        {
            try
            {
                var statistics = await _reportService.GetUserTaskStatistics(userId);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET: api/Reports/overdue
        [HttpGet("overdue")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetOverdueTasks()
        {
            try
            {
                var tasks = await _reportService.GetOverdueTasks();
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET: api/Reports/due-soon?days=7
        [HttpGet("due-soon")]
        public async Task<IActionResult> GetTasksDueSoon([FromQuery] int days = 7)
        {
            try
            {
                var tasks = await _reportService.GetTasksDueSoon(days);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}