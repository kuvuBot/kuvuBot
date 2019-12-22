using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using System.Globalization;
using kuvuBot.Data;
using DSharpPlus;
using kuvuBot.Commands.Attributes;
using kuvuBot.Lang;

namespace kuvuBot.Commands.Information
{
    public class UserCommand : BaseCommandModule
    {
        [Command("user"), LocalizedDescription("user.description")]
        [RequireBotPermissions(Permissions.SendMessages)]
        public async Task User(CommandContext ctx, DiscordUser target = null)
        {
            target ??= ctx.User;
            var embed = new ModernEmbedBuilder
            {
                Title = await ctx.Lang("user.title"),
                Fields =
                    {
                        ("Discord tag", target.Name(true), inline: true),
                        ("ID", target.Id.ToString(), inline: true),
                        (await ctx.Lang("user.registration"),
                            target.CreationTimestamp.DateTime.ToString("g", CultureInfo.CreateSpecificCulture("pl-PL")),
                            inline: false),
                    },
                ThumbnailUrl = target.AvatarUrl ?? target.DefaultAvatarUrl,
            }.AddGeneratedForFooter(ctx);
            using (var botContext = new BotContext())
                if (!target.IsBot)
                {
                    var kuvuGuild = await ctx.Guild.GetKuvuGuild(botContext);
                    var kuvuUser = await target.GetKuvuUser(kuvuGuild, botContext);
                    var globalUser = await target.GetGlobalUser(botContext);

                    embed.AddField(await ctx.Lang("user.level"), kuvuUser.GetLevel().ToString(), inline: true);
                    embed.AddField(await ctx.Lang("user.exp"), $"{kuvuUser.Exp}/{KuvuUser.ConvertLevelToExp(kuvuUser.GetLevel() + 1)}", inline: true);
                    embed.AddField(await ctx.Lang("user.rep"), globalUser.Reputation.ToString(), inline: true);
                }
            await embed.Send(ctx.Message.Channel);
        }
    }
}
