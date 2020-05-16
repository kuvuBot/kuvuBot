using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using kuvuBot.Commands.Attributes;
using kuvuBot.Lang;

namespace kuvuBot.Commands.Pictures
{
    public class DogCommand : BaseCommandModule
    {
        private class DogApiResponse
        {
            [JsonProperty("status")] public string Status { get; set; }
            [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)] public string Code { get; set; }
            [JsonProperty("message")] public string Message { get; set; }
        }

        [Aliases("pies")]
        [Command("dog"), LocalizedDescription("dog.description")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Dog(CommandContext ctx, [Description("Dog breed, if null random")] string breed = null)
        {
            await ctx.Channel.TriggerTypingAsync();
            var embed = new ModernEmbedBuilder
            {
                Title = breed == null ? await ctx.Lang("dog.random") : (await ctx.Lang("dog.randomBreed")).Replace("{breed}", breed),
            }.AddGeneratedForFooter(ctx);
            var url = breed == null ? "https://dog.ceo/api/breeds/image/random" : $"https://dog.ceo/api/breed/{breed}/images/random";

            WebResponse response = null;
            try
            {
                var request = WebRequest.Create(url);
                response = request.GetResponse();

            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    response = (HttpWebResponse)ex.Response;
                }
            }

            using (var responseStream = response.GetResponseStream())
            using (var reader = new StreamReader(responseStream))
            {
                var dogresponse = JsonConvert.DeserializeObject<DogApiResponse>(await reader.ReadToEndAsync());

                if (dogresponse.Status == "success")
                {
                    embed.ImageUrl = dogresponse.Message;
                }
                else
                {
                    CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                    TextInfo textInfo = cultureInfo.TextInfo;
                    embed.AddField($"{textInfo.ToTitleCase(dogresponse.Status)} {dogresponse.Code}", dogresponse.Message);
                }
                await embed.Send(ctx.Channel);
                response.Close(); response.Dispose();
            }
        }
    }
}
