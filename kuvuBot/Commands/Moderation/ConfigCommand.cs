using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using kuvuBot.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace kuvuBot.Commands.Moderation
{
    [Group("config")] // let's mark this class as a command group
    [Description("Configuration commands.")] // give it a description for help purposes
    public class ConfigCommand : BaseCommandModule
    {
        [GroupCommand]
        public async Task ExecuteGroupAsync(CommandContext ctx)
        {
            if(!await ctx.HasPermission(Permissions.ManageGuild))
            {
                return;
            }
            var kuvuGuild = await ctx.Guild.GetKuvuGuild();
            await ctx.RespondAsync($"Usage {kuvuGuild.Prefix}config prefix <prefix>");
        }

        [Command("prefix"), Description("Change bot prefix")]
        public async Task Prefix(CommandContext ctx, string prefix = null)
        {
            if (!await ctx.HasPermission(Permissions.ManageGuild))
            {
                return;
            }
            var botContext = new BotContext();
            var kuvuGuild = await ctx.Guild.GetKuvuGuild(botContext);
            if (prefix == null)
            {
                await ctx.RespondAsync($"Current prefix is `{kuvuGuild.Prefix}`");
            }
            else
            {
                kuvuGuild.Prefix = prefix;
                await ctx.RespondAsync($"👌, changed prefix to `{kuvuGuild.Prefix}`");
                botContext.Guilds.Update(kuvuGuild);
                await botContext.SaveChangesAsync();
            }
        }

        [Command("lang"), Description("Change bot language"), Aliases("language", "język", "jezyk")]
        public async Task Lang(CommandContext ctx, string lang = null)
        {
            if (!await ctx.HasPermission(Permissions.ManageGuild))
            {
                return;
            }
            var botContext = new BotContext();
            var kuvuGuild = await ctx.Guild.GetKuvuGuild(botContext);
            if (lang == null)
            {
                await ctx.RespondAsync($"Current language is `{kuvuGuild.Lang}`");
            }
            else
            {
                kuvuGuild.Lang = lang;
                await ctx.RespondAsync($"👌, changed language to `{kuvuGuild.Lang}`");
                botContext.Guilds.Update(kuvuGuild);
                await botContext.SaveChangesAsync();
            }
        }
    }
}
