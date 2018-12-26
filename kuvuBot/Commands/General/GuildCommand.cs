using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HSNXT.DSharpPlus.ModernEmbedBuilder;

namespace kuvuBot.Commands.General
{
    public class GuildCommand : BaseCommandModule
    {
        [Aliases("serwer", "server", "gildia")]
        [Command("guild"), Description("Guild informations")]
        public async Task Guild(CommandContext ctx)
        {
            await new ModernEmbedBuilder
            {
                Title = "Guild informations",
                Fields =
                    {
                        ("Name", ctx.Guild.Name, inline: true),
                        ("Owner", ctx.Guild.Owner.DisplayName, inline: true),
                        ("Number of members", ctx.Guild.MemberCount.ToString(), inline: true),
                        ("Number of roles", ctx.Guild.Roles.Count.ToString(), inline: true),
                        ("Region", ctx.Guild.VoiceRegion.Name, inline: true),
                        ("Verification level", ctx.Guild.VerificationLevel.ToString(), inline: true),
                    },
                Color = new DuckColor(33, 150, 243),
                Timestamp = DuckTimestamp.Now,
                Footer = ($"Zrobione dla: {ctx.User.Username}#{ctx.User.Discriminator}", ctx.User.AvatarUrl),
                ThumbnailUrl = ctx.Guild.IconUrl,
            }.Send(ctx.Message.Channel);
        }
    }
}
