using WeatherApi.Data;

namespace WeatherApi.Services
{
    public class WeatherUpdateService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(15); //15 dakikada bir güncelle

        public WeatherUpdateService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await UpdateWeatherDataAsync();
                await Task.Delay(_interval, stoppingToken);
            }
        }

        private async Task UpdateWeatherDataAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var weatherService = scope.ServiceProvider.GetRequiredService<WeatherService>();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var weatherConditions = await weatherService.GetWeatherConditionsAsync();

                // Clear existing data if needed
                dbContext.WeatherConditions.RemoveRange(dbContext.WeatherConditions);

                // Add new data
                dbContext.WeatherConditions.AddRange(weatherConditions);
                await dbContext.SaveChangesAsync();
            }
        }
    }

}
