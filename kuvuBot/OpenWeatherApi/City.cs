using System.Collections.Generic;
using Newtonsoft.Json;

namespace kuvuBot.OpenWeatherApi
{
    public class City
    {
        /// <summary>
        /// City geo location
        /// </summary>
        [JsonProperty("coord")]
        public Coordinates Coordinates { get; set; }

        /// <summary>
        /// Weather condition codes
        /// </summary>
        [JsonProperty("weather")]
        public List<Weather> Weather { get; set; }

        /// <summary>
        /// Internal parameter
        /// </summary>
        [JsonProperty("base")]
        public string Base { get; set; }

        [JsonProperty("main")]
        public Main Main { get; set; }

        [JsonProperty("visibility", NullValueHandling = NullValueHandling.Ignore)]
        public long? Visibility { get; set; }

        [JsonProperty("wind")]
        public Wind Wind { get; set; }

        [JsonProperty("clouds")]
        public Clouds Clouds { get; set; }

        [JsonProperty("rain")]
        public Rain Rain { get; set; }

        [JsonProperty("snow")]
        public Snow Snow { get; set; }

        /// <summary>
        /// Time of data calculation, unix, UTC
        /// </summary>
        [JsonProperty("dt")]
        public long Dt { get; set; }

        [JsonProperty("sys")]
        public Sys Sys { get; set; }

        /// <summary>
        /// Shift in seconds from UTC
        /// </summary>
        [JsonProperty("timezone", NullValueHandling = NullValueHandling.Ignore)]
        public long? Timezone { get; set; }

        /// <summary>
        /// City ID
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; set; }

        /// <summary>
        /// City name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Internal parameter (error code)
        /// </summary>
        [JsonProperty("cod")]
        public int Cod { get; set; }

        /// <summary>
        /// Internal parameter (error message)
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class Clouds
    {
        /// <summary>
        /// Cloudiness, %
        /// </summary>
        [JsonProperty("all")]
        public long All { get; set; }
    }

    public class Coordinates
    {
        [JsonProperty("lon")]
        public decimal Longitude { get; set; }

        [JsonProperty("lat")]
        public decimal Latitude { get; set; }
    }

    public class Main
    {
        /// <summary>
        /// Temperature. Unit Default: Kelvin, Metric: Celsius, Imperial: Fahrenheit.
        /// </summary>
        [JsonProperty("temp")]
        public decimal Temperature { get; set; }

        /// <summary>
        /// Atmospheric pressure (on the sea level, if there is no sea_level or grnd_level data), hPa
        /// </summary>
        [JsonProperty("pressure")]
        public long Pressure { get; set; }

        /// <summary>
        /// Humidity, %
        /// </summary>
        [JsonProperty("humidity")]
        public long Humidity { get; set; }

        /// <summary>
        /// Minimum temperature at the moment. This is deviation from current temp that is possible for large cities and megalopolises geographically expanded (use these parameter optionally). Unit Default: Kelvin, Metric: Celsius, Imperial: Fahrenheit.
        /// </summary>
        [JsonProperty("temp_min")]
        public decimal MinimumTemperature { get; set; }

        /// <summary>
        /// Maximum temperature at the moment. This is deviation from current temp that is possible for large cities and megalopolises geographically expanded (use these parameter optionally). Unit Default: Kelvin, Metric: Celsius, Imperial: Fahrenheit.
        /// </summary>
        [JsonProperty("temp_max")]
        public decimal MaximumTemperature { get; set; }
    }

    public class Sys
    {
        /// <summary>
        /// Internal parameter
        /// </summary>
        [JsonProperty("type")]
        public long Type { get; set; }

        /// <summary>
        /// Internal parameter
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; set; }

        /// <summary>
        /// Internal parameter
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// Country code (GB, JP etc.)
        /// </summary>
        [JsonProperty("country")]
        public string Country { get; set; }

        /// <summary>
        /// Sunrise time, unix, UTC
        /// </summary>
        [JsonProperty("sunrise")]
        public long Sunrise { get; set; }

        /// <summary>
        /// Sunset time, unix, UTC
        /// </summary>
        [JsonProperty("sunset")]
        public long Sunset { get; set; }
    }

    public class Weather
    {
        /// <summary>
        /// Weather condition id
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; set; }

        /// <summary>
        /// Group of weather parameters (Rain, Snow, Extreme etc.)
        /// </summary>
        [JsonProperty("main")]
        public string Main { get; set; }

        /// <summary>
        /// Weather condition within the group
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Weather icon id
        /// </summary>
        [JsonProperty("icon")]
        public string Icon { get; set; }
    }

    public class Wind
    {
        /// <summary>
        /// Wind speed. Unit Default: meter/sec, Metric: meter/sec, Imperial: miles/hour.
        /// </summary>
        [JsonProperty("speed")]
        public decimal Speed { get; set; }

        /// <summary>
        /// Wind direction, degrees (meteorological)
        /// </summary>
        [JsonProperty("deg")]
        public decimal Degrees { get; set; }
    }

    public class Rain
    {
        /// <summary>
        /// Rain volume for the last 1 hour, mm
        /// </summary>
        [JsonProperty("1h")]
        public decimal LastHour { get; set; }

        /// <summary>
        /// Rain volume for the last 3 hours, mm
        /// </summary>
        [JsonProperty("3h")]
        public decimal Last3Hours { get; set; }
    }

    public class Snow
    {
        /// <summary>
        /// Snow  volume for the last 1 hour, mm
        /// </summary>
        [JsonProperty("1h")]
        public decimal LastHour { get; set; }

        /// <summary>
        /// Snow  volume for the last 3 hours, mm
        /// </summary>
        [JsonProperty("3h")]
        public decimal Last3Hours { get; set; }
    }
}
