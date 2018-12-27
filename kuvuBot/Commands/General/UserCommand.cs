using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using System.Globalization;

namespace kuvuBot.Commands.General
{
    public class UserCommand : BaseCommandModule
    {
        [Command("user"), Description("User informations")]
        public async Task User(CommandContext ctx, DiscordUser target = null)
        {
            target = target ?? ctx.User;
            await new ModernEmbedBuilder
            {
                Title = "User informations",
                Fields =
                    {
                        ("Discord tag", target.Name(true), inline: true),
                        ("Id", target.Id.ToString(), inline: true),
                        ("Registration date", target.CreationTimestamp.DateTime.ToString("g", CultureInfo.CreateSpecificCulture("pl-PL")), inline: true),
                },
                Color = new DuckColor(33, 150, 243),
                Timestamp = DuckTimestamp.Now,
                Footer = ($"Generated for {ctx.User.Name()}", ctx.User.AvatarUrl),
                ThumbnailUrl = target.AvatarUrl ?? target.DefaultAvatarUrl,
            }.Send(ctx.Message.Channel);
        }
    }
}
