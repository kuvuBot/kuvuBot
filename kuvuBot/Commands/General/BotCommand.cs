﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSNXT.DSharpPlus.ModernEmbedBuilder;

namespace kuvuBot.Commands.General
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
                    ("Number of servers", guilds.ToString(), inline: true),
                    ("Number of channels", channels.ToString(), inline: true),
                    ("Github", "[Check out](https://github.com/kuvuBot/kuvuBot)", inline: true),
                    ("Website", "[Check out](https://kuvuBot.xyz)", inline: true),
                },
                Color = new DuckColor(33, 150, 243),
                Timestamp = DuckTimestamp.Now,
                Footer = ($"Generated for {ctx.User.Username}#{ctx.User.Discriminator}", ctx.User.AvatarUrl),
                ThumbnailUrl = ctx.Guild.IconUrl,
            }.Send(ctx.Message.Channel);
        }
    }
}
