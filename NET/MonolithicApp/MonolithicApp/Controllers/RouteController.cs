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

        [HttpGet]
        public IActionResult FindRouteFromAToB([FromQuery] string from, [FromQuery] string to)
        {
            try
            {
                var route = _routeService.FindAnyRoute(from, to);
                return Ok(route);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex);
            }
        }
    }
}