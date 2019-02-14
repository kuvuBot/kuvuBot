using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using System.Globalization;
using Newtonsoft.Json;
using System.Net;
using DSharpPlus;
using kuvuBot.Commands.Attributes;

namespace kuvuBot.Commands.Information
{
    public class MinecraftCommand : BaseCommandModule
    {
        public partial class McApiResponse
        {
            [JsonProperty("online")]
            public bool Online { get; set; }

            [JsonProperty("status")]
            public bool Status { get; set; }

            [JsonProperty("favicon_base64")]
            public string FaviconBase64 { get; set; }

            [JsonProperty("favicon")]
            public Uri Favicon { get; set; }

            [JsonProperty("source")]
            public string Source { get; set; }

            [JsonProperty("took")]
            public double Took { get; set; }

            [JsonProperty("cache")]
            public Cache Cache { get; set; }

            [JsonProperty("dns")]
            public Dns Dns { get; set; }

            [JsonProperty("version")]
            public Version Version { get; set; }

            [JsonProperty("players")]
            public Players Players { get; set; }

            [JsonProperty("description")]
            public Description Description { get; set; }

            [JsonProperty("modinfo")]
            public Modinfo Modinfo { get; set; }

            [JsonProperty("fetch")]
            public DateTimeOffset Fetch { get; set; }
        }

        public partial class Cache
        {
            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("ttl")]
            public long Ttl { get; set; }

            [JsonProperty("insertion_time")]
            public DateTimeOffset InsertionTime { get; set; }
        }

        public partial class Description
        {
            [JsonProperty("text")]
            public string Text { get; set; }
        }

        public partial class Dns
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("port")]
            public long Port { get; set; }

            [JsonProperty("ip")]
            public string Ip { get; set; }
        }

        public partial class Modinfo
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("modList")]
            public List<object> ModList { get; set; }
        }

        public partial class Players
        {
            [JsonProperty("online")]
            public long Online { get; set; }

            [JsonProperty("max")]
            public long Max { get; set; }

            [JsonProperty("sample")]
            public List<Sample> Sample { get; set; }
        }

        public partial class Sample
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("id")]
            public Guid Id { get; set; }
        }

        public partial class Version
        {
            [JsonProperty("protocol")]
            public long Protocol { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        }

        [Command("minecraft"), LocalizedDescription("minecraft.description")]
        [RequireBotPermissions(Permissions.SendMessages)]
        public async Task Minecraft(CommandContext ctx, string ip, ulong? port = null)
        {
            using (var wc = new WebClient())
            {
                var url = $"https://eu.mc-api.net/v3/server/ping/{ip}";
                if (port.HasValue) url = url + $":{port}";
                var json = await wc.DownloadStringTaskAsync(url);
                var response = JsonConvert.DeserializeObject<McApiResponse>(json);

                if (response.Status)
                {
                    await new ModernEmbedBuilder
                    {
                        Title = "Minecraft server status",
                        Fields =
                        {
                            ("Status", "Online", inline: true),
                            ("Players", $"{response.Players.Online}/{response.Players.Max}", inline: true),
                            ("Version", $"{response.Version.Name}", inline: true),
                            ("Motd", $"```{response.Description.Text}```", inline: false),
                        },
                        Color = new DuckColor(139, 195, 74),
                        Timestamp = DuckTimestamp.Now,
                        Footer = ($"Generated for {ctx.User.Name()}", ctx.User.AvatarUrl),
                        ThumbnailUrl = response.Favicon.ToString(),
                    }.Send(ctx.Message.Channel);
                }
                else
                {
                    await new ModernEmbedBuilder
                    {
                        Title = "Minecraft server status",
                        Fields =
                        {
                            ("Status", "Offline", inline: true),
                        },
                        Color = new DuckColor(244, 67, 54),
                        Timestamp = DuckTimestamp.Now,
                        Footer = ($"Generated for {ctx.User.Name()}", ctx.User.AvatarUrl),
                    }.Send(ctx.Message.Channel);
                }
            }
        }
    }
}
