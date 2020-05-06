using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using kuvuBot.Commands.Attributes;
using kuvuBot.Core.Commands;
using kuvuBot.Data;
using kuvuBot.Lang;
using Microsoft.EntityFrameworkCore.Internal;

namespace kuvuBot.Commands.Information
{
    public class LeaderboardCommand : BaseCommandModule
    {
        [Command("leaderboard"), LocalizedDescription("leaderboard.description"), Aliases("ranking", "toplvl", "toplevel")]
        [RequireBotPermissions(Permissions.SendMessages)]
        public async Task Leaderboard(CommandContext ctx)
        {
            var emojis = new Dictionary<int, string>
            {
                [1] = ":first_place:",
                [2] = ":second_place:",
                [3] = ":third_place:",
                [4] = ":four:",
                [5] = ":five:",
                [6] = ":six:",
                [7] = ":seven:",
                [8] = ":eight:",
                [9] = ":nine:",
                [10] = ":keycap_ten:"
            };

            var botContext = new BotContext();

            var i = 1;
            var users = botContext.Users.Where(u => u.Guild.GuildId == ctx.Guild.Id).OrderByDescending(u => u.Exp).Take(10).ToList();
            await new ModernEmbedBuilder
            {
                Title = ":trophy: " + await ctx.Lang("leaderboard.title"),
                Description = (await users.SelectAsync(async u => $"**{emojis[i++]} {(await ctx.Guild.GetMemberAsync(u.DiscordUser)).Name(true)}** - level: {u.GetLevel()} ({u.Exp}/{KuvuUser.ConvertLevelToExp(u.GetLevel() + 1)} exp)")).Join("\n"),
                Url = $"https://kuvubot.xyz/leaderboard/{ctx.Guild.Id}"
            }.AddGeneratedForFooter(ctx).Send(ctx.Message.Channel);
        }
    }
}
