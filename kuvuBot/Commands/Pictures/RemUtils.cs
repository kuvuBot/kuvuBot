using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using System;

namespace kuvuBot.Commands.Pictures
{
    public class RemResponse
    {
        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("nsfw")]
        public bool Nsfw { get; set; }
    }

    public class RemUtils
    {
        public class ImageType
        {
            public string Name { get; set; }
            public string PastVerb { get; set; }
            public ImageType(string name, string pastVerb)
            {
                Name = name;
                PastVerb = pastVerb;
            }

            public static ImageType Cry => new ImageType("cry", "cried");
            public static ImageType Cuddle => new ImageType("cuddle", "cuddled");
            public static ImageType Hug => new ImageType("hug", "hugged");
            public static ImageType Kiss => new ImageType("kiss", "kissed");
            public static ImageType Lewd => new ImageType("lewd", "lewded");
            public static ImageType Lick => new ImageType("lick", "licked");
            public static ImageType Nom => new ImageType("nom", "nommed");
            public static ImageType Nyan => new ImageType("nyan", "nyaned");
            public static ImageType Owo => new ImageType("owo", "owoed");
            public static ImageType Pat => new ImageType("pat", "patted");
            public static ImageType Pout => new ImageType("pout", "pout");
            public static ImageType Rem => new ImageType("rem", "remmed");
            public static ImageType Slap => new ImageType("slap", "slapped");
            public static ImageType Smug => new ImageType("smug", "smugged");
            public static ImageType Stare => new ImageType("stare", "stared");
            public static ImageType Tickle => new ImageType("tickle", "tickled");
            public static ImageType Triggered => new ImageType("triggered", "triggered");
            public static ImageType NnsfwGtn => new ImageType("nsfw-gtn", "nsfw-gtned");
            public static ImageType Potato => new ImageType("potato", "potated");
            public static ImageType Kermit => new ImageType("kermit", "kermitted");
        }
        public static async Task<DiscordMessage> SendRemEmbed(CommandContext ctx, ImageType type, DiscordUser target = null)
        {
            using (WebClient wc = new WebClient())
            {
                var response = JsonConvert.DeserializeObject<RemResponse>(wc.DownloadString("https://rra.ram.moe/i/r?type=" + type.Name));
                var embed = new ModernEmbedBuilder
                {
                    Title = target == null ? type.Name : $"{ctx.User.Username} {type.PastVerb} {target.Username}",
                    Color = new DuckColor(33, 150, 243),
                    Timestamp = DuckTimestamp.Now,
                    ImageUrl = "https://rra.ram.moe" + response.Path,
                    Footer = ($"Generated for {ctx.User.Username}#{ctx.User.Discriminator}", ctx.User.AvatarUrl),
                };
                return await embed.Send(ctx.Channel);
            }
        }
    }
}
