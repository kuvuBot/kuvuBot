using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using kuvuBot.Commands.Attributes;

namespace kuvuBot.Commands.Information
{
    public class PingCommand : BaseCommandModule
    {
        [Command("ping"), LocalizedDescription("ping.description")]
        [RequireBotPermissions(Permissions.SendMessages)]
        public async Task Dice(CommandContext ctx)
        {
            await ctx.RespondAsync($"🏓 Ping: {ctx.Client.Ping}ms");
        }
    }
}
