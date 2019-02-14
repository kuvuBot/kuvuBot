using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus;
using kuvuBot.Commands.Attributes;

namespace kuvuBot.Commands.Pictures
{
    public class SlapCommand : BaseCommandModule
    {
        [Command("slap"), LocalizedDescription("slap.description")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Slap(CommandContext ctx, [Description("User to slap")] DiscordUser target)
        {
            await ctx.Channel.TriggerTypingAsync();
            await RemUtils.SendRemEmbed(ctx, RemUtils.ImageType.Slap, target);
        }
    }
}
