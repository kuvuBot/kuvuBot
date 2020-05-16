using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using DSharpPlus;
using kuvuBot.Commands.Attributes;

namespace kuvuBot.Commands.Information
{

    public class InviteCommand : BaseCommandModule
    {
        [Command("invite"), LocalizedDescription("invite.description")]
        [RequireBotPermissions(Permissions.SendMessages)]
        public async Task Invite(CommandContext ctx)
        {
            var app = await ctx.Client.GetCurrentApplicationAsync();
            await ctx.RespondAsync(app.GenerateBotOAuth(DSharpPlus.Permissions.Administrator));
        }
    }

}
