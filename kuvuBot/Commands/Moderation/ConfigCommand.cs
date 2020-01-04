using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using kuvuBot.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using DSharpPlus.Entities;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using System.Reflection;
using DSharpPlus.CommandsNext.Converters;
using kuvuBot.Lang;
using kuvuBot.Commands.Converters;

namespace kuvuBot.Commands.Moderation
{
    [Group("config")]
    [DSharpPlus.CommandsNext.Attributes.Description("Configuration commands")]
    [RequireUserPermissions(Permissions.ManageGuild), RequireBotPermissions(Permissions.SendMessages)]
    public class ConfigCommandGroup : BaseCommandModule
    {
        public enum OptionType { Field }
        public class Option
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Field { get; set; }
            public Type FieldType { get; set; } = typeof(string);
            public IArgumentConverter Converter { get; set; }
            public string[] Aliases { get; set; }
            public OptionType Type { get; set; }

            public Option(string name, string description, OptionType type = OptionType.Field, params string[] aliases)
            {
                Name = name;
                Description = description;
                Type = type;
                Aliases = aliases;
            }

            private PropertyInfo GetProperty(object instance)
            {
                var type = instance.GetType();
                return type.GetProperty(Field);
            }

            public object GetValue(object instance)
            {
                return GetProperty(instance).GetValue(instance);
            }

            public async Task SetValue(object instance, object value, bool useConverter = true, CommandContext ctx = null)
            {
                if (useConverter && Converter != null)
                {
                    var task = (dynamic)Converter.GetType().GetMethod("ConvertAsync").Invoke(Converter, new[] { value, ctx });
                    var optional = await task;

                    if (!optional.HasValue)
                    {
                        throw new Exception("Bad value type");
                    }

                    var convertToDatabaseFormat = Converter.GetType().GetMethod("ConvertToDatabaseFormat");
                    value = convertToDatabaseFormat != null ? convertToDatabaseFormat.Invoke(Converter, new []{ optional.Value }) : optional.Value;
                }

                if (value is string)
                {
                    if (value.ToString() != "null")
                    {
                        var converter = TypeDescriptor.GetConverter(GetProperty(instance).PropertyType);
                        value = converter.ConvertFrom(value);
                    }
                    else
                    {
                        value = null;
                    }
                }

                GetProperty(instance).SetValue(instance, value);
            }

            public static List<Option> Options = new List<Option>
            {
                new Option("prefix", "Bot's commands prefix") { Field = "Prefix"},
                new Option("language", "Bot language", aliases: "lang") { Field = "Lang", Converter = new LangConverter() },
                new Option("logchannel", "Channel where bot will log") { Field = "LogChannel", Converter = new FriendlyDiscordChannelConverter() },
                new Option("greetingchannel", "Channel where bot will say greeting to new users") { Field = "GreetingChannel", Converter = new FriendlyDiscordChannelConverter() },
                new Option("goodbyechannel", "Channel where bot will say goodbye to leaving users") { Field = "GoodbyeChannel", Converter = new FriendlyDiscordChannelConverter() },
                new Option("greeting", "Greeting message") { Field = "GreetingMessage" },
                new Option("goodbye", "Goodbye message") { Field = "GoodbyeMessage" },
                new Option("autorole", "Role that will be given to new users") { Field = "AutoRole", Converter = new FriendlyDiscordRoleConverter() },
                new Option("muterole", "Role that will be given to muted users") { Field = "MuteRole", Converter = new FriendlyDiscordRoleConverter() },
                new Option("showlevelup", "Option that enable level up messages") { Field = "ShowLevelUp", Converter = new FriendlyBoolConverter() },
            };
        }

        [Command("list"), DSharpPlus.CommandsNext.Attributes.Description("Lists config options"), GroupCommand]
        public async Task List(CommandContext ctx)
        {
            var kuvuGuild = await ctx.Guild.GetKuvuGuild();
            await new ModernEmbedBuilder
            {
                Title = "Config options",
                Fields =
                {
                    ("Available options", string.Join("\n", Option.Options.Select(x=>$"**{x.Name}**: `{x.Description}` = `{x.GetValue(kuvuGuild) ?? "not set"}`"))),
                    ("To set option: ", $"{kuvuGuild.Prefix}config (option name) (option value)", true),
                    ("To view option: ", $"{kuvuGuild.Prefix}config (option name)", true)
                }
            }.AddGeneratedForFooter(ctx).Send(ctx.Message.Channel);
        }

        [DSharpPlus.CommandsNext.Attributes.Description("Change config option"), GroupCommand]
        [RequireUserPermissions(Permissions.ManageGuild), RequireBotPermissions(Permissions.SendMessages)]
        public async Task Set(CommandContext ctx, string optionName, [RemainingText] string value = null)
        {
            var botContext = new BotContext();
            var kuvuGuild = await ctx.Guild.GetKuvuGuild(botContext);
            var option = Option.Options.FirstOrDefault(x => x.Name == optionName);
            if (option == null)
            {
                await List(ctx);
                return;
            }

            if (value == null)
            {
                await ctx.RespondAsync($"Current {option.Name} is `{option.GetValue(kuvuGuild)}`");
            }
            else
            {
                try
                {
                    await option.SetValue(kuvuGuild, value, true, ctx);
                    botContext.Guilds.Update(kuvuGuild);
                    await botContext.SaveChangesAsync();
                    await ctx.RespondAsync($"👌, changed {option.Name} to `{option.GetValue(kuvuGuild)}`");
                }
                catch
                {
                    await ctx.RespondAsync("Bad value type");
                }
            }
        }
    }
}
