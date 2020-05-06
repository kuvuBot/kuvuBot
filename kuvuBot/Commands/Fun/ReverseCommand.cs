using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using kuvuBot.Commands.Attributes;

namespace kuvuBot.Commands.Fun
{
    public class ReverseCommand : BaseCommandModule
    {
        [RequireBotPermissions(Permissions.SendMessages)]
        [Command("reverse"), LocalizedDescription("reverse.description"), Aliases("odwróć")]
        public async Task ReverseText(CommandContext ctx, [RemainingText] string message)
        {
            message.RequireRemainingText();
            await ctx.RespondAsync(new string(message.Reverse().ToArray()));
        }
    }
}