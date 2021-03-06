﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;
using kuvuBot.Commands.Attributes;

namespace kuvuBot.Commands.Fun
{
    public class RPSCommand : BaseCommandModule
    {
        [Aliases("kpn")]
        [Command("rps"), LocalizedDescription("rps.description")]
        [RequireBotPermissions(Permissions.SendMessages)]
        public async Task Rps(CommandContext ctx, string thing)
        {
            var things = new Dictionary<string, string>()
            {
                { "rock", "⚪ Rock" },
                { "paper", "📰 Paper" },
                { "scissors", "✂ Scissors" },
            };

            var results = new Dictionary<string, string>()
            {
                { "win", "🎉 You won!" },
                { "lost", "🥊 You lost!" },
                { "draw", "🏳 Draw!" },
            };

            thing = thing.Replace("rock", things["rock"], StringComparison.CurrentCultureIgnoreCase);
            thing = thing.Replace("paper", things["paper"], StringComparison.CurrentCultureIgnoreCase);
            thing = thing.Replace("scissors", things["scissors"], StringComparison.CurrentCultureIgnoreCase);

            if (things.Values.Contains(thing))
            {
                string result = null;
                var botthing = things.Values.ToArray()[new Random().Next(0, things.Count)];
                
                if(thing == botthing)
                {
                    result = results["draw"];
                }else
                // user beat bot
                if ((thing == things["rock"] && botthing == things["scissors"]) 
                    || (thing == things["paper"] && botthing == things["rock"])
                    || (thing == things["scissors"] && botthing == things["paper"]))
                {
                    result = results["win"];
                }else
                // bot beat user
                if ((botthing == things["rock"] && thing == things["scissors"])
                    || (botthing == things["paper"] && thing == things["rock"])
                    || (botthing == things["scissors"] && thing == things["paper"]))
                {
                    result = results["lost"];
                }

                await new ModernEmbedBuilder
                {
                    Title = "Rock, paper and scissors",
                    Fields =
                    {
                        ("You", thing, true),
                        ("Bot", botthing, true),
                        ("Result", result, true)
                    },
                }.AddGeneratedForFooter(ctx).Send(ctx.Message.Channel);
            }
            else
            {
                await ctx.RespondAsync("Stop cheater! You can use only `rock`/`paper`/`scissors`");
            }
        }
    }
}
