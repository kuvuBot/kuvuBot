using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using kuvuBot.Lang;
using DSharpPlus;

namespace kuvuBot.Commands.Moderation
{
    public class BroadcastCommand : BaseCommandModule
    {
        [Aliases("ogłoszenie", "ogloszenie", "bc")]
        [Command("broadcast"), Description("Makes broadcast")]
        public async Task Broadcast(CommandContext ctx, [RemainingText, Description("Broadcast")] string content = null)
        {
            if (content == null)
            {
                await ctx.RespondAsync(await ctx.Lang("global.badArguments"));
                return;
            }
            if (!await ctx.HasPermission(Permissions.ManageGuild))
            {
                return;
            }
            var message = await new ModernEmbedBuilder
            {
                Title = await ctx.Lang("broadcast.broadcast"),
                Description = content,
                Color = Program.Config.EmbedColor,
                Timestamp = DuckTimestamp.Now,
                Footer = ($"Generated for {ctx.User.Username}#{ctx.User.Discriminator}", ctx.User.AvatarUrl),
            }.Send(ctx.Message.Channel);
        }
    }
}