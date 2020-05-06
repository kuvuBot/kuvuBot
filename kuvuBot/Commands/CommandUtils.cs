﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using DSharpPlus.CommandsNext.Exceptions;
using kuvuBot.Commands.Attributes;

namespace kuvuBot.Commands
{
    public static class CommandUtils
    {
        public static int LevenshteinDistance(string source, string target)
        {
            // degenerate cases
            if (source == target) return 0;
            if (source.Length == 0) return target.Length;
            if (target.Length == 0) return source.Length;

            // create two work vectors of integer distances
            int[] v0 = new int[target.Length + 1];
            int[] v1 = new int[target.Length + 1];

            // initialize v0 (the previous row of distances)
            // this row is A[0][i]: edit distance for an empty s
            // the distance is just the number of characters to delete from t
            for (int i = 0; i < v0.Length; i++)
                v0[i] = i;

            for (int i = 0; i < source.Length; i++)
            {
                // calculate v1 (current row distances) from the previous row v0

                // first element of v1 is A[i+1][0]
                //   edit distance is delete (i+1) chars from s to match empty t
                v1[0] = i + 1;

                // use formula to fill in the rest of the row
                for (int j = 0; j < target.Length; j++)
                {
                    var cost = (source[i] == target[j]) ? 0 : 1;
                    v1[j + 1] = Math.Min(v1[j] + 1, Math.Min(v0[j + 1] + 1, v0[j] + cost));
                }

                // copy v1 (current row) to v0 (previous row) for next iteration
                for (int j = 0; j < v0.Length; j++)
                    v0[j] = v1[j];
            }

            return v1[target.Length];
        }

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
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("Required text can't be null!");
            }
        }

        public static string Category(this Command command)
        {
            var categoryAttribute = (CategoryAttribute) command.CustomAttributes.FirstOrDefault(x => x is CategoryAttribute);
            if (categoryAttribute != null)
                return categoryAttribute.Category;

            return command.Module.ModuleType.Namespace?.Split('.').Last();
        }

        public static string Name(this DiscordUser user, bool displayBot = false)
        {
            var name = $"{user.Username}#{user.Discriminator}";
            if (displayBot && user.IsBot)
            {
                return $"{name} <:bot:658366346048045099>";
            }
            return name;
        }

        public static string GetCurrentAvatarUrl(this DiscordUser user, ushort size)
        {
            return (user.AvatarHash?.StartsWith("a_") == true ? user.GetAvatarUrl(ImageFormat.Gif, size) : user.GetAvatarUrl(ImageFormat.Png, size)) ?? user.DefaultAvatarUrl;
        }
        
        public static async Task SendAutoRemoveMessageAsync(this DiscordChannel channel, TimeSpan delay, string content = null, bool tts = false, DiscordEmbed embed = null)
        {
            var message = await channel.SendMessageAsync(content, tts, embed);
            await Task.Delay(delay).ContinueWith(async t => await message.DeleteAsync());
        }

        public static IEnumerable<DiscordRole> Selectable(this IEnumerable<DiscordRole> roles, ulong guildId)
        {
            return roles.Where(x => x.Id != guildId);
        }

        public static IEnumerable<DiscordChannel> Selectable(this IEnumerable<DiscordChannel> channels, ChannelType type = ChannelType.Text, bool canBeCategory = false)
        {
            return channels.Where(x => x.IsCategory == canBeCategory && x.Type == type);
        }

        public static async Task<DiscordGuild> GetGuildAsync(this DiscordShardedClient shardedClient, ulong id)
        {
            foreach (var client in shardedClient.ShardClients.Values)
            {
                var guild = await client.GetGuildAsync(id);
                if (guild != null)
                {
                    return guild;
                }
            }
            return null;
        }

        public static async Task<DiscordUser> GetUserAsync(this DiscordShardedClient shardedClient, ulong id)
        {
            foreach (var client in shardedClient.ShardClients.Values)
            {
                var user = await client.GetUserAsync(id);
                if (user != null)
                {
                    return user;
                }
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
    }
}
