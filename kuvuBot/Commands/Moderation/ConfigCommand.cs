﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using kuvuBot.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Linq;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using System.Reflection;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using kuvuBot.Core.Commands;
using kuvuBot.Core.Commands.Converters;
using kuvuBot.Lang;

namespace kuvuBot.Commands.Moderation
{
    [Group("config")]
    [DSharpPlus.CommandsNext.Attributes.Description("Configuration commands")]
    [RequireUserPermissions(Permissions.ManageGuild), RequireBotPermissions(Permissions.SendMessages)]
    public class ConfigCommandGroup : BaseCommandModule
    {
        public enum OptionType { Field }

        public class Option<T> : Option<T, T>
        {
            public Option(string name, string description, OptionType type = OptionType.Field, params string[] aliases) : base(name, description, type, aliases) { }
        }

        public class Option<T, TResult> : Option
        {
            public ITwoWayConverter<T, TResult> Converter { get; set; }

            public Option(string name, string description, OptionType type = OptionType.Field, params string[] aliases) : base(name, description, type, aliases) { }

            public override async Task<object> SetValue(object instance, object value, bool useConverter = true, CommandContext ctx = null)
            {
                var optional = await Converter.ConvertAsync<T>(value.ToString(), ctx);

                if (optional.HasValue)
                {
                    value = await Converter.ConvertAsync(optional.Value);
                }

                return await base.SetValue(instance, value, useConverter, ctx);
            }
        }

        public class TooLongException : Exception { }

        public class Option
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Field { get; set; }
            public Type FieldType { get; set; } = typeof(string);
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

            public virtual Task<object> SetValue(object instance, object value, bool useConverter = true, CommandContext ctx = null)
            {
                if (value is string text)
                {
                    if (text.Length > 1800)
                    {
                        throw new TooLongException();
                    }

                    if (value.ToString() == "null" || value.ToString() == "unset" || value.ToString() == "none")
                    {
                        value = null;
                    }
                    else
                    {
                        var converter = TypeDescriptor.GetConverter(GetProperty(instance).PropertyType);
                        value = converter.ConvertFrom(value);
                    }
                }

                GetProperty(instance).SetValue(instance, value);
                return Task.FromResult(value);
            }

            public static List<Option> Options = new List<Option>
            {
                new Option("prefix", "Bot's commands prefix") { Field = "Prefix"},
                new Option<string>("language", "Bot language", aliases: "lang") { Field = "Lang", Converter = new LangConverter() },
                new Option<DiscordChannel, ulong?>("logchannel", "Channel where bot will log") { Field = "LogChannel", Converter = new FriendlyDiscordChannelConverter() },
                new Option<DiscordChannel, ulong?>("greetingchannel", "Channel where bot will say greeting to new users") { Field = "GreetingChannel", Converter = new FriendlyDiscordChannelConverter() },
                new Option<DiscordChannel, ulong?>("goodbyechannel", "Channel where bot will say goodbye to leaving users") { Field = "GoodbyeChannel", Converter = new FriendlyDiscordChannelConverter() },
                new Option("greeting", "Greeting message") { Field = "GreetingMessage" },
                new Option("goodbye", "Goodbye message") { Field = "GoodbyeMessage" },
                new Option<DiscordRole, ulong?>("autorole", "Role that will be given to new users") { Field = "AutoRole", Converter = new FriendlyDiscordRoleConverter() },
                new Option<DiscordRole, ulong?>("muterole", "Role that will be given to muted users") { Field = "MuteRole", Converter = new FriendlyDiscordRoleConverter() },
                new Option<bool>("showlevelup", "Option that enable level up messages") { Field = "ShowLevelUp", Converter = new FriendlyBoolConverter() },
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
                    ("Available options", string.Join("\n", Option.Options.Select(x=>$"**{x.Name}**: `{x.Description}` = `{x.GetValue(kuvuGuild) ?? "not set"}`".Truncate(1000 / Option.Options.Count, "...`")))),
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
                await ctx.RespondAsync((await ctx.Lang("config.get"))
                    .Replace("{name}", option.Name)
                    .Replace("{value}", option.GetValue(kuvuGuild).ToString()));
            }
            else
            {
                object newValue;

                try
                {
                    newValue = await option.SetValue(kuvuGuild, value, true, ctx);
                }
                catch (NotFoundException)
                {
                    await ctx.RespondAsync(await ctx.Lang("config.notFound"));
                    return;
                }
                catch (TooLongException)
                {
                    await ctx.RespondAsync(await ctx.Lang("config.tooLong"));
                    return;
                }
                catch (ArgumentException)
                {
                    await ctx.RespondAsync(await ctx.Lang("config.wrong"));
                    return;
                }

                botContext.Guilds.Update(kuvuGuild);
                await botContext.SaveChangesAsync();

                if (newValue == null)
                {
                    await ctx.RespondAsync((await ctx.Lang("config.unset"))
                        .Replace("{name}", option.Name));
                }
                else
                {
                    await ctx.RespondAsync((await ctx.Lang("config.set"))
                        .Replace("{name}", option.Name)
                        .Replace("{value}", newValue.ToString()));
                }
            }
        }
    }
}
