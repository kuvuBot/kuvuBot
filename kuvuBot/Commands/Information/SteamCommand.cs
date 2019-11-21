using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using SteamWebAPI2.Interfaces;
using System.Text.RegularExpressions;
using kuvuBot.Data;
using DSharpPlus;
using kuvuBot.Commands.Attributes;
using SteamWebAPI2.Utilities;

namespace kuvuBot.Commands.Information
{
    public class SteamCommand : BaseCommandModule
    {
        public static string ClearHtml(string input)
        {
            input = input.Replace("<br>", "\n");
            return Regex.Replace(input, "<.*?>", string.Empty);
        }

        [Command("steam"), LocalizedDescription("steam.description")]
        [RequireBotPermissions(Permissions.SendMessages)]
        public async Task Steam(CommandContext ctx, [RemainingText] string gameName)
        {
            await ctx.Channel.TriggerTypingAsync();
            var factory = new SteamWebInterfaceFactory(Program.Config.Apis.SteamWebApi);
            using var httpClient = new HttpClient();
            var steamStore = new SteamStore();
            var steamApps = factory.CreateSteamWebInterface<SteamApps>(httpClient);

            var botContext = new BotContext();
            var cacheInfo = await botContext.CacheInfos.FindAsync(CacheType.Steam);
            if (cacheInfo == null || (DateTime.Now - cacheInfo.RefreshedTime).Hours >= 12)
            {
                var apps = await steamApps.GetAppListAsync();
                botContext.SteamAppsCache.RemoveRange(botContext.SteamAppsCache);
                await botContext.SteamAppsCache.AddRangeAsync(apps.Data);

                cacheInfo = new CacheInfo() { Type = CacheType.Steam, RefreshedTime = DateTime.Now };
                if(!botContext.CacheInfos.Contains(cacheInfo))
                {
                    await botContext.CacheInfos.AddAsync(cacheInfo);
                }
                await botContext.SaveChangesAsync();
                await ctx.Channel.TriggerTypingAsync();
            }
            var appModel = botContext.SteamAppsCache.ToList().OrderBy(x=>CommandUtils.LevenshteinDistance(x.Name, gameName)).FirstOrDefault();
            if(appModel == null)
            {
                await ctx.RespondAsync($"Nie znaleziono gry `{gameName}`");
                return;
            }

            var game = await steamStore.GetStoreAppDetailsAsync(appModel.AppId);
            var desc = ClearHtml(game.DetailedDescription);
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
                Color = Program.Config.EmbedColor,
                Timestamp = DuckTimestamp.Now,
                Footer = ($"Generated for {ctx.User.Username}#{ctx.User.Discriminator}", ctx.User.AvatarUrl),
                ImageUrl = game.HeaderImage,
            }.Send(ctx.Message.Channel);
        }
    }

}
