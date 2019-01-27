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
using kuvuBot.Data;

namespace kuvuBot.Commands.Moderation
{
    public class MuteCommand : BaseCommandModule
    {
        [Command("mute"), Description("Mutes user")]
        public async Task Mute(CommandContext ctx, [Description("User to mute")] DiscordMember target, [RemainingText] string reason = null)
        {
            if (!await ctx.HasPermission(Permissions.ManageMessages))
            {
                return;
            }

            var kuvuGuild = await ctx.Guild.GetKuvuGuild();
            if (kuvuGuild.MuteRole.HasValue)
            {
                await target.GrantRoleAsync(ctx.Guild.GetRole(kuvuGuild.MuteRole.Value), reason);
                await ctx.RespondAsync($"Successfuly muted {target.Mention}{(reason == null ? "" : $"with reason `{reason}`")}");
            }
            else
            {
                await ctx.RespondAsync($"There are not mute role. Type `{kuvuGuild.Prefix}config setup` to create it");
            }
        }
    }
}