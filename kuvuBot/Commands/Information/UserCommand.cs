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

namespace kuvuBot.Commands.Information
{
    public class UserCommand : BaseCommandModule
    {
        [Command("user"), Description("User information")]
        [RequireBotPermissions(Permissions.SendMessages)]
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
                Color = Program.Config.EmbedColor,
                Timestamp = DuckTimestamp.Now,
                Footer = ($"Generated for {ctx.User.Name()}", ctx.User.AvatarUrl),
                ThumbnailUrl = target.AvatarUrl ?? target.DefaultAvatarUrl,
            };
            using (var botContext = new BotContext())
                if (!target.IsBot)
                {
                    var kuvuGuild = await ctx.Guild.GetKuvuGuild(botContext);
                    var kuvuUser = await target.GetKuvuUser(kuvuGuild, botContext);
                    var globalUser = await target.GetGlobalUser(botContext);

                    embed.AddField("Level", kuvuUser.GetLevel().ToString(), inline: true);
                    embed.AddField("EXP", $"{kuvuUser.Exp}/{KuvuUser.ConvertLevelToExp(kuvuUser.GetLevel() + 1)}", inline: true);
                    embed.AddField("Reputation", globalUser.Reputation.ToString(), inline: true);
                }
            await embed.Send(ctx.Message.Channel);
        }
    }
}
