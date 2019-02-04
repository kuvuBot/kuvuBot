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
    public class ClearCommand : BaseCommandModule
    {
        [Command("clear"), Description("Removes specified amount of messages")]
        public async Task Clear(CommandContext ctx, [Description("Messages count")] int messagecount)
        {
            if (!await ctx.HasPermission(Permissions.ManageMessages))
            {
                return;
            }
            await ctx.Channel.DeleteMessagesAsync(await ctx.Channel.GetMessagesAsync(messagecount + 1), $"cleared by {ctx.Message.Id}");
            await ctx.Channel.SendAutoRemoveMessageAsync(TimeSpan.FromSeconds(1.5), $"👌, deleted {messagecount} messages");
        }
    }
}