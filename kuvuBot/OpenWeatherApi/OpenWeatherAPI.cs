using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace kuvuBot.OpenWeatherApi
{
    public static class OpenWeatherApiExt
    {
        public static decimal KelvinToCelsius(this decimal kelvin)
        {
            return kelvin - (decimal) 273.15;
        }
    }
    
    public class OpenWeatherApi
    {
        private readonly string apiKey;
        private readonly string language;
        private readonly HttpClient httpClient = new HttpClient();

        public OpenWeatherApi(string apiKey, string language = "en")
        {
            this.apiKey = apiKey;
            this.language = language;
            httpClient.BaseAddress = new Uri($"https://api.openweathermap.org/data/2.5/");
        }

        private async Task<City> GetCity(string query)
        {
            var json = await httpClient.GetAsync($"weather?appid={apiKey}&lang={language}{query}");
            return JsonConvert.DeserializeObject<City>(await json.Content.ReadAsStringAsync());
        }

        public async Task<City> GetWeatherByCityName(string cityName)
        {
            return await GetCity($"&q={cityName}");
        }

        public async Task<City> GetWeatherByCityId(int cityId)
        {
            return await GetCity($"&id={cityId}");
        }

        public async Task<City> GetWeatherByCoords(long lat, long lon)
        {
            return await GetCity($"&lat={lat}&lon={lon}");
        }
    }
}
