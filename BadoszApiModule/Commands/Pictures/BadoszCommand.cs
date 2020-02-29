using System;
using System.Threading.Tasks;
using System.Web;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using kuvuBot.Commands;
using kuvuBot.Commands.Attributes;
using Newtonsoft.Json.Linq;

namespace BadoszApiModule.Commands.Pictures
{
    public class BirdCommand : BaseCommandModule
    {
        [Command("bird"), Description("Send a random bird")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Bird(CommandContext ctx)
        {
            await ctx.Channel.TriggerTypingAsync();
            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.Bird);
        }
    }

    public class BeeCommand : BaseCommandModule
    {
        [Command("bee"), Description("Send a random bee")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Bee(CommandContext ctx)
        {
            await ctx.Channel.TriggerTypingAsync();
            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.Bee);
        }
    }

    public class RabbitCommand : BaseCommandModule
    {
        [Command("rabbit"), Description("Send a random rabbit")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Rabbit(CommandContext ctx)
        {
            await ctx.Channel.TriggerTypingAsync();
            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.Rabbit);
        }
    }

    public class BlurpleCommand : BaseCommandModule
    {
        [Command("blurple"), Description("Blurple marked user")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Blurple(CommandContext ctx, DiscordMember target = null)
        {
            await ctx.Channel.TriggerTypingAsync();
            target ??= ctx.Member;
            var avatar = target.GetAvatarUrl(ImageFormat.Png) ?? target.DefaultAvatarUrl;
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["url"] = avatar;
            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.Blurple, $"Blurpled {target.DisplayName}", query);
        }
    }

    public class MorseCommand : BaseCommandModule
    {
        [Command("morse"), Description("Morse text"), Category("Fun")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Morse(CommandContext ctx, [RemainingText] string text)
        {
            await ctx.Channel.TriggerTypingAsync();
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["text"] = text;
            var json = JObject.Parse(await BadoszApiModule.BadoszApi.GetJson(BadoszApi.BadoszEndpoint.Morse, query));
            var morse = json.GetValue("formatted").ToString();

            await new ModernEmbedBuilder
            {
                Description = morse
            }.AddGeneratedForFooter(ctx).Send(ctx.Channel);
        }
    }

    public class ChangeMyMindCommand : BaseCommandModule
    {
        [Command("changemymind"), Description("Change my mind gif")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task ChangeMyMind(CommandContext ctx, [RemainingText] string text)
        {
            await ctx.Channel.TriggerTypingAsync();
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["text"] = text;
            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.ChangeMyMind, "Change my mind", query);
        }
    }

    public class CuddleCommand : BaseCommandModule
    {
        [Command("cuddle"), Description("Cuddles someone")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Cuddle(CommandContext ctx, DiscordMember target = null)
        {
            await ctx.Channel.TriggerTypingAsync();
            target ??= ctx.Member;
            var title = ctx.Member == target 
                ? $"Take a cuddle, {ctx.Member.DisplayName}!" 
                : $"{ctx.Member.DisplayName} cuddled {target.DisplayName}";

            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.Cuddle, title);
        }
    }

    public class ExcusemeCommand : BaseCommandModule
    {
        [Command("excuseme"), Description("Excuse me wtf?")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Excuseme(CommandContext ctx, [RemainingText] string text)
        {
            await ctx.Channel.TriggerTypingAsync();
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["text"] = text;
            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.ExcuseMe, "-_-", query);
        }
    }

    public class FoxCommand : BaseCommandModule
    {
        [Command("fox"), Description("Send a fox image")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Fox(CommandContext ctx)
        {
            await ctx.Channel.TriggerTypingAsync();
            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.Fox);
        }
    }

    public class InvertCommand : BaseCommandModule
    {
        [Command("invert"), Description("Inverts user avatar colors")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Invert(CommandContext ctx, DiscordMember target = null)
        {
            await ctx.Channel.TriggerTypingAsync();
            target ??= ctx.Member;
            var avatar = target.GetAvatarUrl(ImageFormat.Png) ?? target.DefaultAvatarUrl;
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["url"] = avatar;
            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.Invert, $"Inverted {target.DisplayName}", query);
        }
    }

    public class KissCommand : BaseCommandModule
    {
        [Command("kiss"), Description("Kiss someone")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Kiss(CommandContext ctx, DiscordMember target = null)
        {
            await ctx.Channel.TriggerTypingAsync();
            target ??= ctx.Member;
            var title = ctx.Member == target
                ? $"Take a kiss, {ctx.Member.DisplayName}!"
                : $"{ctx.Member.DisplayName} kissed {target.DisplayName}";

            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.Kiss, title);
        }
    }

    public class OranglyCommand : BaseCommandModule
    {
        [Command("orangly"), Description("Orangly someone")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Orangly(CommandContext ctx, DiscordMember target = null)
        {
            await ctx.Channel.TriggerTypingAsync();
            target ??= ctx.Member;
            var avatar = target.GetAvatarUrl(ImageFormat.Png) ?? target.DefaultAvatarUrl;
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["url"] = avatar;
            query["hex"] = "#ffa500";
            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.Colorify, $"Orangled {target.DisplayName}", query);
        }
    }

    public class TrumpCommand : BaseCommandModule
    {
        [Command("trump"), Description("Shows 100% scam, no legit Trump's tweet")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Trump(CommandContext ctx, [RemainingText] string text)
        {
            await ctx.Channel.TriggerTypingAsync();
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["text"] = text;
            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.Trump, parameters: query);
        }
    }

    public class WantedCommand : BaseCommandModule
    {
        [Command("wanted"), Description("Wants someone")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Wanted(CommandContext ctx, DiscordMember target = null)
        {
            await ctx.Channel.TriggerTypingAsync();
            target ??= ctx.Member;
            var avatar = target.GetAvatarUrl(ImageFormat.Png) ?? target.DefaultAvatarUrl;
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["url"] = avatar;
            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.Wanted, $"{target.DisplayName} is wanted!", query);
        }
    }

    public class WastedCommand : BaseCommandModule
    {
        [Command("wasted"), Description("Kills somebody *in gta of course*")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Wasted(CommandContext ctx, DiscordMember target = null)
        {
            await ctx.Channel.TriggerTypingAsync();
            target ??= ctx.Member;
            var avatar = target.GetAvatarUrl(ImageFormat.Png) ?? target.DefaultAvatarUrl;
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["url"] = avatar;
            await BadoszApiModule.BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.Wasted, $"{target.DisplayName} died", query);
        }
    }
}