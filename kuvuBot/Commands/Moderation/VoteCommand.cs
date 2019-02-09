using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using kuvuBot.Lang;
using DSharpPlus;

namespace kuvuBot.Commands.Moderation
{
    public class VoteCommand : BaseCommandModule
    {
        [Aliases("g³osuj", "glosuj")]
        [Command("vote"), Description("Makes voting")]
        [ RequireBotPermissions(Permissions.SendMessages)]
        public async Task Vote(CommandContext ctx, [RemainingText,Description("Question")] string question)
        {            
            var message = await new ModernEmbedBuilder
            {
                Title = await ctx.Lang("vote.voting"),
                Description = question,
                Color = Program.Config.EmbedColor,
                Timestamp = DuckTimestamp.Now,
                Footer = ($"Generated for {ctx.User.Username}#{ctx.User.Discriminator}", ctx.User.AvatarUrl),
            }.Send(ctx.Message.Channel);
            await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsup:"));
            await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsdown:"));
        }
    }
}