using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using DSharpPlus;
using kuvuBot.Commands.Attributes;
using kuvuBot.Lang;

namespace kuvuBot.Commands.Information
{
    public class GuildCommand : BaseCommandModule
    {
        [Aliases("serwer", "server", "gildia")]
        [Command("guild"), LocalizedDescription("guild.description")]
        [RequireBotPermissions(Permissions.SendMessages)]
        public async Task Guild(CommandContext ctx)
        {
            await new ModernEmbedBuilder
            {
                Title = ctx.Lang("guild.title").Result,
                Fields =
                    {
                        (ctx.Lang("guild.name").Result, ctx.Guild.Name, inline: true),
                        (ctx.Lang("guild.owner").Result, ctx.Guild.Owner.DisplayName, inline: true),
                        (ctx.Lang("guild.numOfMembers").Result, ctx.Guild.MemberCount.ToString(), inline: true),
                        (ctx.Lang("guild.numOfRoles").Result, ctx.Guild.Roles.Count.ToString(), inline: true),
                        (ctx.Lang("guild.region").Result, ctx.Guild.VoiceRegion.Name, inline: true),
                        (ctx.Lang("guild.verification").Result, ctx.Guild.VerificationLevel.ToString(), inline: true),
                    },
                Color = Program.Config.EmbedColor,
                Timestamp = DuckTimestamp.Now,
                Footer = (ctx.Lang("global.footer").Result.Replace("{user}", ctx.User.Name()), ctx.User.AvatarUrl),
                ThumbnailUrl = ctx.Guild.IconUrl,
            }.Send(ctx.Message.Channel);
        }
    }
}
