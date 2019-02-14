using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Web;
using DSharpPlus;

namespace BadoszApiModule.Commands.Pictures
{
    public class BirdCommand : BaseCommandModule
    {
        [Command("bird"), Description("Send a random bird")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Bird(CommandContext ctx)
        {
            await ctx.Channel.TriggerTypingAsync();
            await BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.bird);
        }
    }

    public class BlurpleCommand : BaseCommandModule
    {
        [Command("blurple"), Description("Blurple marked user")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Blurple(CommandContext ctx, DiscordMember target = null)
        {
            await ctx.Channel.TriggerTypingAsync();
            target = target ?? ctx.Member;
            var avatar = target.GetAvatarUrl(DSharpPlus.ImageFormat.Png) ?? target.DefaultAvatarUrl;
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["url"] = avatar;
            await BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.blurple, $"Blurpled {target.DisplayName}", query);
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
            await BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.changemymind, "Change my mind", query);
        }
    }

    public class CuddleCommand : BaseCommandModule
    {
        [Command("cuddle"), Description("Cuddles someone")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Cuddle(CommandContext ctx, DiscordMember target = null)
        {
            await ctx.Channel.TriggerTypingAsync();
            target = target ?? ctx.Member;
            string title;
            if(ctx.Member == target)
            {
                title = $"Take a cuddle, {ctx.Member.DisplayName}!";
            }else
            {
                title = $"{ctx.Member.DisplayName} cuddled {target.DisplayName}";
            }

            await BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.cuddle, title);
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
            await BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.excuseme, "-_-", query);
        }
    }

    public class FoxCommand : BaseCommandModule
    {
        [Command("fox"), Description("Send a fox image")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Fox(CommandContext ctx)
        {
            await ctx.Channel.TriggerTypingAsync();
            await BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.fox);
        }
    }

    public class InvertCommand : BaseCommandModule
    {
        [Command("invert"), Description("Inverts user avatar colors")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Invert(CommandContext ctx, DiscordMember target = null)
        {
            await ctx.Channel.TriggerTypingAsync();
            target = target ?? ctx.Member;
            var avatar = target.GetAvatarUrl(DSharpPlus.ImageFormat.Png) ?? target.DefaultAvatarUrl;
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["url"] = avatar;
            await BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.invert, $"Inverted {target.DisplayName}", query);
        }
    }

    public class KissCommand : BaseCommandModule
    {
        [Command("kiss"), Description("Kiss someone")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Kiss(CommandContext ctx, DiscordMember target = null)
        {
            await ctx.Channel.TriggerTypingAsync();
            target = target ?? ctx.Member;
            await BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.kiss);
        }
    }

    public class NoteCommand : BaseCommandModule
    {
        [Command("note"), Description("*Gives a note*")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Note(CommandContext ctx, [RemainingText] string text)
        {
            await ctx.Channel.TriggerTypingAsync();
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["text"] = text;
            await BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.note, parameters: query);
        }
    }

    public class OranglyCommand : BaseCommandModule
    {
        [Command("orangly"), Description("Orangly someone")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Orangly(CommandContext ctx, DiscordMember target = null)
        {
            await ctx.Channel.TriggerTypingAsync();
            target = target ?? ctx.Member;
            var avatar = target.GetAvatarUrl(DSharpPlus.ImageFormat.Png) ?? target.DefaultAvatarUrl;
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["url"] = avatar;
            await BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.orangly, $"Orangled {target.DisplayName}", query);
        }
    }

    public class TriggeredCommand : BaseCommandModule
    {
        [Command("triggered"), Description("Triggers specified user")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Triggered(CommandContext ctx, DiscordMember target = null)
        {
            await ctx.Channel.TriggerTypingAsync();
            target = target ?? ctx.Member;
            var avatar = target.GetAvatarUrl(DSharpPlus.ImageFormat.Png) ?? target.DefaultAvatarUrl;
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["url"] = avatar;
            await BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.triggered, $"Triggered {target.DisplayName}", query);
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
            await BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.trump, parameters: query);
        }
    }

    public class TweetCommand : BaseCommandModule
    {
        [Command("tweet"), Description("Shows specified user tweet")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Tweet(CommandContext ctx, DiscordMember target, [RemainingText] string text)
        {
            await ctx.Channel.TriggerTypingAsync();
            var avatar = target.GetAvatarUrl(DSharpPlus.ImageFormat.Png) ?? target.DefaultAvatarUrl;
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["text"] = text;
            query["url"] = avatar;
            query["username"] = target.DisplayName;
            await BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.tweet, parameters: query);
        }
    }

    public class WantedCommand : BaseCommandModule
    {
        [Command("wanted"), Description("Wants someone")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Wanted(CommandContext ctx, DiscordMember target = null)
        {
            await ctx.Channel.TriggerTypingAsync();
            target = target ?? ctx.Member;
            var avatar = target.GetAvatarUrl(DSharpPlus.ImageFormat.Png) ?? target.DefaultAvatarUrl;
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["url"] = avatar;
            await BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.wanted, $"{target.DisplayName} is wanted!", query);
        }
    }

    public class WastedCommand : BaseCommandModule
    {
        [Command("wasted"), Description("Kills somebody *in gta of course*")]
        [RequireBotPermissions(Permissions.SendMessages | Permissions.AttachFiles)]
        public async Task Wasted(CommandContext ctx, DiscordMember target = null)
        {
            await ctx.Channel.TriggerTypingAsync();
            target = target ?? ctx.Member;
            var avatar = target.GetAvatarUrl(DSharpPlus.ImageFormat.Png) ?? target.DefaultAvatarUrl;
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["url"] = avatar;
            await BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.wasted, $"{target.DisplayName} died", query);
        }
    }
}