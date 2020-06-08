using System;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using Colorful;
using System.Reflection;
using DSharpPlus;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using kuvuBot.Commands.Attributes;
using kuvuBot.Core.Commands;
using kuvuBot.Lang;

namespace kuvuBot.Commands.Fun
{
    public class FigletCommand : BaseCommandModule
    {
        private FigletFont FigletFont { get; }

        public FigletCommand()
        {
            FigletFont = FigletFont.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("kuvuBot.Assets.big.flf"));
        }

        [Command("figlet"), LocalizedDescription("figlet.description")]
        [RequireBotPermissions(Permissions.SendMessages)]
        public async Task Figlet(CommandContext ctx, [RemainingText] string message)
        {
            message.RequireRemainingText();

            var figlet = new Figlet(FigletFont);

            try
            {
                await ctx.RespondAsync($"```{figlet.ToAscii(message).ConcreteValue}```");
            }
            catch (ArgumentException)
            {
                await new ModernEmbedBuilder
                {
                    Color = EmbedUtils.Red,
                    Description = await ctx.Lang("figlet.specialCharacters")
                }.AddGeneratedForFooter(ctx, false).Send(ctx.Channel);
            }
        }
    }
}
