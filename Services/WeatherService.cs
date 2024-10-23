using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Weather_Api.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IDistributedCache _redisCache;

        public WeatherService(HttpClient httpClient, IConfiguration config, IDistributedCache redisCache)
        {
            _httpClient = httpClient;
            _config = config;
            _redisCache = redisCache;
        }

        public async Task<string> GetWeatherAsync(string location)
        {
            var cachedWeather = await _redisCache.GetStringAsync(location);

            if (!string.IsNullOrEmpty(cachedWeather))
            {
                return cachedWeather;
            }
            var apiKey = _config["VisualCrossing:ApiKey"];
            var baseUrl = _config["VisualCrossing:BaseUrl"];
            var url = $"{baseUrl}{location}?unitGroup=metric&key={apiKey}";

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var weatherData =  await response.Content.ReadAsStringAsync();

                //Store the data in Redis cache for a set Time
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(5)
                };
                await _redisCache.SetStringAsync(location, weatherData, cacheOptions);

                return weatherData;
            }
            return "Error fetching weather data";
        }
    }
}