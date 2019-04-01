﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using kuvuBot.Data;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using System.Reflection;
using System.Text.RegularExpressions;
using DSharpPlus.Entities;
using kuvuBot.Features.Modular;
using kuvuBot.Commands.Attributes;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace kuvuBot.Commands.Moderation
{
    [Group("global")]
    [Description("KuvuBot configuration commands")]
    [Hidden]
    public class GlobalCommandGroup : BaseCommandModule
    {
        [Command("modules"), Description("Print active modules")]
        [RequireGlobalRank(KuvuGlobalRank.Root)]
        public async Task Modules(CommandContext ctx)
        {
            var embed = new ModernEmbedBuilder
            {
                Title = "Modules",
                Color = Program.Config.EmbedColor,
                Timestamp = DuckTimestamp.Now,
                Footer = ($"Generated for {ctx.User.Username}#{ctx.User.Discriminator}", ctx.User.AvatarUrl),
                Fields = new List<DuckField>
                {
                    ("kuvuBot", $"Version: {Assembly.GetExecutingAssembly().GetName().Version.ToString(3)}")
                }
            };

            foreach (var module in ModuleManager.Modules)
            {
                embed.AddField(module.Name, $"Version: {module.Version}, Author: {module.Author}, Description: `{module.Description}`");
            }

            await embed.Send(ctx.Message.Channel);
        }

        public static class Globals
        {
            public class EvalContext
            {
                public CommandContext Ctx;
            }
        }

        [Command("eval"), Description("Evaluate c# code")]
        [RequireGlobalRank(KuvuGlobalRank.Root)]
        public async Task Eval(CommandContext ctx, [RemainingText] string code)
        {
            try
            {
                var state = await CSharpScript.RunAsync(Regex.Replace(code, "```(csharp)?", "").Trim(),
                    ScriptOptions.Default.WithReferences(typeof(int).Assembly, typeof(CommandContext).Assembly, typeof(Globals.EvalContext).Assembly)
                        .WithImports("DSharpPlus", "DSharpPlus.Entities", "DSharpPlus.CommandsNext"),
                    new Globals.EvalContext {Ctx=ctx}, typeof(Globals.EvalContext));
                if (state.Exception == null)
                {
                    var embed = new ModernEmbedBuilder
                    {
                        Title = "Evaluation success",
                        Color = DiscordColor.Green,
                        Timestamp = DuckTimestamp.Now,
                        Footer = ($"Generated for {ctx.User.Username}#{ctx.User.Discriminator}", ctx.User.AvatarUrl),
                        Fields = new List<DuckField>
                        {
                            ("Result", state.ReturnValue?.ToString() ?? "*null*")
                        }
                    };

                    if (state.Variables.Any())
                        embed.AddField("Variables", string.Join("\n", state.Variables.Select(variable => $"*{variable.Type}* **{variable.Name}** = `{variable.Value}`")));

                    await embed.Send(ctx.Message.Channel);
                }
                else
                {
                    throw state.Exception;
                }
            }
            catch (Exception e)
            {
                await new ModernEmbedBuilder
                {
                    Title = "Evaluation failed",
                    Color = DiscordColor.Red,
                    Timestamp = DuckTimestamp.Now,
                    Footer = ($"Generated for {ctx.User.Username}#{ctx.User.Discriminator}", ctx.User.AvatarUrl),
                    Description = e.ToString().Replace("`", @"\`").Replace("*", @"\*").Replace("~", @"\~").Replace("_", @"\_")
                }.Send(ctx.Message.Channel);
            }
        }
    }
}