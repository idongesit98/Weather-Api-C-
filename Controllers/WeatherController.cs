using Microsoft.AspNetCore.Mvc;
using Weather_Api.Services;

namespace Weather_Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherController : ControllerBase
{
    private readonly WeatherService _weatherService;

    public WeatherController(WeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [HttpGet("{location}")]
    public async Task<IActionResult> GetWeather(string location)
    {
        var weatherData = await _weatherService.GetWeatherAsync(location);
        return Ok(weatherData);
    }
}
