using DSharpPlus.CommandsNext;
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
            await RemUtils.SendRemEmbed(ctx, RemUtils.ImageType.Hug, target);
        }
    }
}
