using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using kuvuBot.Data;
using MoreLinq;

namespace kuvuBot.Commands.General
{
    public class HelpFormatter : BaseHelpFormatter
    {
        public ModernEmbedBuilder EmbedBuilder { get; }
        private Command Command { get; set; }
        private KuvuGuild kuvuGuild { get; set; }

        public HelpFormatter(CommandContext ctx) : base(ctx)
        {
            var kuvuGuildTask = ctx.Guild.GetKuvuGuild(); kuvuGuildTask.Wait();
            kuvuGuild = kuvuGuildTask.Result;
            EmbedBuilder = new ModernEmbedBuilder()
            {
                Title = "Command list",
                Color = new DuckColor(33, 150, 243),
                Timestamp = DuckTimestamp.Now,
                Footer = ($"Generated for {ctx.User.Username}#{ctx.User.Discriminator}", ctx.User.AvatarUrl),
                //Footer = $"kuvuBot - {Assembly.GetExecutingAssembly().GetName().Version.ToString(3)}",
            };
        }

        public override BaseHelpFormatter WithCommand(Command command)
        {
            Command = command;

            EmbedBuilder.Description = $"{Formatter.InlineCode(command.Name)}: {command.Description ?? "No description provided."}";

            if (command is CommandGroup cgroup && cgroup.IsExecutableWithoutSubcommands)
                EmbedBuilder.Description = $"{EmbedBuilder.Description}\n\nThis group can be executed as a standalone command.";

            if (command.Aliases?.Any() == true)
                EmbedBuilder.AddField("Aliases", string.Join(", ", command.Aliases.Select(Formatter.InlineCode)), false);

            if (command.Overloads?.Any() == true)
            {
                var sb = new StringBuilder();

                foreach (var ovl in command.Overloads.OrderByDescending(x => x.Priority))
                {
                    sb.Append('`').Append(command.QualifiedName);

                    foreach (var arg in ovl.Arguments)
                        sb.Append(arg.IsOptional || arg.IsCatchAll ? " [" : " <").Append(arg.Name).Append(arg.IsCatchAll ? "..." : "").Append(arg.IsOptional || arg.IsCatchAll ? ']' : '>');

                    sb.Append("`\n");

                    foreach (var arg in ovl.Arguments)
                        sb.Append('`').Append(arg.Name).Append(" (").Append(CommandsNext.GetUserFriendlyTypeName(arg.Type)).Append(")`: ").Append(arg.Description ?? "No description provided.").Append('\n');

                    sb.Append('\n');
                }

                EmbedBuilder.AddField("Arguments", sb.ToString().Trim(), false);
            }

            return this;
        }

        public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
        {
            if (Command == null)
            {
                EmbedBuilder.AddField("Prefix", kuvuGuild.Prefix, true);
                EmbedBuilder.AddField("Language", kuvuGuild.Lang, true);
            }
            var categories = subcommands.Where(x=>x.Name != "help").Select(c => c.Category()).DistinctBy(x=>x);
            foreach(var category in categories)
            {
                EmbedBuilder.AddField(Command != null ? "Subcommands" : category, string.Join(", ", subcommands.Where(c=>c.Category() == category).Select(x => $"`{x.Name}`")));
            }
            //EmbedBuilder.AddField(Command != null ? "Subcommands" : "Commands", string.Join(", ", subcommands.Select(x => Formatter.InlineCode(x.Name))), false);
            EmbedBuilder.AddField("For more information type", $"{kuvuGuild.Prefix}help <command>");

            return this;
        }

        public override CommandHelpMessage Build()
        {
            return new CommandHelpMessage(embed: EmbedBuilder.Build());
        }
    }
}
