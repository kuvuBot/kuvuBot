﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus;
using kuvuBot.Commands.Attributes;
using kuvuBot.Lang;

namespace kuvuBot.Commands.Pictures
{
    public class HugCommand : BaseCommandModule
    {
        [Command("hug"), LocalizedDescription("hug.description")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Hug(CommandContext ctx, [Description("User to hug")] DiscordUser target)
        {
            await ctx.Channel.TriggerTypingAsync();
            using (WebClient wc = new WebClient())
            {
                var response = JsonConvert.DeserializeObject<RemResponse>(wc.DownloadString("https://rra.ram.moe/i/r?type=hug"));
                var embed = new ModernEmbedBuilder
                {
                    Title = (await ctx.Lang("hug.title")).Replace("{user}", ctx.User.Username).Replace("{target}", target.Username),
                    Color = Program.Config.EmbedColor,
                    Timestamp = DuckTimestamp.Now,
                    ImageUrl = "https://rra.ram.moe" + response.Path,
                    Footer = ((await ctx.Lang("global.footer")).Replace("{user}", ctx.User.Name()), ctx.User.AvatarUrl),
                };
                await embed.Send(ctx.Channel);
            }
        }
    }
}
