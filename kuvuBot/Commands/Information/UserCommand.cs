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

namespace kuvuBot.Commands.Information
{
    public class UserCommand : BaseCommandModule
    {
        [Command("user"), Description("User information")]
        public async Task User(CommandContext ctx, DiscordUser target = null)
        {
            target = target ?? ctx.User;
            var embed = new ModernEmbedBuilder
            {
                Title = "User information",
                Fields =
                    {
                        ("Discord tag", target.Name(true), inline: true),
                        ("Id", target.Id.ToString(), inline: true),
                        ("Registration date",
                            target.CreationTimestamp.DateTime.ToString("g", CultureInfo.CreateSpecificCulture("pl-PL")),
                            inline: false),
                    },
                Color = new DuckColor(33, 150, 243),
                Timestamp = DuckTimestamp.Now,
                Footer = ($"Generated for {ctx.User.Name()}", ctx.User.AvatarUrl),
                ThumbnailUrl = target.AvatarUrl ?? target.DefaultAvatarUrl,
            };
            using (var botContext = new BotContext())
                if (!target.IsBot)
                {
                    var kuvuGuild = await ctx.Guild.GetKuvuGuild(botContext);
                    var kuvuUser = await target.GetKuvuUser(kuvuGuild, botContext);

                    embed.AddField("Level", kuvuUser.GetLevel().ToString(), inline: true);
                    embed.AddField("EXP", $"{kuvuUser.Exp}/{kuvuUser.ConvertLevelToExp(kuvuUser.GetLevel() + 1)}", inline: true);
                }
            await embed.Send(ctx.Message.Channel);
        }
    }
}
