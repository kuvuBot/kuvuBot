using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using kuvuBot.Core.Commands.Attributes;

namespace kuvuBot.Core.Commands
{
    public static class Extensions
    {
        public static string GetEnumMemberValue<T>(this T value) where T : struct, IConvertible
        {
            return typeof(T)
                .GetTypeInfo()
                .DeclaredMembers
                .SingleOrDefault(x => x.Name == value.ToString())
                ?.GetCustomAttribute<EnumMemberAttribute>(false)
                ?.Value;
        }

        public static void RequireRemainingText(this string text)
        {
            if (string.IsNullOrEmpty(text)) throw new ArgumentException("Required text can't be null!");
        }

        public static string Category(this Command command)
        {
            var categoryAttribute =
                (CategoryAttribute)command.CustomAttributes.FirstOrDefault(x => x is CategoryAttribute);
            return categoryAttribute != null
                ? categoryAttribute.Category
                : command.Module.ModuleType.Namespace?.Split('.').Last();
        }

        public static string Name(this DiscordUser user, bool displayBot = false)
        {
            var name = $"{user.Username}#{user.Discriminator}";
            if (displayBot && user.IsBot) return $"{name} <:bot:658366346048045099>";
            return name;
        }

        public static string GetCurrentAvatarUrl(this DiscordUser user, ushort size = 1024)
        {
            return (user.AvatarHash?.StartsWith("a_") == true
                ? user.GetAvatarUrl(ImageFormat.Gif, size)
                : user.GetAvatarUrl(ImageFormat.Png, size)) ?? user.DefaultAvatarUrl;
        }

        public static async Task SendAutoRemoveMessageAsync(this DiscordChannel channel, TimeSpan delay,
            string content = null, bool tts = false, DiscordEmbed embed = null)
        {
            var message = await channel.SendMessageAsync(content, tts, embed);
            await Task.Delay(delay).ContinueWith(async t => await message.DeleteAsync());
        }

        public static IEnumerable<DiscordRole> Selectable(this IEnumerable<DiscordRole> roles, ulong guildId)
        {
            return roles.Where(x => x.Id != guildId);
        }

        public static IEnumerable<DiscordChannel> Selectable(this IEnumerable<DiscordChannel> channels,
            ChannelType type = ChannelType.Text, bool canBeCategory = false)
        {
            return channels.Where(x => x.IsCategory == canBeCategory && x.Type == type);
        }

        public static async Task<DiscordGuild> GetGuildAsync(this DiscordShardedClient shardedClient, ulong id)
        {
            foreach (var client in shardedClient.ShardClients.Values)
            {
                var guild = await client.GetGuildAsync(id);
                if (guild != null) return guild;
            }

            return null;
        }

        public static async Task<DiscordUser> GetUserAsync(this DiscordShardedClient shardedClient, ulong id)
        {
            foreach (var client in shardedClient.ShardClients.Values)
            {
                var user = await client.GetUserAsync(id);
                if (user != null) return user;
            }

            return null;
        }

        public static async Task<IEnumerable<TResult>> SelectAsync<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Task<TResult>> method)
        {
            return await Task.WhenAll(source.Select(async s => await method(s)));
        }

        public static string EscapeDiscord(this string text)
        {
            return text.Replace("`", @"\`").Replace("*", @"\*").Replace("~", @"\~").Replace("_", @"\_");
        }

        public static string Truncate(this string text, int maxLength, string ellipsis = null)
        {
            return text.Substring(0, Math.Min(text.Length, maxLength - (ellipsis?.Length ?? 0))) + (text.Length >= maxLength ? ellipsis : string.Empty);
        }
    }
}