using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Colorful;
using DSharpPlus.Entities;
using System.Reflection;
using System.IO;

namespace kuvuBot.Commands.Fun
{
    public class FigletCommand : BaseCommandModule
    {
        [Command("figlet"), Description("Generates a figlet")]
        public async Task Figlet(CommandContext ctx, [RemainingText] string message)
        {
            // load figlet font from assembly (.exe)
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "kuvuBot.big.flf";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                var figlet = new Figlet(FigletFont.Load(stream));
                await ctx.RespondAsync($"```{figlet.ToAscii(message).ConcreteValue}```");
            }
        }
    }
}
