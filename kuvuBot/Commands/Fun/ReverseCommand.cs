using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
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
            var charArray = message.ToCharArray();
            Array.Reverse( charArray );
            await ctx.RespondAsync(charArray.ToString());
        }
    }
}