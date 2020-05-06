using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using kuvuBot.Lang;
using DSharpPlus;
using kuvuBot.Commands.Attributes;
using kuvuBot.Core.Commands;

namespace kuvuBot.Commands.Moderation
{
    public class VoteCommand : BaseCommandModule
    {
        [Command("vote"), LocalizedDescription("vote.description")]
        [RequireBotPermissions(Permissions.SendMessages)]
        public async Task Vote(CommandContext ctx, [RemainingText, Description("Question")] string question)
        {
            question.RequireRemainingText();
            var message = await new ModernEmbedBuilder
            {
                Title = await ctx.Lang("vote.voting"),
                Description = question
            }.AddGeneratedForFooter(ctx).Send(ctx.Message.Channel);
            await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsup:"));
            await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsdown:"));
        }
    }
}