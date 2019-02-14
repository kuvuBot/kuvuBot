using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using DSharpPlus;
using Newtonsoft.Json;
using System.Net;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using kuvuBot.Commands.Attributes;

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
                    if(response.Rates.ContainsKey(targetCur.ToUpper()) || string.Equals(targetCur, "ALL", StringComparison.OrdinalIgnoreCase))
                    {
                        var embed = new ModernEmbedBuilder
                        {
                            Title = "Currency calculator",
                            Url = "https://exchangeratesapi.io/",
                            Color = Program.Config.EmbedColor,
                            Timestamp = DateTimeOffset.Parse(response.Date),
                            Footer = ($"Generated for {ctx.User.Name()}", ctx.User.AvatarUrl),
                        };

                        embed.AddField($"{amount} {response.Base}",
                            string.Equals(targetCur, "ALL", StringComparison.OrdinalIgnoreCase)
                                ? string.Join("\n", response.Rates.Select(x => $"**{x.Value}** {x.Key}"))
                                : $"**{response.Rates[targetCur.ToUpper()]}** {targetCur.ToUpper()}");

                        await embed.Send(ctx.Message.Channel);
                    }else
                    {
                        await ctx.RespondAsync("Bad target currency");
                    }
                }
                catch (Exception)
                {
                    await ctx.RespondAsync("Bad base currency");
                }
            }
        }
    }
}
