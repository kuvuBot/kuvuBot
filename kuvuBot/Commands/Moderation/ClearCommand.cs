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
    public class ClearCommand : BaseCommandModule
    {
        [Command("clear"), LocalizedDescription("clear.description")]
        [RequireUserPermissions(Permissions.ManageMessages), RequireBotPermissions(Permissions.SendMessages)]
        public async Task Clear(CommandContext ctx, [Description("Messages count")] int messageCount)
        {
            await ctx.Channel.DeleteMessagesAsync(await ctx.Channel.GetMessagesAsync(messageCount + 1), $"cleared by {ctx.Message.Id}");
            await ctx.Channel.SendAutoRemoveMessageAsync(TimeSpan.FromSeconds(1.5), ctx.Lang("clear.success").Result.Replace("{count}", messageCount.ToString()));
        }
    }
}