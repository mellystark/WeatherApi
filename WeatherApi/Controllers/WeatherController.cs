using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using WeatherApi.Services;

namespace WeatherApi.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherService _weatherService;
        private readonly ExcelHelper _excelHelper;

        public WeatherController(WeatherService weatherService, ExcelHelper excelHelper)
        {
            _weatherService = weatherService ?? throw new ArgumentNullException(nameof(weatherService));
            _excelHelper = excelHelper;
        }

        [HttpGet]
        public async Task<IActionResult> GetWeather()
        {
            var weatherConditions = await _weatherService.GetWeatherConditionsAsync();
            return Ok(weatherConditions);
        }

        [HttpGet("{city}")]
        public async Task<IActionResult> GetWeatherByCity(string city)
        {
            var weatherConditions = await _weatherService.GetWeatherConditionsAsync();
            var cityWeather = weatherConditions.Where(w => w.City.Equals(city, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!cityWeather.Any())
            {
                return NotFound($"Weather data for city '{city}' not found.");
            }

            return Ok(cityWeather);
        }

        [HttpGet("{city}/dateRange")]
        public async Task<IActionResult> GetWeatherByCityAndDateRange(string city, [FromQuery] string startDate, [FromQuery] string endDate)
        {
            var weatherConditions = await _weatherService.GetWeatherConditionsAsync();

            try
            {
                var parsedStartDate = DateTime.ParseExact(startDate, "yyyyMMdd", CultureInfo.InvariantCulture).Date;
                var parsedEndDate = DateTime.ParseExact(endDate, "yyyyMMdd", CultureInfo.InvariantCulture).Date.AddDays(1).AddTicks(-1); // Include end day till 23:59:59

                var cityWeather = weatherConditions
                    .Where(w => w.City.Equals(city, StringComparison.OrdinalIgnoreCase) && w.Date.Date >= parsedStartDate && w.Date.Date <= parsedEndDate)
                    .ToList();

                if (!cityWeather.Any())
                {
                    return NotFound($"No weather data found for city '{city}' between {parsedStartDate:yyyy-MM-dd} and {parsedEndDate:yyyy-MM-dd}.");
                }

                return Ok(cityWeather);
            }
            catch (FormatException)
            {
                return BadRequest("Invalid date format. Use yyyyMMdd.");
            }
        }

        [Authorize]
        [HttpGet("{city}/export")]
        public async Task<IActionResult> ExportWeatherDataToExcel(string city)
        {
            var weatherConditions = await _weatherService.GetWeatherConditionsAsync();
            var cityWeather = weatherConditions
                .Where(w => w.City.Equals(city, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!cityWeather.Any())
            {
                return NotFound($"No weather data found for city '{city}'.");
            }

            var excelFile = await _excelHelper.CreateExcelFileAsync(cityWeather);
            return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{city}_WeatherData.xlsx");
        }

        // Şehir bazlı hava durumu silme
        [Authorize]
        [HttpDelete("{city}/{district}")]
        public async Task<IActionResult> DeleteWeatherByCityAndDistrict(string city, string district)
        {
            try
            {
                await _weatherService.DeleteWeatherConditionByCityAndDistrictAsync(city, district);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize]
        [HttpPut("{city}/update")]
        public async Task<IActionResult> UpdateWeatherConditionByCity(string city)
        {
            var result = await _weatherService.UpdateWeatherConditionByCityAsync(city);

            if (!result)
            {
                return NotFound($"'{city}' şehri için güncel hava durumu verisi bulunamadı.");
            }

            return Ok($"'{city}' şehri için hava durumu verileri güncellendi.");
        }
    }
}