using System;
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
using MoreLinq.Extensions;

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
                Fields = new List<DuckField>
                {
                    ("kuvuBot", $"Version: {Assembly.GetExecutingAssembly().GetName().Version.ToString(3)}")
                }
            }.AddGeneratedForFooter(ctx);

            foreach (var module in ModuleManager.Modules)
            {
                embed.AddField(module.Name, $"Version: {module.DisplayVersion}, Author: {module.Author}, Description: `{module.Description}`");
            }

            await embed.Send(ctx.Message.Channel);
        }

        [Command("shards"), Description("Print shards")]
        [RequireGlobalRank(KuvuGlobalRank.Helper)]
        public async Task Shards(CommandContext ctx)
        {
            var embed = new ModernEmbedBuilder
            {
                Title = "Shards",
                Fields = new List<DuckField>
                {
                    ("ShardId", ctx.Client.ShardId.ToString(), true),
                    ("ShardCount", ctx.Client.ShardCount.ToString(), true),
                    ("Shards", $"[{Program.Client.ShardClients.Values.Select(x=>x.Guilds.Count.ToString()).ToDelimitedString(", ")}]"),
                }
            }.AddGeneratedForFooter(ctx);

            await embed.Send(ctx.Message.Channel);
        }

        public static class Globals
        {
            public class EvalContext
            {
                public CommandContext ctx;

                public EvalContext(CommandContext ctx)
                {
                    this.ctx = ctx;
                }
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
                    new Globals.EvalContext(ctx), typeof(Globals.EvalContext));
                if (state.Exception == null)
                {
                    var embed = new ModernEmbedBuilder
                    {
                        Title = "Evaluation success",
                        Fields = new List<DuckField>
                        {
                            ("Result", state.ReturnValue?.ToString() ?? "*null*")
                        }
                    }.AddGeneratedForFooter(ctx);

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
                    Description = e.ToString().Replace("`", @"\`").Replace("*", @"\*").Replace("~", @"\~").Replace("_", @"\_")
                }.AddGeneratedForFooter(ctx).Send(ctx.Message.Channel);
            }
        }
    }
}