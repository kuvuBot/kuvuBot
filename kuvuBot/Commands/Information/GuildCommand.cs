using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
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
                Title = await ctx.Lang("guild.title"),
                Fields =
                    {
                        (await ctx.Lang("guild.name"), ctx.Guild.Name, inline: true),
                        (await ctx.Lang("guild.owner"), ctx.Guild.Owner.DisplayName, inline: true),
                        (await ctx.Lang("guild.numOfMembers"), ctx.Guild.MemberCount.ToString(), inline: true),
                        (await ctx.Lang("guild.numOfRoles"), ctx.Guild.Roles.Count.ToString(), inline: true),
                        (await ctx.Lang("guild.region"), ctx.Guild.VoiceRegion.Name, inline: true),
                        (await ctx.Lang("guild.verification"), ctx.Guild.VerificationLevel.ToString(), inline: true),
                    },
                ThumbnailUrl = ctx.Guild.IconUrl,
            }.AddGeneratedForFooter(ctx).Send(ctx.Message.Channel);
        }
    }
}
