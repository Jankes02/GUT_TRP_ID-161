using Microsoft.AspNetCore.Mvc;
using MonolithicApp.Services.intf;
using System;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace MonolithicApp.Controllers
{
    [ApiController]
    [Route("routes")]
    public class RouteController : ControllerBase
    {
        private readonly IRouteService _routeService;
        private readonly ILogger<RouteController> _logger;

        public RouteController(IRouteService routeService, ILogger<RouteController> logger)
        {
            _routeService = routeService;
            _logger = logger;
        }

        [HttpGet("shortest")]
        public IActionResult GetShortestRoute([FromQuery] string fromName, [FromQuery] string toName, [FromQuery] string? date)
        {
            DateTime? parsedDate = null;
            if (!string.IsNullOrEmpty(date))
            {
                if (DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                {
                    parsedDate = dt;
                }
                else
                {
                    return BadRequest("Invalid date format. Use yyyy-MM-dd.");
                }
            }

            try
            {
                var route = _routeService.ShortestByStops(fromName, toName, parsedDate);
                return Ok(route);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return NotFound(ex.Message);
            }
        }

        [HttpGet("random-points")]
        public IActionResult RandomPoints([FromQuery] int count)
        {
            try
            {
                var points = _routeService.FindShortestRouteRandomPoints(count);
                return Ok(points);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("most-used")]
        public IActionResult GetMostUsedConnections([FromQuery] string? date)
        {
            DateTime? parsedDate = null;
            if (!string.IsNullOrEmpty(date))
            {
                if (DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                {
                    parsedDate = dt;
                }
                else
                {
                    return BadRequest("Invalid date format. Use yyyy-MM-dd.");
                }
            }

            try
            {
                var connections = _routeService.GetMostUsedConnections(parsedDate);
                return Ok(connections);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}