using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using kuvuBot.Lang;
using System.Linq;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using kuvuBot.Data;

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

        public async static Task<bool> HasPermission(this CommandContext ctx, Permissions permission, bool respond = true)
        {
            if (ctx.Member.PermissionsIn(ctx.Channel).HasPermission(permission))
            {
                return true;
            }
            if (respond)
                await ctx.RespondAsync(await ctx.Lang("global.nopermission"));
            return false;
        }

        public async static Task<bool> HasGlobalPermission(this CommandContext ctx, KuvuGlobalRank rank, bool respond = true)
        {
            var globalUser = await ctx.User.GetGlobalUser();
            if (globalUser.GlobalRank.HasValue && globalUser.GlobalRank.Value >= rank)
            {
                return true;
            }
            if (respond)
                await ctx.RespondAsync(await ctx.Lang("global.nopermission"));
            return false;
        }

        public static string Category(this Command command)
        {
            return command.Module.ModuleType.Namespace.Split('.').Last();
        }

        public static string Name(this DiscordUser user, bool displayBot = false)
        {
            var name = $"{user.Username}#{user.Discriminator}";
            if (displayBot && user.IsBot)
            {
                return "[BOT] " + name;
            }
            return name;
        }


        public static async Task SendAutoRemoveMessageAsync(this DiscordChannel channel, TimeSpan delay, string content = null, bool tts = false, DiscordEmbed embed = null)
        {
            var message = await channel.SendMessageAsync(content, tts, embed);
            await Task.Delay(delay).ContinueWith(async t => await message.DeleteAsync());
        }
    }
}
