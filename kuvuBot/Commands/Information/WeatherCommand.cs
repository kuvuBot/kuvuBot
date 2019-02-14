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
                    Title = "Weather",
                    Fields =
                {
                    ("City", $"{(weather.Icon.Contains("n") ? "🏙" : "🌆")} {query.Name}", inline: true),
                    ("Country", $"{flag} {query.Sys.Country}", inline: true),
                    ("Temperature", "🌡 " + query.Main.Temperature.CelsiusCurrent.ToString() + "℃", inline: true),
                },
                    Color = Program.Config.EmbedColor,
                    Timestamp = DuckTimestamp.Now,
                    Footer = ($"Generated for {ctx.User.Username}#{ctx.User.Discriminator}", ctx.User.AvatarUrl),
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

                embed.AddField("Weather conditions", $"{weatherIcon} {weather.Description}", true);
                if (query.Clouds != null)
                    embed.AddField("☁ Clouds", $"{query.Clouds.All}%", true);
                if (query.Rain != null)
                    embed.AddField("🌧 Rain", $"H3: {query.Rain.H3}", true);
                if (query.Snow != null)
                    embed.AddField("🌨 Snow", $"H3: {query.Snow.H3}", true);
                if (query.Wind != null)
                    embed.AddField("💨 Wind", $"{query.Wind.SpeedMetersPerSecond}m/s", true);
                await embed.Send(ctx.Message.Channel);
            }
            catch (WebException)
            {
                await ctx.RespondAsync("Unkown city");
            }
        }
    }
}
