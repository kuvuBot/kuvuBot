using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using Newtonsoft.Json;
using System.Net;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using kuvuBot.Commands.Attributes;
using kuvuBot.Lang;

namespace kuvuBot.Commands.Fun
{
    public class CurrencyCommand : BaseCommandModule
    {
        public partial class ExchangeRateApiResponse
        {
            [JsonProperty("base")]
            public string Base { get; set; }

            [JsonProperty("date")]
            public string Date { get; set; }

            [JsonProperty("rates")]
            public Dictionary<string, double> Rates { get; set; }
        }

        [Command("currency"), LocalizedDescription("currency.description")]
        [RequireBotPermissions(Permissions.SendMessages)]
        public async Task Currency(CommandContext ctx, string baseCur, string targetCur, double amount = 1)
        {
            using (var wc = new WebClient())
            {
                try
                {
                    var response = JsonConvert.DeserializeObject<ExchangeRateApiResponse>(wc.DownloadString($"https://api.exchangeratesapi.io/latest?base={baseCur.ToUpper()}"));
                    if (response.Rates.ContainsKey(targetCur.ToUpper()) || string.Equals(targetCur, "ALL", StringComparison.OrdinalIgnoreCase))
                    {
                        var embed = new ModernEmbedBuilder
                        {
                            Title = await ctx.Lang("currency.title"),
                            Url = "https://exchangeratesapi.io/",
                        }.AddGeneratedForFooter(ctx);

                        embed.AddField($"{amount} {response.Base}",
                            string.Equals(targetCur, "ALL", StringComparison.OrdinalIgnoreCase)
                                ? string.Join("\n", response.Rates.Select(x => $"**{x.Value}** {x.Key}"))
                                : $"**{response.Rates[targetCur.ToUpper()]}** {targetCur.ToUpper()}");

                        await embed.Send(ctx.Message.Channel);
                    }
                    else
                    {
                        await ctx.RespondAsync(await ctx.Lang("currency.badTarget"));
                    }
                }
                catch (Exception)
                {
                    await ctx.RespondAsync(await ctx.Lang("currency.badBase"));
                }
            }
        }
    }
}
