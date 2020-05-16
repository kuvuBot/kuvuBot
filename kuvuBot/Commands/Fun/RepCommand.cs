using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;
using kuvuBot.Data;
using DSharpPlus;
using kuvuBot.Commands.Attributes;
using kuvuBot.Lang;

namespace kuvuBot.Commands.Fun
{
    public class RepCommand : BaseCommandModule
    {
        [Command("rep"), LocalizedDescription("rep.description")]
        [RequireBotPermissions(Permissions.SendMessages)]
        public async Task Rep(CommandContext ctx, DiscordUser target)
        {
            await ctx.Channel.TriggerTypingAsync();
            if (ctx.User == target)
            {
                await ctx.RespondAsync(await ctx.Lang("rep.self"));
                return;
            }

            var botContext = new BotContext();
            var globalUser = await ctx.User.GetGlobalUser(botContext);
            var globalTarget = await target.GetGlobalUser(botContext);

            if (!globalUser.LastGivedRep.HasValue || (DateTime.Now - globalUser.LastGivedRep.Value).Days >= 1)
            {
                globalTarget.Reputation += 1;
                globalUser.LastGivedRep = DateTime.Now;
                await botContext.SaveChangesAsync();
                await ctx.RespondAsync((await ctx.Lang("rep.success"))
                    .Replace("{user}", target.Mention)
                    .Replace("{points}", globalTarget.Reputation.ToString()));
            }
            else
            {
                var nextRep = globalUser.LastGivedRep.Value.AddHours(24);
                var span = nextRep - DateTime.Now;
                await ctx.RespondAsync((await ctx.Lang("rep.limit"))
                    .Replace("{hours}", span.Hours.ToString())
                    .Replace("{minutes}", span.Minutes.ToString()));
            }
        }
    }
}