﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SkiaSharp;
using kuvuBot.Data;

namespace kuvuBot.Commands.Fun
{
    public class RepCommand : BaseCommandModule
    {
        [Command("rep"), Description("Gives user reputation point")]
        public async Task Rep(CommandContext ctx, DiscordUser target)
        {            
            await ctx.Channel.TriggerTypingAsync();
            if(ctx.User == target)
            {
                await ctx.RespondAsync($"You can't give rep yourself");
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
                await ctx.RespondAsync($"🎉 | {target.Mention} now have {globalTarget.Reputation} rep!");
            }else
            {
                var nextRep = globalUser.LastGivedRep.Value.AddHours(24);
                var span = nextRep - DateTime.Now;
                await ctx.RespondAsync($"You already give rep recently. Please wait {span.Hours} hours, {span.Minutes} minutes");
            }
        }
    }
}