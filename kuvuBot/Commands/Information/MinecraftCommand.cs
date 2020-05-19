using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using Newtonsoft.Json;
using DSharpPlus;
using kuvuBot.Commands.Attributes;
using kuvuBot.Lang;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;

namespace kuvuBot.Commands.Information
{
    public class TopkaMcApiResponse
    {
        [JsonProperty("error")]
        public ErrorType? Error { get; set; }

        [JsonProperty("address")]
        public Address Address { get; set; }

        [JsonProperty("version")]
        public Version Version { get; set; }

        [JsonProperty("players")]
        public Players Players { get; set; }

        [JsonProperty("favicon")]
        public string Favicon { get; set; }

        [JsonProperty("motd")]
        public Motd Motd { get; set; }

        [JsonProperty("query")]
        public Query Query { get; set; }

        public enum ErrorType
        {
            [EnumMember(Value = "SERVER_UNREACHABLE")]
            ServerUnreachable,

            [EnumMember(Value = "INVALID_ADDRESS")]
            InvalidAddress
        }
    }

    public class Address
    {
        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("port")]
        public long Port { get; set; }
    }

    public class Motd
    {
        [JsonProperty("raw")]
        public string Raw { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("normalized")]
        public string Normalized { get; set; }

        [JsonProperty("html")]
        public string Html { get; set; }
    }

    public class Players
    {
        [JsonProperty("list")]
        public List<object> List { get; set; }

        [JsonProperty("max")]
        public long Max { get; set; }

        [JsonProperty("online")]
        public long Online { get; set; }
    }

    public class Query
    {
        [JsonProperty("hostname")]
        public string Hostname { get; set; }

        [JsonProperty("hostip")]
        public string Hostip { get; set; }

        [JsonProperty("plugins")]
        public string Plugins { get; set; }

        [JsonProperty("numplayers")]
        public long Numplayers { get; set; }

        [JsonProperty("gametype")]
        public string Gametype { get; set; }

        [JsonProperty("maxplayers")]
        public long Maxplayers { get; set; }

        [JsonProperty("hostport")]
        public long Hostport { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("map")]
        public string Map { get; set; }

        [JsonProperty("game_id")]
        public string GameId { get; set; }
    }

    public class Version
    {
        [JsonProperty("raw")]
        public Raw Raw { get; set; }

        [JsonProperty("range")]
        public Range Range { get; set; }
    }

    public class Range
    {
        [JsonProperty("minimal_version")]
        public ImalVersion MinimalVersion { get; set; }

        [JsonProperty("maximal_version")]
        public ImalVersion MaximalVersion { get; set; }

        [JsonProperty("display")]
        public string Display { get; set; }
    }

    public class ImalVersion
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("full_name")]
        public string FullName { get; set; }

        [JsonProperty("protocol")]
        public long Protocol { get; set; }
    }

    public class Raw
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("protocol")]
        public long Protocol { get; set; }
    }

    public class MinecraftCommand : BaseCommandModule
    {
        [Command("minecraft"), LocalizedDescription("minecraft.description")]
        [RequireBotPermissions(Permissions.SendMessages)]
        public async Task Minecraft(CommandContext ctx, string ip)
        {
            using var httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync($"https://www.topkamc.pl/api/info/{ip}");
            var response = JsonConvert.DeserializeObject<TopkaMcApiResponse>(json);

            if (response.Error == null)
            {
                var embed = new ModernEmbedBuilder
                {
                    Title = await ctx.Lang("minecraft.title"),
                    Fields =
                        {
                            (await ctx.Lang("minecraft.status"), await ctx.Lang("minecraft.online"), inline: true),
                            (await ctx.Lang("minecraft.players"), $"{response.Players.Online}/{response.Players.Max}", inline: true),
                            (await ctx.Lang("minecraft.version"), $"{response.Version.Raw.Name}", inline: true),
                            (await ctx.Lang("minecraft.motd"), $"```{HttpUtility.HtmlDecode(response.Motd.Text)}```", inline: false),
                        },
                    ThumbnailUrl = "attachment://favicon.png"
                }.AddGeneratedForFooter(ctx);

                var favicon = Convert.FromBase64String(Regex.Replace(response.Favicon, @"data:image/.+,", ""));
                await ctx.RespondWithFileAsync(embed: embed.Build(), fileData: new MemoryStream(favicon, 0, favicon.Length), fileName: "favicon.png");
            }
            else
            {
                await new ModernEmbedBuilder
                {
                    Title = "Minecraft server status",
                    Fields =
                        {
                            (await ctx.Lang("minecraft.status"), await ctx.Lang("minecraft.offline"), inline: true),
                            ("Error", Enum.GetName(typeof(TopkaMcApiResponse.ErrorType), response.Error.Value))
                        }
                }.AddGeneratedForFooter(ctx).Send(ctx.Message.Channel);
            }
        }
    }
}
