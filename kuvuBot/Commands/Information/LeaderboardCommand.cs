using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using kuvuBot.Commands.Attributes;
using kuvuBot.Data;
using Microsoft.EntityFrameworkCore.Internal;
using kuvuBot.Lang;

namespace kuvuBot.Commands.Information
{
    public class LeaderboardCommand : BaseCommandModule
    {
        [Command("leaderboard"), LocalizedDescription("leaderboard.description"), Aliases("ranking", "toplvl", "toplevel")]
        [RequireBotPermissions(Permissions.SendMessages)]
        public async Task Leaderboard(CommandContext ctx)
        {
            var botContext = new BotContext();

            var users = botContext.Users.Where(u => u.Guild.GuildId == ctx.Guild.Id).OrderByDescending(u => u.Exp).Take(10);
            await new ModernEmbedBuilder
            {
                Title = await ctx.Lang("leaderboard.title"),
                Description = users.Select(u => $"{users.IndexOf(u) + 1}. {ctx.Guild.GetMemberAsync(u.DiscordUser).Result.Name(true)} Level: {u.GetLevel()} Exp: {u.Exp}").Join("\n"),
            }.AddGeneratedForFooter(ctx).Send(ctx.Message.Channel);
        }
    }
}
