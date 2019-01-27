using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace kuvuBot.Commands.Pictures
{
    public class HugCommand : BaseCommandModule
    {
        [Command("hug"), Description("Hugs the marked user")]
        public async Task Hug(CommandContext ctx, [Description("User to hug")] DiscordUser target)
        {
            await ctx.Channel.TriggerTypingAsync();
            using (WebClient wc = new WebClient())
            {
                var response = JsonConvert.DeserializeObject<RemResponse>(wc.DownloadString("https://rra.ram.moe/i/r?type=hug"));
                var embed = new ModernEmbedBuilder
                {
                    Title = $"{ctx.User.Username} hugged {target.Username}",
                    Color = new DuckColor(33, 150, 243),
                    Timestamp = DuckTimestamp.Now,
                    ImageUrl = "https://rra.ram.moe" + response.Path,
                    Footer = ($"Generated for {ctx.User.Username}#{ctx.User.Discriminator}", ctx.User.AvatarUrl),
                };
                await embed.Send(ctx.Channel);
            }
        }
    }
}
