using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus;
using kuvuBot.Commands.Attributes;

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
