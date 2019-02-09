using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using kuvuBot.Data;
using Microsoft.EntityFrameworkCore.Internal;

namespace kuvuBot.Commands.Information
{
    public class LeaderboardCommand : BaseCommandModule
    {
        [Command("leaderboard"), Description("Shows level leaderboard"), Aliases("ranking", "toplvl", "toplevel")]
        [RequireBotPermissions(Permissions.SendMessages)]
        public async Task Leaderboard(CommandContext ctx)
        {
            var botContext = new BotContext();

            var users = botContext.Users.Where(u => u.Guild.GuildId == ctx.Guild.Id).OrderByDescending(u => u.Exp).Take(10);
            await new ModernEmbedBuilder
            {
                Title = "Level leaderboard",
                Description = users.Select(u=>$"{users.IndexOf(u)+1}. {ctx.Guild.GetMemberAsync(u.DiscordUser).Result.Name(true)} Level: {u.GetLevel()} Exp: {u.Exp}").Join("\n"),
                Color = Program.Config.EmbedColor,
                Timestamp = DuckTimestamp.Now,
                Footer = ($"Generated for {ctx.User.Username}#{ctx.User.Discriminator}", ctx.User.AvatarUrl),
            }.Send(ctx.Message.Channel);
        }
    }
}
