using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using SteamWebAPI2.Interfaces;
using System.Text.RegularExpressions;
using kuvuBot.Data;

namespace kuvuBot.Commands.Information
{
    public class SteamCommand : BaseCommandModule
    {
        public static int LevenshteinDistance(string source, string target)
        {
            // degenerate cases
            if (source == target) return 0;
            if (source.Length == 0) return target.Length;
            if (target.Length == 0) return source.Length;

            // create two work vectors of integer distances
            int[] v0 = new int[target.Length + 1];
            int[] v1 = new int[target.Length + 1];

            // initialize v0 (the previous row of distances)
            // this row is A[0][i]: edit distance for an empty s
            // the distance is just the number of characters to delete from t
            for (int i = 0; i < v0.Length; i++)
                v0[i] = i;

            for (int i = 0; i < source.Length; i++)
            {
                // calculate v1 (current row distances) from the previous row v0

                // first element of v1 is A[i+1][0]
                //   edit distance is delete (i+1) chars from s to match empty t
                v1[0] = i + 1;

                // use formula to fill in the rest of the row
                for (int j = 0; j < target.Length; j++)
                {
                    var cost = (source[i] == target[j]) ? 0 : 1;
                    v1[j + 1] = Math.Min(v1[j] + 1, Math.Min(v0[j + 1] + 1, v0[j] + cost));
                }

                // copy v1 (current row) to v0 (previous row) for next iteration
                for (int j = 0; j < v0.Length; j++)
                    v0[j] = v1[j];
            }

            return v1[target.Length];
        }

        public static string ClearHTML(string input)
        {
            input = input.Replace("<br>", "\n");
            return Regex.Replace(input, "<.*?>", string.Empty);
        }

        [Command("steam"), Description("Informations about steam game")]
        public async Task Steam(CommandContext ctx, [RemainingText] string gameName)
        {
            await ctx.Channel.TriggerTypingAsync();
            var steamInterface = new SteamStore();
            var steamApps = new SteamApps(Program.Config.Apis.SteamWebApi);

            var botContext = new BotContext();
            var CacheInfo = await botContext.CacheInfos.FindAsync(CacheType.Steam);
            if (CacheInfo == null || (DateTime.Now - CacheInfo.RefreshedTime).Hours >= 12)
            {
                var apps = await steamApps.GetAppListAsync();
                botContext.SteamAppsCache.RemoveRange(botContext.SteamAppsCache);
                await botContext.SteamAppsCache.AddRangeAsync(apps.Data);

                CacheInfo = new CacheInfo() { Type = CacheType.Steam, RefreshedTime = DateTime.Now };
                if(!botContext.CacheInfos.Contains(CacheInfo))
                {
                    await botContext.CacheInfos.AddAsync(CacheInfo);
                }
                await botContext.SaveChangesAsync();
                await ctx.Channel.TriggerTypingAsync();
            }
            var appModel = botContext.SteamAppsCache.OrderBy(x=>LevenshteinDistance(x.Name, gameName)).FirstOrDefault();
            if(appModel == null)
            {
                await ctx.RespondAsync($"Nie znaleziono gry `{gameName}`");
                return;
            }

            var game = await steamInterface.GetStoreAppDetailsAsync(appModel.AppId);
            string desc = ClearHTML(game.DetailedDescription);
            if(desc.Length >= 2000)
            {
                desc = desc.Substring(0, Math.Min(desc.Length, 2000-3));
                desc += "...";
            }
            else
            {
                desc = desc.Substring(0, Math.Min(desc.Length, 2000));
            }
            await new ModernEmbedBuilder
            {
                Title = $"{game.Name}",
                Url = game.Website,
                Description = desc,
                Fields =
                    {
                        ("F2P", game.IsFree ? "Tak" : "Nie", inline: true),
                        ("Data wydania", (game.ReleaseDate.ComingSoon ? "[Coming soon] " : "") + game.ReleaseDate.Date, inline: true),
                        (game.Genres.Length > 1 ? "Gatunki" : "Gatunek", string.Join(", ", game.Genres.Select(x=>x.Description)), inline: true),
                        (game.Categories.Length > 1 ? "Kategorie" : "Kategoria", string.Join(", ", game.Categories.Select(x=>x.Description)), inline: true),
                        (game.Developers.Length > 1 ? "Programiści" : "Programista", string.Join(", ", game.Developers), inline: true),
                        (game.Publishers.Length > 1 ? "Wydawcy" : "Wydawca", string.Join(", ", game.Publishers), inline: true),
                    },
                Color = new DuckColor(33, 150, 243),
                Timestamp = DuckTimestamp.Now,
                Footer = ($"Generated for {ctx.User.Username}#{ctx.User.Discriminator}", ctx.User.AvatarUrl),
                ImageUrl = game.HeaderImage,
            }.Send(ctx.Message.Channel);
        }
    }

}
