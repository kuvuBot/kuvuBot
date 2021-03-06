﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Linq;
using System.Reflection;
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
                Title = await ctx.Lang("bot.title"),
                Fields =
                {
                    (await ctx.Lang("bot.name"), ctx.Client.CurrentUser.Mention, inline: true),
                    (await ctx.Lang("bot.version"), Assembly.GetExecutingAssembly().GetName().Version.ToString(3), inline: true),
                    (await ctx.Lang("bot.numOfGuilds"), guilds.ToString(), inline: true),
                    (await ctx.Lang("bot.numOfChannels"), channels.ToString(), inline: true),
                    (await ctx.Lang("bot.git"), $"[{await ctx.Lang("bot.checkout")}](https://github.com/kuvuBot/kuvuBot)", inline: true),
                    (await ctx.Lang("bot.website"), $"[{await ctx.Lang("bot.checkout")}](https://kuvuBot.xyz)", inline: true),
                },
                ThumbnailUrl = ctx.Client.CurrentUser.AvatarUrl,
            }.AddGeneratedForFooter(ctx).Send(ctx.Message.Channel);
        }
    }
}
