using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using System.Text.RegularExpressions;
using DSharpPlus;
using kuvuBot.Commands.Attributes;
using kuvuBot.Lang;
using kuvuBot.OpenWeatherApi;

namespace kuvuBot.Commands.Information
{
    public class WeatherCommand : BaseCommandModule
    {
        [Command("weather"), LocalizedDescription("weather.description")]
        [RequireBotPermissions(Permissions.SendMessages)]
        public async Task Weather(CommandContext ctx, [RemainingText] string cityName)
        {
            cityName.RequireRemainingText();
            await ctx.Channel.TriggerTypingAsync();
            var api = new OpenWeatherApi.OpenWeatherApi(Program.Config.Apis.OpenWeatherApi, await ctx.Lang("weather.lang"));
            var city = await api.GetWeatherByCityName(cityName);

            if (city.Cod != (int)HttpStatusCode.OK)
            {
                await new ModernEmbedBuilder
                {
                    Title = await ctx.Lang("weather.error") + $" ({city.Cod})",
                    Color = EmbedUtils.Red,
                    Description = city.Message.First().ToString().ToUpper() + city.Message.Substring(1)
                }.AddGeneratedForFooter(ctx, false).Send(ctx.Message.Channel);
                return;
            }

            var emoji = DiscordEmoji.FromName(ctx.Client, $":flag_{city.Sys.Country.ToLower()}:");
            var flag = emoji == null ? "🏁" : emoji.ToString();
            var weather = city.Weather.First();

            var embed = new ModernEmbedBuilder
            {
                Title = await ctx.Lang("weather.title"),
                Fields =
                {
                    (await ctx.Lang("weather.city"), $"{(weather.Icon.Contains("n") ? "🏙" : "🌆")} {city.Name}", inline: true),
                    (await ctx.Lang("weather.country"), $"{flag} {city.Sys.Country}", inline: true),
                    (await ctx.Lang("weather.temperature"), "🌡 " + city.Main.Temperature.KelvinToCelsius() + "℃", inline: true),
                }
            }.AddGeneratedForFooter(ctx);

            var weatherIcon = weather.Icon;
            weatherIcon = Regex.Replace(weatherIcon, @"01.", "☀");
            weatherIcon = Regex.Replace(weatherIcon, @"02.", "🌥");
            weatherIcon = Regex.Replace(weatherIcon, @"03.", "☁");
            weatherIcon = Regex.Replace(weatherIcon, @"04.", "☁");
            weatherIcon = Regex.Replace(weatherIcon, @"09.", "🌧");
            weatherIcon = Regex.Replace(weatherIcon, @"10.", "🌦");
            weatherIcon = Regex.Replace(weatherIcon, @"11.", "🌩");
            weatherIcon = Regex.Replace(weatherIcon, @"13.", "🌨");
            weatherIcon = Regex.Replace(weatherIcon, @"50.", "🌁");

            embed.AddField(await ctx.Lang("weather.conditions"), $"{weatherIcon} {weather.Description}", true);
            if (city.Clouds != null)
                embed.AddField($"☁ {await ctx.Lang("weather.conditionTypes.clouds")}", $"{city.Clouds.All}%", true);
            if (city.Rain != null)
                embed.AddField($"🌧 {await ctx.Lang("weather.conditionTypes.rain")}", $"{await ctx.Lang("weather.lastHour")}: {city.Rain.LastHour}mm", true);
            if (city.Snow != null)
                embed.AddField($"🌨 {await ctx.Lang("weather.conditionTypes.snow")}", $"{await ctx.Lang("weather.lastHour")}: {city.Snow.LastHour}mm", true);
            if (city.Wind != null)
                embed.AddField($"💨 {await ctx.Lang("weather.conditionTypes.wind")}", $"{city.Wind.Speed}m/s", true);
            await embed.Send(ctx.Message.Channel);
        }
    }
}
