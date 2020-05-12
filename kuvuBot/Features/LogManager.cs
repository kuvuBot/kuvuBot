using DSharpPlus;
using DSharpPlus.EventArgs;
using kuvuBot.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using kuvuBot.Core.Commands;
using kuvuBot.Core.Features;

namespace kuvuBot.Features
{
    public class LogManager : IFeature
    {
        public LogManager(DiscordShardedClient client)
        {
            client.GuildMemberAdded += Client_GuildMemberAdded;
            client.GuildMemberRemoved += Client_GuildMemberRemoved;

            client.MessageUpdated += Client_MessageUpdated;
            client.MessageDeleted += Client_MessageDeleted;
        }

        private static async Task Client_MessageDeleted(MessageDeleteEventArgs e)
        {
            var kuvuGuild = await e.Guild.GetKuvuGuild();
            if (kuvuGuild.LogChannel.HasValue)
            {
                e.Handled = true;
                var channel = e.Guild.GetChannel(kuvuGuild.LogChannel.Value);
                await Log("❌", $"{e.Message?.Content ?? "*message was not fetched - can't display*"}", kuvuGuild, channel, e.Message.Author, new List<DuckField>() { new DuckField("Channel", $"[{e.Message.Channel.Mention}]({e.Message.JumpLink.ToString()})") });
            }
        }

        private static async Task Client_MessageUpdated(MessageUpdateEventArgs e)
        {
            var kuvuGuild = await e.Guild.GetKuvuGuild();
            if (kuvuGuild.LogChannel.HasValue && e.Message != null && !string.IsNullOrEmpty(e.Message.Content))
            {
                e.Handled = true;
                var channel = e.Guild.GetChannel(kuvuGuild.LogChannel.Value);
                await Log("🗒", $"{e.MessageBefore?.Content ?? "*message was not fetched - can't display*"} -> {e.Message.Content}", kuvuGuild, channel, e.Author,
                    new List<DuckField>() { new DuckField("Channel", $"[{e.Message.Channel.Mention}]({e.Message.JumpLink.ToString()})") });
            }
        }

        private static async Task Log(string emoji, string message, KuvuGuild kuvuGuild, DiscordChannel discordChannel, DiscordUser user, List<DuckField> fields = null, string url = null)
        {
            var embed = new ModernEmbedBuilder
            {
                Url = url,
                Title = $"{emoji} | {message}",
                Color = Program.Config.EmbedColor,
                Timestamp = DuckTimestamp.Now,
                Fields = fields
            };
            if (user != null)
                embed.Author = ($"{user.Name()} | {user.Id}", null, user.AvatarUrl ?? user.DefaultAvatarUrl);
            await embed.Send(discordChannel);
        }

        private static async Task Client_GuildMemberRemoved(GuildMemberRemoveEventArgs e)
        {
            var kuvuGuild = await e.Guild.GetKuvuGuild();
            if (kuvuGuild.LogChannel.HasValue)
            {
                var channel = e.Guild.GetChannel(kuvuGuild.LogChannel.Value);
                await Log("⬅", $"{e.Member.Name()} leave guild", kuvuGuild, channel, e.Member);
            }
            if (kuvuGuild.GoodbyeChannelId.HasValue && !string.IsNullOrWhiteSpace(kuvuGuild.GoodbyeMessage))
            {
                var channel = e.Guild.GetChannel(kuvuGuild.GoodbyeChannelId.Value);
                await channel.SendMessageAsync(kuvuGuild.GoodbyeMessage
                    .Replace("%name%", e.Member.DisplayName)
                    .Replace("%discriminator%", e.Member.Discriminator)
                    .Replace("%mention%", e.Member.Mention));
            }
        }

        private static async Task Client_GuildMemberAdded(GuildMemberAddEventArgs e)
        {
            var kuvuGuild = await e.Guild.GetKuvuGuild();
            if (kuvuGuild.LogChannel.HasValue)
            {
                var channel = e.Guild.GetChannel(kuvuGuild.LogChannel.Value);
                await Log("➡", $"{e.Member.Name()} joined guild", kuvuGuild, channel, e.Member);
            }
            if (kuvuGuild.GreetingChannelId.HasValue && !string.IsNullOrWhiteSpace(kuvuGuild.GreetingMessage))
            {
                var channel = e.Guild.GetChannel(kuvuGuild.GoodbyeChannelId.Value);
                await channel.SendMessageAsync(kuvuGuild.GreetingMessage
                    .Replace("%name%", e.Member.DisplayName)
                    .Replace("%discriminator%", e.Member.Discriminator)
                    .Replace("%mention%", e.Member.Mention));
            }
            if (kuvuGuild.AutoRole.HasValue)
            {
                await e.Member.GrantRoleAsync(e.Guild.GetRole(kuvuGuild.AutoRole.Value));
            }
        }
    }
}