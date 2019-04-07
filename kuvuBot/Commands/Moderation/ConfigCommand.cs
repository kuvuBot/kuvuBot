using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using kuvuBot.Data;
using System;
using System.Collections.Generic;
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
    [Description("Configuration commands")]
    [RequireUserPermissions(Permissions.ManageGuild), RequireBotPermissions(Permissions.SendMessages)]
    public class ConfigCommandGroup : BaseCommandModule
    {
        private enum OptionType { Field }
        private class Option
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Field { get; set; }
            public Type FieldType { get; set; }
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

            public void SetValue(object instance, object value)
            {
                GetProperty(instance).SetValue(instance, value);
            }

            public static List<Option> Options = new List<Option>
            {
                new Option("prefix", "Bot's commands prefix") { Field = "Prefix" },
                new Option("language", "Bot language", aliases: "lang") { Field = "Lang", Converter = new LangConverter() },
                new Option("logchannel", "Channel where bot will log") { Field = "LogChannel", Converter = new FriendlyDiscordChannelConverter() },
                new Option("greetingchannel", "Channel where bot will say greeting to new users") { Field = "GreetingChannel", Converter = new FriendlyDiscordChannelConverter() },
                new Option("goodbyechannel", "Channel where bot will say goodbye to leaving users") { Field = "GoodbyeChannel", Converter = new FriendlyDiscordChannelConverter() },
                new Option("greeting", "Greeting message") { Field = "GreetingMessage" },
                new Option("goodbye", "Goodbye message") { Field = "GoodbyeMessage" },
                new Option("autorole", "Role that will be given to new users") { Field = "AutoRole", Converter = new DiscordRoleConverter() },
                new Option("muterole", "Role that will be given to muted users") { Field = "MuteRole", Converter = new DiscordRoleConverter() },
                new Option("showlevelup", "Option that enable level up messages") { Field = "ShowLevelUp", Converter = new FriendlyBoolConverter() },
            };
        }

        [Command("list"), Description("Lists config options"), GroupCommand]
        public async Task List(CommandContext ctx)
        {
            var kuvuGuild = await ctx.Guild.GetKuvuGuild();
            await new ModernEmbedBuilder
            {
                Title = "Config options",
                Color = Program.Config.EmbedColor,
                Timestamp = DuckTimestamp.Now,
                Footer = ($"Generated for {ctx.User.Username}#{ctx.User.Discriminator}", ctx.User.AvatarUrl),
                Fields =
                {
                    ("Available options", string.Join("\n", Option.Options.Select(x=>$"**{x.Name}**: `{x.Description}` = `{x.GetValue(kuvuGuild) ?? "not set"}`"))),
                    ("To set option: ", $"{kuvuGuild.Prefix}config (option name) (option value)", true),
                    ("To view option: ", $"{kuvuGuild.Prefix}config (option name)", true)
                }
            }.Send(ctx.Message.Channel);
        }

        [Description("Change config option"), GroupCommand]
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
                object converted = value;

                if (option.Converter != null)
                {
                    var task = (dynamic)option.Converter.GetType().GetMethod("ConvertAsync").Invoke(option.Converter, new object[] { value, ctx });
                    dynamic optional = await task;
                    if (!optional.HasValue)
                    {
                        await ctx.RespondAsync("Bad value type");
                        return;
                    }
                    converted = optional.Value;
                }

                option.SetValue(kuvuGuild, converted);
                botContext.Guilds.Update(kuvuGuild);
                await botContext.SaveChangesAsync();
                await ctx.RespondAsync($"👌, changed {option.Name} to `{option.GetValue(kuvuGuild)}`");
            }
        }
    }
}
