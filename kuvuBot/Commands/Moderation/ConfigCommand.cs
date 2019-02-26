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
            public OptionType Type { get; set; }

            public Option(string name, string description, OptionType type = OptionType.Field)
            {
                Name = name;
                Description = description;
                Type = type;
            }

            public static List<Option> Options = new List<Option>
            {
                new Option("prefix", "Change bot prefix")
            };
        }

        [Command("list"), Description("Lists config options"), GroupCommand]
        public async Task List(CommandContext ctx)
        {
            await new ModernEmbedBuilder
            {
                Title = "Config options",
                Color = Program.Config.EmbedColor,
                Timestamp = DuckTimestamp.Now,
                Footer = ($"Generated for {ctx.User.Username}#{ctx.User.Discriminator}", ctx.User.AvatarUrl),
                Fields =
                {
                    ("Available options", string.Join("\n", Option.Options.Select(x=>$"**{x.Name}**: `{x.Description}`")))
                }
            }.Send(ctx.Message.Channel);
        }

        [Command("prefix"), Description("Change bot prefix")]
        [RequireUserPermissions(Permissions.ManageGuild), RequireBotPermissions(Permissions.SendMessages)]
        public async Task Prefix(CommandContext ctx, string prefix = null)
        {
            var botContext = new BotContext();
            var kuvuGuild = await ctx.Guild.GetKuvuGuild(botContext);
            if (prefix == null)
            {
                await ctx.RespondAsync($"Current prefix is `{kuvuGuild.Prefix}`");
            }
            else
            {
                kuvuGuild.Prefix = prefix;
                await ctx.RespondAsync($"👌, changed prefix to `{kuvuGuild.Prefix}`");
                botContext.Guilds.Update(kuvuGuild);
                await botContext.SaveChangesAsync();
            }
        }

        [Command("lang"), Description("Change bot language"), Aliases("language", "język", "jezyk")]
        [RequireUserPermissions(Permissions.ManageGuild), RequireBotPermissions(Permissions.SendMessages)]
        public async Task Lang(CommandContext ctx, string lang = null)
        {
            var botContext = new BotContext();
            var kuvuGuild = await ctx.Guild.GetKuvuGuild(botContext);
            if (lang == null)
            {
                await ctx.RespondAsync($"Current language is `{kuvuGuild.Lang}`");
            }
            else
            {
                kuvuGuild.Lang = lang;
                await ctx.RespondAsync($"👌, changed language to `{kuvuGuild.Lang}`");
                botContext.Guilds.Update(kuvuGuild);
                await botContext.SaveChangesAsync();
            }
        }

        [Command("logchannel"), Description("Change log channel")]
        [RequireUserPermissions(Permissions.ManageGuild), RequireBotPermissions(Permissions.SendMessages)]
        public async Task LogChannel(CommandContext ctx, DiscordChannel channel = null)
        {
            var botContext = new BotContext();
            var kuvuGuild = await ctx.Guild.GetKuvuGuild(botContext);
            if (channel == null)
            {
                await ctx.RespondAsync($"Current log channel is `{(kuvuGuild.LogChannel.HasValue ? kuvuGuild.LogChannel.ToString() : "none")}`");
            }
            else
            {
                kuvuGuild.LogChannel = channel.Id;
                await ctx.RespondAsync($"👌, changed log to `{kuvuGuild.LogChannel.ToString()}`");
                botContext.Guilds.Update(kuvuGuild);
                await botContext.SaveChangesAsync();
            }
        }
        [Command("greetingchannel"), Description("Change greeting channel")]
        [RequireUserPermissions(Permissions.ManageGuild), RequireBotPermissions(Permissions.SendMessages)]
        public async Task GreetingChannel(CommandContext ctx, DiscordChannel channel = null)
        {
            var botContext = new BotContext();
            var kuvuGuild = await ctx.Guild.GetKuvuGuild(botContext);
            if (channel == null)
            {
                await ctx.RespondAsync($"Current greeting channel is `{(kuvuGuild.GreetingChannel.HasValue ? kuvuGuild.GreetingChannel.ToString() : "none")}`");
            }
            else
            {
                kuvuGuild.GreetingChannel = channel.Id;
                await ctx.RespondAsync($"👌, changed greeting to `{kuvuGuild.GreetingChannel.ToString()}`");
                botContext.Guilds.Update(kuvuGuild);
                await botContext.SaveChangesAsync();
            }
        }
        [Command("goodbyechannel"), Description("Change goodbye channel")]
        [RequireUserPermissions(Permissions.ManageGuild), RequireBotPermissions(Permissions.SendMessages)]
        public async Task GoodbyeChannel(CommandContext ctx, DiscordChannel channel = null)
        {
            var botContext = new BotContext();
            var kuvuGuild = await ctx.Guild.GetKuvuGuild(botContext);
            if (channel == null)
            {
                await ctx.RespondAsync($"Current goodbye channel is `{(kuvuGuild.GoodbyeChannel.HasValue ? kuvuGuild.GoodbyeChannel.ToString() : "none")}`");
            }
            else
            {
                kuvuGuild.GoodbyeChannel = channel.Id;
                await ctx.RespondAsync($"👌, changed goodbye to `{kuvuGuild.GoodbyeChannel.ToString()}`");
                botContext.Guilds.Update(kuvuGuild);
                await botContext.SaveChangesAsync();
            }
        }

        [Command("greeting"), Description("Change greeting message")]
        [RequireUserPermissions(Permissions.ManageGuild), RequireBotPermissions(Permissions.SendMessages)]
        public async Task Greeting(CommandContext ctx, [RemainingText] string message = null)
        {
            var botContext = new BotContext();
            var kuvuGuild = await ctx.Guild.GetKuvuGuild(botContext);
            if (message == null)
            {
                await ctx.RespondAsync($"Current greeting message is `{(kuvuGuild.GreetingMessage ?? "none")}`");
            }
            else
            {
                kuvuGuild.GreetingMessage = message;
                await ctx.RespondAsync($"👌, changed greeting message to `{kuvuGuild.GreetingMessage}`");
                botContext.Guilds.Update(kuvuGuild);
                await botContext.SaveChangesAsync();
            }
        }
        [Command("goodbye"), Description("Change goodbye message")]
        [RequireUserPermissions(Permissions.ManageGuild), RequireBotPermissions(Permissions.SendMessages)]
        public async Task Goodbye(CommandContext ctx, [RemainingText] string message = null)
        {
            var botContext = new BotContext();
            var kuvuGuild = await ctx.Guild.GetKuvuGuild(botContext);
            if (message == null)
            {
                await ctx.RespondAsync($"Current goodbye message is `{(kuvuGuild.GoodbyeMessage ?? "none")}`");
            }
            else
            {
                kuvuGuild.GoodbyeMessage = message;
                await ctx.RespondAsync($"👌, changed goodbye message to `{kuvuGuild.GoodbyeMessage}`");
                botContext.Guilds.Update(kuvuGuild);
                await botContext.SaveChangesAsync();
            }
        }

        [Command("autorole"), Description("Change autorole")]
        [RequireUserPermissions(Permissions.ManageGuild), RequireBotPermissions(Permissions.SendMessages)]
        public async Task Autorole(CommandContext ctx, DiscordRole role = null)
        {
            var botContext = new BotContext();
            var kuvuGuild = await ctx.Guild.GetKuvuGuild(botContext);
            if (role == null)
            {
                await ctx.RespondAsync($"Current autorole is `{(kuvuGuild.AutoRole.HasValue ? kuvuGuild.AutoRole.ToString() : "none")}`");
            }
            else
            {
                kuvuGuild.AutoRole = role.Id;
                await ctx.RespondAsync($"👌, changed autorole to `{kuvuGuild.AutoRole.ToString()}`");
                botContext.Guilds.Update(kuvuGuild);
                await botContext.SaveChangesAsync();
            }
        }

        [Aliases("showlvlup")]
        [Command("showlevelup"), Description("Toggle level up message")]
        [RequireUserPermissions(Permissions.ManageGuild), RequireBotPermissions(Permissions.SendMessages)]
        public async Task ShowLevelUp(CommandContext ctx, bool? toggle = null)
        {
            var botContext = new BotContext();
            var kuvuGuild = await ctx.Guild.GetKuvuGuild(botContext);
            if (!toggle.HasValue)
            {
                await ctx.RespondAsync($"Current level up message mode is `{(kuvuGuild.ShowLevelUp ? "showed" : "hided")}`");
            }
            else
            {
                kuvuGuild.ShowLevelUp = toggle.Value;
                await ctx.RespondAsync($"👌, changed level up message mode to `{(kuvuGuild.ShowLevelUp ? "showed" : "hided")}`");
                botContext.Guilds.Update(kuvuGuild);
                await botContext.SaveChangesAsync();
            }
        }

        [Group("mute")]
        [Description("Mute configuration commands")]
        public class MuteCommandGroup : BaseCommandModule
        {
            [Command("setup"), Description("Setup mute role")]
            [RequireUserPermissions(Permissions.ManageGuild), RequireBotPermissions(Permissions.SendMessages)]
            public async Task Setup(CommandContext ctx, DiscordRole role = null)
            {
                var botContext = new BotContext();
                var kuvuGuild = await ctx.Guild.GetKuvuGuild(botContext);
                if (role == null)
                {
                    if (kuvuGuild.AutoRole.HasValue)
                    {
                        await ctx.RespondAsync($"Current mute role is `{kuvuGuild.AutoRole.ToString()}`");
                    }
                    else
                    {
                        await ctx.RespondAsync($"Creating mute role...");
                        await ctx.Channel.TriggerTypingAsync();
                        try
                        {
                            role = await ctx.Guild.CreateRoleAsync("Muted", Permissions.None.Revoke(Permissions.SendMessages));
                            await ctx.RespondAsync($"Created {role.Mention}. Roles above will bypass mute");
                        }
                        catch (Exception)
                        {
                            await ctx.RespondAsync($"Can't create role.");
                        }
                    }

                }
                if (role != null)
                {
                    kuvuGuild.MuteRole = role.Id;
                    await ctx.RespondAsync($"👌, changed mute role to `{kuvuGuild.MuteRole.ToString()}`");
                    botContext.Guilds.Update(kuvuGuild);
                    await botContext.SaveChangesAsync();
                }
            }
        }
    }
}
