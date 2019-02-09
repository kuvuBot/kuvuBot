using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace kuvuBot.Commands.Pictures
{
    public class CatCommand : BaseCommandModule
    {
        static List<Breed> BreedList { get; set; }
        private class CatApiImage
        {
            [JsonProperty("breeds")]
            public List<Breed> Breeds { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("url")]
            public Uri Url { get; set; }
        }
        public partial class Breed
        {
            [JsonProperty("adaptability")]
            public long Adaptability { get; set; }

            [JsonProperty("affection_level")]
            public long AffectionLevel { get; set; }

            [JsonProperty("alt_names")]
            public string AltNames { get; set; }

            [JsonProperty("child_friendly")]
            public long ChildFriendly { get; set; }

            [JsonProperty("country_code")]
            public string CountryCode { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("dog_friendly")]
            public long DogFriendly { get; set; }

            [JsonProperty("energy_level")]
            public long EnergyLevel { get; set; }

            [JsonProperty("grooming")]
            public long Grooming { get; set; }

            [JsonProperty("health_issues")]
            public long HealthIssues { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("intelligence")]
            public long Intelligence { get; set; }

            [JsonProperty("life_span")]
            public string LifeSpan { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("origin")]
            public string Origin { get; set; }

            [JsonProperty("shedding_level")]
            public long SheddingLevel { get; set; }

            [JsonProperty("social_needs")]
            public long SocialNeeds { get; set; }

            [JsonProperty("stranger_friendly")]
            public long StrangerFriendly { get; set; }

            [JsonProperty("temperament")]
            public string Temperament { get; set; }

            [JsonProperty("vocalisation")]
            public long Vocalisation { get; set; }

            [JsonProperty("wikipedia_url")]
            public Uri WikipediaUrl { get; set; }
        }

        string GetWithApi(string url)
        {
            try
            {

                var request = WebRequest.Create(url);
                request.Headers["x-api-key"] = Program.Config.Apis.Cat;
                using (var response = request.GetResponse())
                using (var responseStream = response.GetResponseStream())
                using (var reader = new StreamReader(responseStream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (WebException)
            {
                return null;
            }
        }

        void RefreshList(string url)
        {
            BreedList = JsonConvert.DeserializeObject<List<Breed>>(GetWithApi(url));
        }

        [Aliases("kot")]
        [Command("cat"), Description("Display random cat image")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Cat(CommandContext ctx, [Description("Cat breed, if null random,\"list\" for breed list"), RemainingText] string breed = null)
        {
            await ctx.Channel.TriggerTypingAsync();
            var embed = new ModernEmbedBuilder
            {
                Title = breed == "list" ? "Cat breed list" : breed == null ? "Random cat" : $"Random {breed.ToLower()} cat",
                Color = Program.Config.EmbedColor,
                Timestamp = DuckTimestamp.Now,
                Footer = ($"Generated for {ctx.User.Username}#{ctx.User.Discriminator}", ctx.User.AvatarUrl),
            };

            if (breed != null && BreedList == null) RefreshList("https://api.thecatapi.com/v1/breeds?limit=100");
            if (BreedList != null)
                foreach (var breedo in BreedList)
                {
                    breed = breed.Replace(breedo.Name, breedo.Id, StringComparison.CurrentCultureIgnoreCase);
                }

            var url = $"https://api.thecatapi.com/v1/images/search?limit=1&breed_id={breed}";


            if (breed == "list")
            {
                embed.AddField("Breeds", string.Join(", ", BreedList.Select(b => b.Name)));
                embed.AddField($"For more information type `{ctx.Prefix}cat <breed>`", "*info from thecatapi*");
            }
            else
            {
                var catresponse = JsonConvert.DeserializeObject<List<CatApiImage>>(GetWithApi(url)).FirstOrDefault();

                if (catresponse == null)
                {
                    embed.AddField("Error", "Unkown breed");
                }
                else if (catresponse.Url != null)
                {
                    embed.ImageUrl = catresponse.Url.ToString();
                    var catbreed = catresponse.Breeds.FirstOrDefault();
                    if (catbreed != null)
                    {
                        embed.AddField("Breed", catbreed.Name, true);
                        embed.AddField("Origin", catbreed.Origin, true);
                        embed.AddField("Life span", catbreed.LifeSpan, true);
                        embed.AddField("Description", catbreed.Description);
                        embed.Url = catbreed.WikipediaUrl.ToString();
                    }
                }
                else
                {
                    CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                    TextInfo textInfo = cultureInfo.TextInfo;
                    embed.AddField($"Error", "unkown");
                }
            }
            await embed.Send(ctx.Channel);
        }
    }
}
