using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotoSeguro.API.Services;

namespace VotoSeguro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("statistics")]
    [Authorize(Roles = "administrador")]
    public async Task<IActionResult> GetStatistics()
    {
        try
        {
            var statistics = await _reportService.GetStatistics();
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener estadísticas", error = ex.Message });
        }
    }
}
