using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus;
using kuvuBot.Commands.Attributes;

namespace kuvuBot.Commands.Pictures
{
    public class PatCommand : BaseCommandModule
    {
        [Command("pat"), LocalizedDescription("pat.description")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Pat(CommandContext ctx, [Description("User to pat")] DiscordUser target)
        {
            await ctx.Channel.TriggerTypingAsync();
            await RemUtils.SendRemEmbed(ctx, RemUtils.ImageType.Pat, target);
        }
    }
}
