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
using System.Threading;
using System.Threading.Tasks;
using kuvuBot.Commands.Attributes;
using kuvuBot.Lang;
using System.Net.Http;
using Castle.Core.Internal;

namespace kuvuBot.Commands.Pictures
{
    public class CatCommand : BaseCommandModule
    {
        private static List<Breed> BreedCache { get; set; }

        private class CatApiImage
        {
            [JsonProperty("breeds")]
            public List<Breed> Breeds { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("url")]
            public Uri Url { get; set; }
        }

        public class Breed
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

        private HttpClient HttpClient { get; } = new HttpClient()
        {
            BaseAddress = new Uri("https://api.thecatapi.com/v1/"),
            DefaultRequestHeaders =
            {
                { "x-api-key", Program.Config.Apis.Cat }
            }
        };

        private async Task RefreshCache(string url)
        {
            BreedCache = JsonConvert.DeserializeObject<List<Breed>>(await HttpClient.GetStringAsync(url));
        }

        [Aliases("kot")]
        [Command("cat"), LocalizedDescription("cat.description")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Cat(CommandContext ctx, [Description("Cat breed, if null random, \"list\" for breed list"), RemainingText] string breed = null)
        {
            await ctx.Channel.TriggerTypingAsync();
            var embed = new ModernEmbedBuilder
            {
                Title = breed switch
                {
                    "list" => "Cat breed list",
                    null => "Random cat",
                    _ => $"Random {breed.ToLower()} cat"
                }
            }.AddGeneratedForFooter(ctx);

            if (BreedCache.IsNullOrEmpty()) await RefreshCache("breeds?limit=100");
            if (breed != null) 
            {
                foreach (var cachedBreed in BreedCache)
                {
                    breed = breed.Replace(cachedBreed.Name, cachedBreed.Id, StringComparison.CurrentCultureIgnoreCase);
                }
            }

            var url = $"images/search?limit=1&breed_id={breed}";

            if (breed == "list")
            {
                embed.AddField(await ctx.Lang("cat.list"), string.Join(", ", BreedCache.Select(b => b.Name)));
                embed.AddField((await ctx.Lang("cat.moreInfo")).Replace("{prefix}", ctx.Prefix), "*info from thecatapi*");
            }
            else
            {
                var response = JsonConvert.DeserializeObject<List<CatApiImage>>(await HttpClient.GetStringAsync(url)).FirstOrDefault();

                if (response == null)
                {
                    embed.AddField(await ctx.Lang("global.error"), await ctx.Lang("cat.breed.unknown"));
                }
                else if (response.Url != null)
                {
                    embed.ImageUrl = response.Url.ToString();
                    var catBreed = response.Breeds.FirstOrDefault();
                    if (catBreed != null)
                    {
                        embed.AddField(await ctx.Lang("cat.breed.breed"), catBreed.Name, true);
                        embed.AddField(await ctx.Lang("cat.breed.origin"), catBreed.Origin, true);
                        embed.AddField(await ctx.Lang("cat.breed.lifeSpan"), catBreed.LifeSpan, true);
                        embed.AddField(await ctx.Lang("cat.breed.description"), catBreed.Description);
                        embed.Url = catBreed.WikipediaUrl.ToString();
                    }
                }
                else
                {
                    embed.AddField(await ctx.Lang("global.error"), ".");
                }
            }
            await embed.Send(ctx.Channel);
        }
    }
}
