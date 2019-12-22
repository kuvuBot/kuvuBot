using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus.CommandsNext;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using kuvuBot.Lang;

namespace kuvuBot.Commands
{
    public static class EmbedUtils
    {
        public static DuckColor Red = new DuckColor(231, 76, 60);

        public static ModernEmbedBuilder AddGeneratedForFooter(this ModernEmbedBuilder embed, CommandContext ctx, bool defaultColor = true)
        {
            if(defaultColor)
                embed.Color = Program.Config.EmbedColor;
            embed.Timestamp = DuckTimestamp.Now;
            embed.FooterText = ctx.Lang("global.footer").GetAwaiter().GetResult().Replace("{user}", ctx.User.Name());
            embed.FooterIcon = ctx.User.AvatarUrl;
            return embed;
        }
    }
}
