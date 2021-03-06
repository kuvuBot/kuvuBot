﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using kuvuBot.Commands.Attributes;
using kuvuBot.Core.Commands;
using kuvuBot.Data;
using kuvuBot.Lang;
using MoreLinq;

namespace kuvuBot.Commands.General
{
    public class HelpFormatter : BaseHelpFormatter
    {
        public ModernEmbedBuilder EmbedBuilder { get; }
        private CommandContext Ctx { get; set; }
        private Command Command { get; set; }
        private KuvuGuild KuvuGuild { get; set; }

        public HelpFormatter(CommandContext ctx) : base(ctx)
        {
            Ctx = ctx;
            KuvuGuild = ctx.Guild.GetKuvuGuild().GetAwaiter().GetResult();
            EmbedBuilder = new ModernEmbedBuilder()
            {
                Title = "Command list"
            }.AddGeneratedForFooter(ctx);
        }

        public override BaseHelpFormatter WithCommand(Command command)
        {
            Command = command;

            EmbedBuilder.Description = $"{Formatter.InlineCode(command.Name)}: {command.LocalizedDescription(KuvuGuild) ?? "No description provided."}";

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
                        sb.Append('`').Append(arg.Name).Append(" (").Append(CommandsNext.GetUserFriendlyTypeName(arg.Type)).Append(")`: ").Append(arg.LocalizedDescription(KuvuGuild) ?? "No description provided.").Append('\n');

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
                EmbedBuilder.AddField("Prefix", KuvuGuild.Prefix, true);
                EmbedBuilder.AddField("Language", $"{KuvuGuild.Lang.ToUpper()} {DiscordEmoji.FromUnicode(Ctx.Client, Ctx.Lang("lang.flag").Result)}", true);
                if (Program.Config.CustomBot)
                {
                    EmbedBuilder.AddField("powered by", $"kuvuBot ({Assembly.GetExecutingAssembly().GetName().Version.ToString(3)})", true);
                }
                else
                {
                    EmbedBuilder.AddField("kuvuBot", $"{Assembly.GetExecutingAssembly().GetName().Version.ToString(3)}", true);
                }
            }
            var categories = subcommands.Where(x => x.Name != "help" && x.IsHidden == false).Select(c => c.Category()).DistinctBy(x => x);
            foreach (var category in categories)
            {
                EmbedBuilder.AddField(Command != null ? "Subcommands" : category, string.Join(", ", subcommands.Where(c => c.Category() == category).Select(x => $"`{x.Name}`")));
            }
            if (Command == null)
            {
                EmbedBuilder.AddField("For more information type", $"{KuvuGuild.Prefix}help <command>");
                if (!Program.Config.CustomBot)
                    EmbedBuilder.AddField("Support server", "[Join our support server](https://discord.gg/KbUdeKe)", true);
                EmbedBuilder.AddField("Add kuvuBot", "[Add kuvuBot to your guild](https://discordapp.com/oauth2/authorize?&client_id=205965155282976768&scope=bot&permissions=8)", true);
            }
            else
            {
                EmbedBuilder.AddField("For more information type", $"{KuvuGuild.Prefix}help {Command.Name} <subcommand>");
            }

            return this;
        }

        public override CommandHelpMessage Build()
        {
            return new CommandHelpMessage(embed: EmbedBuilder.Build());
        }
    }
}
