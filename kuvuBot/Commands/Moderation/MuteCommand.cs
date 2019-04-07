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
                    await ctx.RespondAsync($"Current mute role is `{kuvuGuild.MuteRole.ToString()}`");
                }
                else
                {
                    await ctx.RespondAsync($"Creating mute role...");
                    await ctx.Channel.TriggerTypingAsync();
                    try
                    {
                        role = await ctx.Guild.CreateRoleAsync("Muted", Permissions.None.Revoke(Permissions.SendMessages));
                        await ctx.RespondAsync($"Created {role.Mention}. Roles above will bypass mute");
                    }
                    catch (Exception)
                    {
                        await ctx.RespondAsync($"Can't create role.");
                    }
                }

            }
            if (role != null)
            {
                kuvuGuild.MuteRole = role.Id;
                await ctx.RespondAsync($"👌, changed mute role to `{kuvuGuild.MuteRole.ToString()}`");
                botContext.Guilds.Update(kuvuGuild);
                await botContext.SaveChangesAsync();
            }
        }
    }
}