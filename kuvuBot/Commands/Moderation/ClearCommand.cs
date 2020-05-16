using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Linq;
using System.Threading.Tasks;
using kuvuBot.Lang;
using DSharpPlus;
using kuvuBot.Commands.Attributes;
using kuvuBot.Core.Commands;
using kuvuBot.Data;

namespace kuvuBot.Commands.Moderation
{
    public class ClearCommand : BaseCommandModule
    {
        [Command("clear"), LocalizedDescription("clear.description")]
        [RequireUserPermissions(Permissions.ManageMessages), RequireBotPermissions(Permissions.SendMessages | Permissions.ManageMessages)]
        public async Task Clear(CommandContext ctx, [Description("Messages count")] int messageCount)
        {
            await ctx.Channel.TriggerTypingAsync();
            var reason = $"cleared by {ctx.Member.Id}";
            var deletedMessages = 0;
            await ctx.Message.DeleteAsync();
            var messages = await ctx.Channel.GetMessagesAsync(messageCount);
            for (var i = 0; i < messageCount / 100 + 1; i++)
            {
                var messagesToDelete = messages.Skip(100 * i).Take(100).Where(x => (DateTime.Now - x.Timestamp).TotalDays < 14).ToList();

                switch (messagesToDelete.Count)
                {
                    case 0:
                        continue;
                    case 1:
                        await messagesToDelete[0].DeleteAsync();
                        break;
                    default:
                        await ctx.Channel.DeleteMessagesAsync(messagesToDelete, reason);
                        break;
                }
                deletedMessages += messagesToDelete.Count();
            }

            var globalUser = await ctx.Member.GetGlobalUser();
            if (globalUser.GlobalRank >= KuvuGlobalRank.Admin)
            {
                foreach (var message in messages.Where(x => (DateTime.Now - x.Timestamp).TotalDays > 14).ToList())
                {
                    deletedMessages++;
                    await message.DeleteAsync(reason).ConfigureAwait(false);
                }
            }
            else if (messageCount - deletedMessages > 0)
            {
                _ = ctx.Channel.SendAutoRemoveMessageAsync(TimeSpan.FromSeconds(1.5), (await ctx.Lang("clear.failed")).Replace("{count}", (messageCount - deletedMessages).ToString())).ConfigureAwait(false);
            }

            await ctx.Channel.SendAutoRemoveMessageAsync(TimeSpan.FromSeconds(1.5), (await ctx.Lang("clear.success")).Replace("{count}", deletedMessages.ToString()));
        }
    }
}