using Microsoft.AspNetCore.Mvc;
using MonolithicApp.Database.Model;
using MonolithicApp.Services.intf;

namespace MonolithicApp.Controllers
{
    [ApiController]
    [Route("city")]
    public class CityController : ControllerBase
    {
        private readonly ICityService _cityService;
        private readonly ILogger<CityController> _logger;

        public CityController(ICityService cityService, ILogger<CityController> logger)
        {
            _cityService = cityService;
            _logger = logger;
        }

        [HttpGet()]
        public IActionResult Get()
        {
            try
            {
                return Ok(_cityService.GetAll());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex);
            }
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            try
            {
                return Ok(_cityService.FindByName(name));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public IActionResult Add([FromBody] City city)
        {
            try
            {
                if (_cityService.AddCity(city))
                    return Ok();

                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex);
            }
        }
    }
}