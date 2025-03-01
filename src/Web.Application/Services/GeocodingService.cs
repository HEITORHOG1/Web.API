using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Web.Application.Interfaces;

namespace Web.Application.Services
{
    public class GeocodingService : IGeocodingService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeocodingService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["GoogleMaps:ApiKey"];
        }

        public async Task<(double Latitude, double Longitude)> GetCoordinatesAsync(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("O endereço não pode ser vazio.", nameof(address));

            var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={_apiKey}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);

            var results = json.RootElement.GetProperty("results");
            if (results.GetArrayLength() == 0)
                throw new Exception("Endereço não encontrado.");

            var location = results[0]
                .GetProperty("geometry")
                .GetProperty("location");

            double latitude = location.GetProperty("lat").GetDouble();
            double longitude = location.GetProperty("lng").GetDouble();

            return (latitude, longitude);
        }
    }
}