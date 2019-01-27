﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace kuvuBot.Commands.Pictures
{
    public class SlapCommand : BaseCommandModule
    {
        [Command("slap"), Description("Slaps the marked user")]
        public async Task Slap(CommandContext ctx, [Description("User to slap")] DiscordUser target)
        {
            await ctx.Channel.TriggerTypingAsync();
            await RemUtils.SendRemEmbed(ctx, RemUtils.ImageType.Slap, target);
        }
    }
}
