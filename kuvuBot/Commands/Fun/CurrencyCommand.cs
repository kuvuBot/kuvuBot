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

        [Command("currency"), Description("Check currency rate relative to other")]
        [RequireBotPermissions(Permissions.SendMessages)]
        public async Task Currency(CommandContext ctx, string baseCur, string targetCur, double amount = 1)
        {
            using (var wc = new WebClient())
            {
                try
                {
                    var response = JsonConvert.DeserializeObject<ExchangeRateApiResponse>(wc.DownloadString($"https://api.exchangeratesapi.io/latest?base={baseCur.ToUpper()}"));
                    if(response.Rates.ContainsKey(targetCur.ToUpper()) || targetCur.ToUpper() == "ALL")
                    {
                        var embed = new ModernEmbedBuilder
                        {
                            Title = "Currency calculator",
                            Url = "https://exchangeratesapi.io/",
                            Color = Program.Config.EmbedColor,
                            Timestamp = DateTimeOffset.Parse(response.Date),
                            Footer = ($"Generated for {ctx.User.Name()}", ctx.User.AvatarUrl),
                        };
                        
                        if(targetCur.ToUpper() == "ALL")
                        {
                            embed.AddField($"{amount} {response.Base}", string.Join("\n", response.Rates.Select(x=>$"**{x.Value}** {x.Key}")));
                        }
                        else
                        {
                            embed.AddField($"{amount} {response.Base}", $"**{response.Rates[targetCur.ToUpper()]}** {targetCur.ToUpper()}");
                        }

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
