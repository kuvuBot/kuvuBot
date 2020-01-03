using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using kuvuBot.Commands.Attributes;
using kuvuBot.Lang;

namespace kuvuBot.Commands.Pictures
{
    public class AvatarCommand : BaseCommandModule
    {
        [Command("avatar"), LocalizedDescription("avatar.description")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Avatar(CommandContext ctx, [Description("User to get avatar")] DiscordUser target)
        {
            await ctx.Channel.TriggerTypingAsync();
            await new ModernEmbedBuilder()
            {
                Title = (await ctx.Lang("avatar.title")).Replace("{target}", target.Name()),
                ImageUrl = target.AvatarUrl ?? target.DefaultAvatarUrl,
                Color = Program.Config.EmbedColor,
            }.Send(ctx.Channel);
        }
    }
}
