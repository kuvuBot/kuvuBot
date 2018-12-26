using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace kuvuBot.Commands.Fun
{
    public class DiceCommand : BaseCommandModule
    {
        [Aliases("kostka")]
        [Command("dice"), Description("Roll dice")]
        public async Task Dice(CommandContext ctx, int? walls = null)
        {
            walls = walls ?? 4;
            if(walls < 2)
            {
                await ctx.RespondAsync("The minimum number of dice walls is 2!");
            }else
            {
                var result = new Random().Next(1, walls.Value);
                await ctx.RespondAsync($"You threw {result}!");
            }
        }
    }
}
