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
using kuvuBot.Commands.Attributes;
using kuvuBot.Data;

namespace kuvuBot.Commands.Moderation
{
    [Group("mute")]
    public class MuteCommand : BaseCommandModule
    {
        [GroupCommand, LocalizedDescription("mute.description")]
        [RequireUserPermissions(Permissions.ManageMessages), RequireBotPermissions(Permissions.SendMessages | Permissions.ManageRoles)]
        public async Task Mute(CommandContext ctx, [Description("User to mute")] DiscordMember target, [RemainingText] string reason = null)
        {
            reason.RequireRemainingText();
            var kuvuGuild = await ctx.Guild.GetKuvuGuild();
            if (kuvuGuild.MuteRole.HasValue)
            {
                await target.GrantRoleAsync(ctx.Guild.GetRole(kuvuGuild.MuteRole.Value), reason);
                await ctx.RespondAsync($"Successfuly muted {target.Mention}{(reason == null ? "" : $"with reason `{reason}`")}");
            }
            else
            {
                await ctx.RespondAsync($"There are not mute role. Type `{kuvuGuild.Prefix}mute setup` to create it");
            }
        }

        [Command("setup"), Description("Setup mute role")]
        [RequireUserPermissions(Permissions.ManageGuild | Permissions.ManageRoles), RequireBotPermissions(Permissions.SendMessages)]
        public async Task Setup(CommandContext ctx, DiscordRole role = null)
        {
            var botContext = new BotContext();
            var kuvuGuild = await ctx.Guild.GetKuvuGuild(botContext);
            if (role == null)
            {
                if (kuvuGuild.MuteRole.HasValue)
                {
                    await ctx.RespondAsync((await ctx.Lang("mute.muteRole.current")).Replace("{role}", kuvuGuild.MuteRole.ToString()));
                }
                else
                {
                    await ctx.RespondAsync(await ctx.Lang("mute.muteRole.creating"));
                    await ctx.Channel.TriggerTypingAsync();
                    try
                    {
                        role = await ctx.Guild.CreateRoleAsync("Muted", Permissions.None.Revoke(Permissions.SendMessages));
                        await ctx.RespondAsync((await ctx.Lang("mute.muteRole.created")).Replace("{role}", role.Mention));
                    }
                    catch (Exception)
                    {
                        await ctx.RespondAsync(await ctx.Lang("mute.muteRole.error"));
                    }
                }

            }
            if (role != null)
            {
                kuvuGuild.MuteRole = role.Id;
                await ctx.RespondAsync((await ctx.Lang("mute.muteRole.changed")).Replace("{role}", kuvuGuild.MuteRole.ToString()));
                botContext.Guilds.Update(kuvuGuild);
                await botContext.SaveChangesAsync();
            }
        }
    }
}