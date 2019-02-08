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

namespace kuvuBot.Commands.Information
{
    public class BotCommand : BaseCommandModule
    {
        [Command("bot"), Description("Bot informations")]
        public async Task Bot(CommandContext ctx)
        {
            int guilds = ctx.Client.Guilds.Count;
            int channels = ctx.Client.Guilds.Values.SelectMany(g => g.Channels).Count();
            await new ModernEmbedBuilder
            {
                Title = "Bot info",
                Fields =
                {
                    ("Name", ctx.Client.CurrentUser.Mention, inline: true),
                    ("Version", Assembly.GetExecutingAssembly().GetName().Version.ToString(3), inline: true),
                    ("Number of guilds", guilds.ToString(), inline: true),
                    ("Number of channels", channels.ToString(), inline: true),
                    ("Github", "[Check out](https://github.com/kuvuBot/kuvuBot)", inline: true),
                    ("Website", "[Check out](https://kuvuBot.xyz)", inline: true),
                },
                Color = Program.Config.EmbedColor,
                Timestamp = DuckTimestamp.Now,
                Footer = ($"Generated for {ctx.User.Username}#{ctx.User.Discriminator}", ctx.User.AvatarUrl),
                ThumbnailUrl = ctx.Client.CurrentUser.AvatarUrl,
            }.Send(ctx.Message.Channel);
        }
    }
}
