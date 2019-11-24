using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using DSharpPlus;
using kuvuBot.Commands.Attributes;
using kuvuBot.Lang;

namespace kuvuBot.Commands.Information
{
    public class BotCommand : BaseCommandModule
    {
        [Command("bot"), LocalizedDescription("bot.description")]
        [RequireBotPermissions(Permissions.SendMessages)]
        public async Task Bot(CommandContext ctx)
        {
            int guilds = ctx.Client.Guilds.Count;
            int channels = ctx.Client.Guilds.Values.SelectMany(g => g.Channels).Count();
            await new ModernEmbedBuilder
            {
                Title = ctx.Lang("bot.title").Result,
                Fields =
                {
                    (ctx.Lang("bot.name").Result, ctx.Client.CurrentUser.Mention, inline: true),
                    (ctx.Lang("bot.version").Result, Assembly.GetExecutingAssembly().GetName().Version.ToString(3), inline: true),
                    (ctx.Lang("bot.numOfGuilds").Result, guilds.ToString(), inline: true),
                    (ctx.Lang("bot.numOfChannels").Result, channels.ToString(), inline: true),
                    (ctx.Lang("bot.git").Result, "[Check out](https://github.com/kuvuBot/kuvuBot)", inline: true),
                    (ctx.Lang("bot.website").Result, "[Check out](https://kuvuBot.xyz)", inline: true),
                },
                Color = Program.Config.EmbedColor,
                Timestamp = DuckTimestamp.Now,
                Footer = (ctx.Lang("global.footer").Result.Replace("{user}", ctx.User.Name()), ctx.User.AvatarUrl),
                ThumbnailUrl = ctx.Client.CurrentUser.AvatarUrl,
            }.Send(ctx.Message.Channel);
        }
    }
}
