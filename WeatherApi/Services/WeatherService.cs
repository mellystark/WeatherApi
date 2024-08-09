using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Xml.Linq;
using WeatherApi.Data;
using WeatherApi.Models;

namespace WeatherApi.Services
{
    public class WeatherService
    {
        private readonly ApplicationDbContext _context; // Burada _context tanımlanıyor
        private readonly string _xmlUrl = "https://mgm.gov.tr/FTPDATA/analiz/SonDurumlarTumu.xml";

        // Constructor'a ApplicationDbContext ekleyin
        public WeatherService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); // Null kontrolü
        }

        public async Task<List<WeatherCondition>> GetWeatherConditionsAsync()
        {
            var weatherConditions = new List<WeatherCondition>();

            using (var httpClient = new HttpClient())
            {
                var xmlContent = await httpClient.GetStringAsync(_xmlUrl);
                var xdoc = XDocument.Parse(xmlContent);

                var items = xdoc.Descendants("Merkezler")
                    .Select(x => new WeatherCondition
                    {
                        Date = DateTime.ParseExact(x.Element("Tarih")?.Value, "yyMMddHHmm", CultureInfo.InvariantCulture),
                        StationId = x.Element("staind")?.Value,
                        City = x.Element("ili")?.Value,
                        District = x.Element("ilcesi")?.Value,
                        StationName = x.Element("istadi")?.Value,
                        Temperature = decimal.Parse(x.Element("tmp")?.Value.Replace(',', '.'), CultureInfo.InvariantCulture),
                        Humidity = (int)Math.Round(decimal.Parse(x.Element("nem")?.Value.Replace(',', '.'), CultureInfo.InvariantCulture)),
                        WindSpeed = (int)Math.Round(decimal.Parse(x.Element("ws")?.Value.Replace(',', '.'), CultureInfo.InvariantCulture)),
                        WindDirection = (int)Math.Round(decimal.Parse(x.Element("wd")?.Value.Replace(',', '.'), CultureInfo.InvariantCulture)),
                    }).ToList();

                weatherConditions.AddRange(items);
            }

            return weatherConditions;
        }

        // Hava durumu silme metodu
        public async Task DeleteWeatherConditionByCityAndDistrictAsync(string city, string district)
        {
            // 'city' ve 'district' parametrelerini küçük harfe çeviriyoruz
            var cityLower = city.ToLower();
            var districtLower = district.ToLower();

            var weatherCondition = await _context.WeatherConditions
                .FirstOrDefaultAsync(w => w.City.ToLower() == cityLower && w.District.ToLower() == districtLower); // 'City' ve 'District' alanlarını da küçük harfe çeviriyoruz

            if (weatherCondition == null)
            {
                throw new InvalidOperationException($"Weather data for city '{city}' and district '{district}' not found.");
            }

            _context.WeatherConditions.Remove(weatherCondition);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateWeatherConditionByCityAsync(string city)
        {
            using (var httpClient = new HttpClient())
            {
                var xmlContent = await httpClient.GetStringAsync(_xmlUrl);
                var xdoc = XDocument.Parse(xmlContent);

                var updatedConditions = xdoc.Descendants("Merkezler")
                    .Where(x => (x.Element("ili")?.Value.Equals(city, StringComparison.OrdinalIgnoreCase) ?? false))
                    .Select(x => new WeatherCondition
                    {
                        Date = DateTime.ParseExact(x.Element("Tarih")?.Value, "yyMMddHHmm", CultureInfo.InvariantCulture),
                        StationId = x.Element("staind")?.Value,
                        City = x.Element("ili")?.Value,
                        District = x.Element("ilcesi")?.Value,
                        StationName = x.Element("istadi")?.Value,
                        Temperature = decimal.Parse(x.Element("tmp")?.Value.Replace(',', '.'), CultureInfo.InvariantCulture),
                        Humidity = (int)Math.Round(decimal.Parse(x.Element("nem")?.Value.Replace(',', '.'), CultureInfo.InvariantCulture)),
                        WindSpeed = (int)Math.Round(decimal.Parse(x.Element("ws")?.Value.Replace(',', '.'), CultureInfo.InvariantCulture)),
                        WindDirection = (int)Math.Round(decimal.Parse(x.Element("wd")?.Value.Replace(',', '.'), CultureInfo.InvariantCulture)),
                    })
                    .ToList();

                if (updatedConditions.Count == 0)
                {
                    return false;
                }

                var existingConditions = await _context.WeatherConditions
                    .Where(w => w.City.ToLower() == city.ToLower())
                    .ToListAsync();

                _context.WeatherConditions.RemoveRange(existingConditions);
                _context.WeatherConditions.AddRange(updatedConditions);

                await _context.SaveChangesAsync();
                return true;
            }
        }


    }
}
