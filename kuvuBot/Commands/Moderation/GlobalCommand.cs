using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using kuvuBot.Data;
using DSharpPlus.Entities;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using System.Reflection;
using kuvuBot.Features.Modular;

namespace kuvuBot.Commands.Moderation
{
    [Group("global")]
    [Description("KuvuBot configuration commands")]
    [Hidden]
    public class GlobalCommandGroup : BaseCommandModule
    {
        [Command("modules"), Description("Print active modules")]
        public async Task Modules(CommandContext ctx)
        {
            if (!await ctx.HasGlobalPermission(KuvuGlobalRank.Root))
            {
                return;
            }
            var embed = new ModernEmbedBuilder
            {
                Title = "Modules",
                Color = Program.Config.EmbedColor,
                Timestamp = DuckTimestamp.Now,
                Footer = ($"Generated for {ctx.User.Username}#{ctx.User.Discriminator}", ctx.User.AvatarUrl),
                Fields = new List<DuckField>
                {
                    ("kuvuBot", $"Version: {Assembly.GetExecutingAssembly().GetName().Version.ToString(3)}")
                }
            };

            foreach(var module in ModuleManager.Modules)
            {
                embed.AddField(module.Name, $"Version: {module.Version}, Author: {module.Author}, Description: `{module.Description}`");
            }

            await embed.Send(ctx.Message.Channel);
        }
    }
}
