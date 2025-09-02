using Microsoft.AspNetCore.Mvc;
using MonolithicApp.Services.intf;

namespace MonolithicApp.Controllers
{
    [ApiController]
    [Route("route")]
    public class RouteController : ControllerBase
    {
        private readonly IRouteService _routeService;
        private readonly ILogger<RouteController> _logger;

        public RouteController(IRouteService routeService, ILogger<RouteController> logger)
        {
            _routeService = routeService;
            _logger = logger;
        }

        [HttpGet("all")]
        public IActionResult FindAllRoutesFromAToB([FromQuery] string from, [FromQuery] string to)
        {
            try
            {
                var routes = _routeService.FindAllRoutes(from, to);
                return Ok(routes);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex);
            }
        }

        [HttpGet("optimal")]
        public IActionResult FindOptimalRoute([FromQuery] string from, [FromQuery] string to)
        {
            try
            {
                var routes = _routeService.FindAllRoutes(from, to);
                if (routes is not null && routes.Count > 0)
                    return Ok(routes.OrderBy(r => r.Count).FirstOrDefault());

                return Ok(new List<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex);
            }
        }
    }
}