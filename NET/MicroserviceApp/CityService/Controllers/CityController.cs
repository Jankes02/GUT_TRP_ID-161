using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using CityService.Services.intf;

namespace CityService.Controllers
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

        [HttpGet("{name}")]
        public IActionResult Get(string name)
        {
            try
            {
                var city = _cityService.FindByName(name);
                return Ok(city);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "City not found: {Name}", name);
                return NotFound("City not found");
            }
        }

        [HttpPost]
        public IActionResult AddCity([FromBody] City city)
        {
            try
            {
                var addedCity = _cityService.AddCity(city);
                return CreatedAtAction(nameof(Get), new { name = addedCity.Name }, addedCity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding city");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("info/{name}")]
        public IActionResult GetInfo(string name)
        {
            try
            {
                var cityInfo = _cityService.ComputeInfo(name);
                return Ok(cityInfo);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "City not found for info: {Name}", name);
                return NotFound("City not found");
            }
        }
    }
}