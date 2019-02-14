using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using kuvuBot.Lang;
using DSharpPlus;
using kuvuBot.Commands.Attributes;
using kuvuBot.Data;
using Microsoft.EntityFrameworkCore.Internal;

namespace kuvuBot.Commands.Moderation
{
    [Group("warn"), LocalizedDescription("warn.group-description"), RequireUserPermissions(Permissions.ManageMessages)]
    public class WarnCommand : BaseCommandModule
    {
        [GroupCommand, LocalizedDescription("warn.description")]
        [RequireUserPermissions(Permissions.ManageMessages), RequireBotPermissions(Permissions.SendMessages)]
        public async Task Warn(CommandContext ctx, [Description("User to warn")] DiscordUser target, [Description("Weight of warn")] int weight, [Description("Reason")] [RemainingText] string reason)
        {
            await ctx.Channel.TriggerTypingAsync();

            var botContext = new BotContext();
            var kuvuGuild = await ctx.Guild.GetKuvuGuild(botContext);
            var kuvuUser = await target.GetKuvuUser(kuvuGuild, botContext);


            var warn = new KuvuWarn
            {
                Date = DateTime.Now,
                User = kuvuUser,
                Warning = ctx.User.Id,
                Weight = weight,
                Reason = reason,
            };
            botContext.Warns.Add(warn);

            await botContext.SaveChangesAsync();
            await ctx.RespondAsync($"Warned {target.Mention} for `{warn.Reason}` (`{warn.Weight}`)");
        }

        [Command("history"), LocalizedDescription("warn.history"), Aliases("list")]
        [RequireUserPermissions(Permissions.ManageMessages), RequireBotPermissions(Permissions.SendMessages)]
        public async Task WarnHistory(CommandContext ctx, [Description("User to list his warns")] DiscordUser target)
        {
            await ctx.Channel.TriggerTypingAsync();
            using (var botContext = new BotContext())
            {
                var kuvuGuild = await ctx.Guild.GetKuvuGuild(botContext);
                var kuvuUser = await target.GetKuvuUser(kuvuGuild, botContext);

                if (kuvuUser.Warns.Count > 0)
                {
                    await new ModernEmbedBuilder
                    {
                        AuthorIcon = target.AvatarUrl,
                        Title = $"{target.Username}'s warns",
                        Fields = new List<DuckField>
                        {
                            ($"{kuvuUser.Warns.Count} warns, {kuvuUser.Warns.Select(w => w.Weight).Sum()} total weight",
                                kuvuUser.Warns.Select(w => $"[{w.Date.ToString("g", CultureInfo.CreateSpecificCulture("pl-PL"))}] Warning `{ctx.Client.GetUserAsync(w.Warning).Result.Name()}` Weight: `{w.Weight}` Reason: `{w.Reason}`").Join("\n"))
                        },
                        Color = Program.Config.EmbedColor,
                        Timestamp = DuckTimestamp.Now,
                        Footer = ($"Generated for {ctx.User.Username}#{ctx.User.Discriminator}", ctx.User.AvatarUrl),
                    }.Send(ctx.Message.Channel);
                }
                else
                {
                    await ctx.RespondAsync($"{target.Name()} doesn't have any warns 🎉");
                }
            }
        }

        [Command("clear"), LocalizedDescription("warn.clear")]
        [RequireUserPermissions(Permissions.ManageMessages), RequireBotPermissions(Permissions.SendMessages)]
        public async Task Clear(CommandContext ctx, [Description("User to clear his warns")] DiscordUser target)
        {
            await ctx.Channel.TriggerTypingAsync();

            var botContext = new BotContext();
            var kuvuGuild = await ctx.Guild.GetKuvuGuild(botContext);
            var kuvuUser = await target.GetKuvuUser(kuvuGuild, botContext);

            botContext.RemoveRange(kuvuUser.Warns);
            await botContext.SaveChangesAsync();

            await ctx.RespondAsync($"Cleared {target.Name()} warns");
        }
    }
}