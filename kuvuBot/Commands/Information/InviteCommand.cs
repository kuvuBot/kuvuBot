using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
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
