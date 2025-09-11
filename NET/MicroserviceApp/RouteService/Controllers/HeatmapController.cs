using Microsoft.AspNetCore.Mvc;
using RouteService.DTOs;
using System;
using Microsoft.Extensions.Logging;
using RouteService.Services.intf;

namespace RouteService.Controllers
{
    [ApiController]
    [Route("routes")]
    public class HeatmapController : ControllerBase
    {
        private readonly IRouteService _routeService;
        private readonly ILogger<HeatmapController> _logger;

        public HeatmapController(IRouteService routeService, ILogger<HeatmapController> logger)
        {
            _routeService = routeService;
            _logger = logger;
        }

        [HttpPost("heatmap")]
        public IActionResult Heatmap([FromBody] HeatmapRequest request)
        {
            try
            {
                var response = _routeService.HeatmapUsage(request);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Bad request for heatmap");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating heatmap");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
