using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace kuvuBot.Commands.Information
{
    public class PingCommand : BaseCommandModule
    {
        [Command("ping"), Description("Shows bot ping")]
        public async Task Dice(CommandContext ctx)
        {
            await ctx.RespondAsync($"🏓 Ping: {ctx.Client.Ping}ms");
        }
    }
}
