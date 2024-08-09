namespace WeatherApi.Models
{
    public class WeatherCondition
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string StationId { get; set; } 
        public string City { get; set; }
        public string District { get; set; } //ilçe
        public string StationName { get; set; }
        public decimal Temperature { get; set; }
        public int Humidity { get; set; }  //nem
        public int WindSpeed { get; set; }
        public int WindDirection { get; set; } //rüzgar yönü

    }
}
