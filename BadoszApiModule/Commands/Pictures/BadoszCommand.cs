using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using kuvuBot.Commands;
using kuvuBot.Commands.Attributes;
using kuvuBot.Core.Commands;
using kuvuBot.Core.Commands.Attributes;
using kuvuBot.Lang;
using Newtonsoft.Json.Linq;

namespace BadoszApiModule.Commands.Pictures
{
    public class BirdCommand : BaseCommandModule
    {
        [Command("bird"), LocalizedDescription("bird.description")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Bird(CommandContext ctx)
        {
            await ctx.Channel.TriggerTypingAsync();
            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.Bird);
        }
    }

    public class BeeCommand : BaseCommandModule
    {
        [Command("bee"), LocalizedDescription("bee.description")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Bee(CommandContext ctx)
        {
            await ctx.Channel.TriggerTypingAsync();
            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.Bee);
        }
    }

    public class RabbitCommand : BaseCommandModule
    {
        [Command("rabbit"), LocalizedDescription("rabbit.description")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Rabbit(CommandContext ctx)
        {
            await ctx.Channel.TriggerTypingAsync();
            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.Rabbit);
        }
    }

    public class BlurpleCommand : BaseCommandModule
    {
        [Command("blurple"), LocalizedDescription("blurple.description")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Blurple(CommandContext ctx, [LocalizedDescription("blurple.target")] DiscordMember target = null)
        {
            await ctx.Channel.TriggerTypingAsync();
            target ??= ctx.Member;
            var avatar = target.GetAvatarUrl(ImageFormat.Png) ?? target.DefaultAvatarUrl;
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["url"] = avatar;
            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.Blurple, $"{await ctx.Lang("blurple.verb")} {target.DisplayName}", query);
        }
    }

    public class MorseCommand : BaseCommandModule
    {
        [Command("morse"), LocalizedDescription("morse.description"), Category("Fun")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Morse(CommandContext ctx, [LocalizedDescription("morse.argument")] [RemainingText] string text)
        {
            text.RequireRemainingText();
            await ctx.Channel.TriggerTypingAsync();
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["text"] = text;
            var json = JObject.Parse(await BadoszApiModule.BadoszApi.GetJson(BadoszApi.BadoszEndpoint.Morse, query));
            var morse = json.GetValue("formatted")?.ToString();

            if (morse == null)
            {
                throw new HttpRequestException();
            }

            if (morse.Length > 2048)
            {
                await new ModernEmbedBuilder
                {
                    Color = DiscordColor.Red,
                    Description = "Given text is too long!"
                }.AddGeneratedForFooter(ctx, false).Send(ctx.Channel);
            }
            else
            {
                await new ModernEmbedBuilder
                {
                    Description = morse
                }.AddGeneratedForFooter(ctx).Send(ctx.Channel);
            }
        }
    }

    public class ChangeMyMindCommand : BaseCommandModule
    {
        [Command("changemymind"), LocalizedDescription("changemymind.description")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task ChangeMyMind(CommandContext ctx, [LocalizedDescription("changemymind.description")] [RemainingText] string text)
        {
            text.RequireRemainingText();
            await ctx.Channel.TriggerTypingAsync();
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["text"] = text;
            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.ChangeMyMind, await ctx.Lang("changemymind.title"), query);
        }
    }

    public class ExcusemeCommand : BaseCommandModule
    {
        [Command("excuseme"), LocalizedDescription("excuseme.description")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Excuseme(CommandContext ctx, [LocalizedDescription("excuseme.argument")] [RemainingText] string text)
        {
            text.RequireRemainingText();
            await ctx.Channel.TriggerTypingAsync();
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["text"] = text;
            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.ExcuseMe, "-_-", query);
        }
    }

    public class FoxCommand : BaseCommandModule
    {
        [Command("fox"), LocalizedDescription("fox.description")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Fox(CommandContext ctx)
        {
            await ctx.Channel.TriggerTypingAsync();
            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.Fox);
        }
    }

    public class InvertCommand : BaseCommandModule
    {
        [Command("invert"), LocalizedDescription("invert.description")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Invert(CommandContext ctx, [LocalizedDescription("invert.target")] DiscordMember target = null)
        {
            await ctx.Channel.TriggerTypingAsync();
            target ??= ctx.Member;
            var avatar = target.GetAvatarUrl(ImageFormat.Png) ?? target.DefaultAvatarUrl;
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["url"] = avatar;
            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.Invert, $"{await ctx.Lang("invert.verb")} {target.DisplayName}", query);
        }
    }

    public class OranglyCommand : BaseCommandModule
    {
        [Command("orangly"), LocalizedDescription("orangly.description")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Orangly(CommandContext ctx, [LocalizedDescription("orangly.target")] DiscordMember target = null)
        {
            await ctx.Channel.TriggerTypingAsync();
            target ??= ctx.Member;
            var avatar = target.GetAvatarUrl(ImageFormat.Png) ?? target.DefaultAvatarUrl;
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["url"] = avatar;
            query["hex"] = "#ffa500";
            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.Colorify, $"{await ctx.Lang("orangly.verb")} {target.DisplayName}", query);
        }
    }

    public class TrumpCommand : BaseCommandModule
    {
        [Command("trump"), LocalizedDescription("trump.description")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Trump(CommandContext ctx, [LocalizedDescription("trump.argument")] [RemainingText] string text)
        {
            text.RequireRemainingText();
            await ctx.Channel.TriggerTypingAsync();
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["text"] = text;
            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.Trump, parameters: query);
        }
    }

    public class WantedCommand : BaseCommandModule
    {
        [Command("wanted"), LocalizedDescription("wanted.description")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Wanted(CommandContext ctx, [LocalizedDescription("wanted.target")] DiscordMember target = null)
        {
            await ctx.Channel.TriggerTypingAsync();
            target ??= ctx.Member;
            var avatar = target.GetAvatarUrl(ImageFormat.Png) ?? target.DefaultAvatarUrl;
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["url"] = avatar;
            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.Wanted, $"{target.DisplayName} {await ctx.Lang("wanted.verb")}", query);
        }
    }

    public class WastedCommand : BaseCommandModule
    {
        [Command("wasted"), LocalizedDescription("wasted.description")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Wasted(CommandContext ctx, [LocalizedDescription("wasted.target")] DiscordMember target = null)
        {
            await ctx.Channel.TriggerTypingAsync();
            target ??= ctx.Member;
            var avatar = target.GetAvatarUrl(ImageFormat.Png) ?? target.DefaultAvatarUrl;
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["url"] = avatar;
            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.Wasted, $"{target.DisplayName} {await ctx.Lang("wasted.verb")}", query);
        }
    }
    
    public class KissCommand : BaseCommandModule
    {
        [Command("kiss"), LocalizedDescription("kiss.description")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Kiss(CommandContext ctx, [LocalizedDescription("kiss.target")] DiscordMember target)
        {
            await ctx.Channel.TriggerTypingAsync();
            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.Kiss, $"{ctx.Member.DisplayName} {await ctx.Lang("kiss.verb")} {target.DisplayName}");
        }
    }
    
    public class PatCommand : BaseCommandModule
    {
        [Command("pat"), LocalizedDescription("pat.description")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Kiss(CommandContext ctx, [LocalizedDescription("pat.target")] DiscordMember target)
        {
            await ctx.Channel.TriggerTypingAsync();
            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.Pat, $"{ctx.Member.DisplayName} {await ctx.Lang("pat.verb")} {target.DisplayName}");
        }
    }
    
    public class PokeCommand : BaseCommandModule
    {
        [Command("poke"), LocalizedDescription("poke.description")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Poke(CommandContext ctx, [LocalizedDescription("poke.target")] DiscordMember target)
        {
            await ctx.Channel.TriggerTypingAsync();
            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.Poke, $"{ctx.Member.DisplayName} {await ctx.Lang("poke.verb")} {target.DisplayName}");
        }
    }
    
    public class TickleCommand : BaseCommandModule
    {
        [Command("tickle"), LocalizedDescription("tickle.description")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Tickle(CommandContext ctx, [LocalizedDescription("tickle.target")] DiscordMember target)
        {
            await ctx.Channel.TriggerTypingAsync();
            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.Tickle, $"{ctx.Member.DisplayName} {await ctx.Lang("tickle.verb")} {target.DisplayName}");
        }
    }
    
    public class CuddleCommand : BaseCommandModule
    {
        [Command("cuddle"), LocalizedDescription("cuddle.description")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Cuddle(CommandContext ctx, [LocalizedDescription("cuddle.target")] DiscordMember target)
        {
            await ctx.Channel.TriggerTypingAsync();
            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.Cuddle, $"{ctx.Member.DisplayName} {await ctx.Lang("cuddle.verb")} {target.DisplayName}");
        }
    }
    
    public class HugCommand : BaseCommandModule
    {
        [Command("hug"), LocalizedDescription("hug.description")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Hug(CommandContext ctx, [LocalizedDescription("hug.target")] DiscordMember target)
        {
            await ctx.Channel.TriggerTypingAsync();
            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.Hug, $"{ctx.Member.DisplayName} {await ctx.Lang("hug.verb")} {target.DisplayName}");
        }
    }
}