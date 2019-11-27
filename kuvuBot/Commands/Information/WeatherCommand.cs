using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using OpenWeatherAPI;
using System.Text.RegularExpressions;
using System.Net;
using DSharpPlus;
using kuvuBot.Commands.Attributes;
using kuvuBot.Lang;

namespace kuvuBot.Commands.Information
{
    public class WeatherCommand : BaseCommandModule
    {
        [Command("weather"), LocalizedDescription("weather.description")]
        [RequireBotPermissions(Permissions.SendMessages)]
        public async Task Weather(CommandContext ctx, [RemainingText] string city)
        {
            try
            {
                await ctx.Channel.TriggerTypingAsync();
                var openWeatherAPI = new OpenWeatherAPI.OpenWeatherAPI(Program.Config.Apis.OpenWeatherApi);
                var query = openWeatherAPI.Query(city);
                var emoji = DiscordEmoji.FromName(ctx.Client, $":flag_{query.Sys.Country.ToLower()}:");
                var flag = emoji == null ? "🏁" : emoji.ToString();

                var weather = query.Weathers.First();

                var embed = new ModernEmbedBuilder
                {
                    Title = await ctx.Lang("weather.title"),
                    Fields =
                {
                    (await ctx.Lang("weather.city"), $"{(weather.Icon.Contains("n") ? "🏙" : "🌆")} {query.Name}", inline: true),
                    (await ctx.Lang("weather.country"), $"{flag} {query.Sys.Country}", inline: true),
                    (await ctx.Lang("weather.temperature"), "🌡 " + query.Main.Temperature.CelsiusCurrent.ToString() + "℃", inline: true),
                },
                    Color = Program.Config.EmbedColor,
                    Timestamp = DuckTimestamp.Now,
                    Footer = (ctx.Lang("global.footer").Result.Replace("{user}", ctx.User.Name()), ctx.User.AvatarUrl),
                };

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
                if (query.Clouds != null)
                    embed.AddField($"☁ {await ctx.Lang("weather.conditionTypes.clouds")}", $"{query.Clouds.All}%", true);
                if (query.Rain != null)
                    embed.AddField($"🌧 {await ctx.Lang("weather.conditionTypes.rain")}", $"H3: {query.Rain.H3}", true);
                if (query.Snow != null)
                    embed.AddField($"🌨 {await ctx.Lang("weather.conditionTypes.snow")}", $"H3: {query.Snow.H3}", true);
                if (query.Wind != null)
                    embed.AddField($"💨 {await ctx.Lang("weather.conditionTypes.wind")}", $"{query.Wind.SpeedMetersPerSecond}m/s", true);
                await embed.Send(ctx.Message.Channel);
            }
            catch (WebException)
            {
                await ctx.RespondAsync(await ctx.Lang("weather.unknown"));
            }
        }
    }
}
