using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using kuvuBot.Lang;
using System.Linq;
using HSNXT.DSharpPlus.ModernEmbedBuilder;

namespace kuvuBot.Commands
{
    public static class CommandUtils
    {
        public async static Task<bool> HasPermission(this CommandContext ctx, Permissions permission, bool respond = true)
        {
            if (ctx.Member.PermissionsIn(ctx.Channel).HasPermission(permission))
            {
                return true;
            }
            if (respond)
                await ctx.RespondAsync(await ctx.Lang("global.nopermission"));
            return false;
        }

        public static string Category(this Command command)
        {
            return command.Module.ModuleType.Namespace.Split('.').Last();
        }

        public static string Name(this DiscordUser user, bool displayBot = false)
        {
            var name = $"{user.Username}#{user.Discriminator}";
            if (displayBot && user.IsBot)
            {
                return "[BOT] " + name;
            }
            return name;
        }


        public static async Task SendAutoRemoveMessageAsync(this DiscordChannel channel, TimeSpan delay, string content = null, bool tts = false, DiscordEmbed embed = null)
        {
            var message = await channel.SendMessageAsync(content, tts, embed);
            await Task.Delay(delay).ContinueWith(async t => await message.DeleteAsync());
        }
    }
}
