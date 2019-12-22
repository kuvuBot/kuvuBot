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
using kuvuBot.Commands.Attributes;

namespace kuvuBot.Commands.Moderation
{
    public class BroadcastCommand : BaseCommandModule
    {
        [Aliases("ogłoszenie", "ogloszenie", "bc")]
        [Command("broadcast"), LocalizedDescription("broadcast.description")]
        [RequireUserPermissions(Permissions.ManageGuild), RequireBotPermissions(Permissions.SendMessages)]
        public async Task Broadcast(CommandContext ctx, [RemainingText, Description("Broadcast")] string content)
        {
            await new ModernEmbedBuilder
            {
                Title = await ctx.Lang("broadcast.title"),
                Description = content
            }.AddGeneratedForFooter(ctx).Send(ctx.Message.Channel);
        }
    }
}