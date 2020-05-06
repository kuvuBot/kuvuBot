using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using kuvuBot.Commands;
using System.Runtime.Serialization;
using kuvuBot.Core.Commands;

namespace BadoszApiModule
{
    public class BadoszApi
    {
        private WebClient WebClient { get; }

        public BadoszApi(string token)
        {
            var wc = new WebClient();
            wc.Headers.Add("Authorization", token);
            WebClient = wc;
        }

        public async Task<string> GetJson(BadoszEndpoint endpoint, NameValueCollection parameters = null)
        {
            var uri = new UriBuilder("https" + $"://api.badosz.com/{endpoint.GetEnumMemberValue()}")
            {
                Query = parameters?.ToString()
            };
            return await WebClient.DownloadStringTaskAsync(uri.Uri);
        }

        public async Task<Stream> GetStream(BadoszEndpoint endpoint, NameValueCollection parameters = null)
        {
            var uri = new UriBuilder("https" + $"://api.badosz.com/{endpoint.GetEnumMemberValue()}")
            {
                Query = parameters?.ToString()
            };
            return new MemoryStream(await WebClient.DownloadDataTaskAsync(uri.Uri));
        }

        public async Task<DiscordMessage> SendEmbedImage(CommandContext ctx, BadoszEndpoint endpoint, string title = null, NameValueCollection parameters = null)
        {
            await using var stream = await GetStream(endpoint, parameters);
            var name = endpoint.GetEnumMemberValue();
            var embed = new ModernEmbedBuilder
            {
                Title = title ?? (name ?? throw new InvalidOperationException()).First().ToString().ToUpper() + name.Substring(1),
                ImageUrl = "attachment://image.gif"
            }.AddGeneratedForFooter(ctx);
            return await ctx.RespondWithFileAsync(embed: embed.Build(), fileData: stream, fileName: $"image.gif");
        }

        public enum BadoszEndpoint
        {
            [EnumMember(Value = "ant")]
            Ant,

            [EnumMember(Value = "bird")]
            Bird,

            [EnumMember(Value = "bee")]
            Bee,

            [EnumMember(Value = "rabbit")]
            Rabbit,

            [EnumMember(Value = "catgirl")]
            CatGirl,

            [EnumMember(Value = "cuddle")]
            Cuddle,

            [EnumMember(Value = "dog")]
            Dog,

            [EnumMember(Value = "feed")]
            Feed,

            [EnumMember(Value = "fox")]
            Fox,

            [EnumMember(Value = "hug")]
            Hug,

            [EnumMember(Value = "jesus")]
            Jesus,

            [EnumMember(Value = "kiss")]
            Kiss,

            [EnumMember(Value = "pat")]
            Pat,

            [EnumMember(Value = "poke")]
            Poke,

            [EnumMember(Value = "shibe")]
            Shibe,

            [EnumMember(Value = "tickle")]
            Tickle,

            [EnumMember(Value = "advice")]
            Advice,

            [EnumMember(Value = "cat")]
            Cat,

            [EnumMember(Value = "chucknorris")]
            ChuckNorris,

            [EnumMember(Value = "dadjoke")]
            DadJoke,

            [EnumMember(Value = "fact")]
            Fact,

            [EnumMember(Value = "Why")]
            Why,

            [EnumMember(Value = "yomomma")]
            YoMomma,

            [EnumMember(Value = "base64")]
            Base64,

            [EnumMember(Value = "binary")]
            Binary,

            [EnumMember(Value = "blurple")]
            Blurple,

            [EnumMember(Value = "changemymind")]
            ChangeMyMind,

            [EnumMember(Value = "color")]
            Color,

            [EnumMember(Value = "colorify")]
            Colorify,

            [EnumMember(Value = "decode-base64")]
            DecodeBase64,

            [EnumMember(Value = "decode-hex")]
            DecodeHex,

            [EnumMember(Value = "endpoints")]
            Endpoints,

            [EnumMember(Value = "excuseme")]
            ExcuseMe,

            [EnumMember(Value = "flip")]
            Flip,

            [EnumMember(Value = "hex")]
            Hex,

            [EnumMember(Value = "invert")]
            Invert,

            [EnumMember(Value = "morse")]
            Morse,

            [EnumMember(Value = "reverse")]
            Reverse,

            [EnumMember(Value = "trump")]
            Trump,

            [EnumMember(Value = "vaporwave")]
            VaporWave,

            [EnumMember(Value = "wanted")]
            Wanted,

            [EnumMember(Value = "wasted")]
            Wasted
        }
    }
}
