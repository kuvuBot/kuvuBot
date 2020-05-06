using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;
using DSharpPlus;
using kuvuBot.Commands.Attributes;
using kuvuBot.Lang;

namespace kuvuBot.Commands.Fun
{
    public class DiceCommand : BaseCommandModule
    {
        [Aliases("kostka")]
        [Command("dice"), LocalizedDescription("dice.description")]
        [RequireBotPermissions(Permissions.SendMessages)]
        public async Task Dice(CommandContext ctx, int? walls = null)
        {
            walls ??= 4;
            if (walls < 2)
            {
                await ctx.RespondAsync(await ctx.Lang("dice.minimum"));
            }
            else
            {
                var result = new Random().Next(1, walls.Value);
                await ctx.RespondAsync((await ctx.Lang("dice.result")).Replace("{result}", result.ToString()));
            }
        }
    }
}