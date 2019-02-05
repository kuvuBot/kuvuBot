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

namespace BadoszApiModule.Commands.Pictures
{
    public class BirdCommand : BaseCommandModule
    {
        [Command("bird"), Description("Send a random bird")]
        public async Task Bird(CommandContext ctx)
        {
            await ctx.Channel.TriggerTypingAsync();
            await BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.bird);
        }
    }

    public class BlurpleCommand : BaseCommandModule
    {
        [Command("blurple"), Description("Blurple marked user")]
        public async Task Blurple(CommandContext ctx, DiscordMember target)
        {
            await ctx.Channel.TriggerTypingAsync();
            var avatar = target.GetAvatarUrl(DSharpPlus.ImageFormat.Png) ?? target.DefaultAvatarUrl;
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["url"] = avatar;
            await BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.blurple, query);
        }
    }

    public class ChangeMyMindCommand : BaseCommandModule
    {
        [Command("changemymind"), Description("~~zmień moje zdanie~~")]
        public async Task ChangeMyMind(CommandContext ctx, [RemainingText] string text)
        {
            await ctx.Channel.TriggerTypingAsync();
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["text"] = text;
            await BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.changemymind, query);
        }
    }

    public class CuddleCommand : BaseCommandModule
    {
        [Command("cuddle"), Description("Send a cuddle gif")]
        public async Task Cuddle(CommandContext ctx)
        {
            await ctx.Channel.TriggerTypingAsync();
            await BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.cuddle);
        }
    }

    public class ExcusemeCommand : BaseCommandModule
    {
        [Command("excuseme"), Description("Excuse me wtf?")]
        public async Task Excuseme(CommandContext ctx, [RemainingText] string text)
        {
            await ctx.Channel.TriggerTypingAsync();
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["text"] = text;
            await BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.excuseme, query);
        }
    }

    public class FoxCommand : BaseCommandModule
    {
        [Command("fox"), Description("Send a fox image")]
        public async Task Fox(CommandContext ctx)
        {
            await ctx.Channel.TriggerTypingAsync();
            await BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.fox);
        }
    }

    public class InvertCommand : BaseCommandModule
    {
        [Command("invert"), Description("Inverts user avatar colors")]
        public async Task Invert(CommandContext ctx, DiscordMember target)
        {
            await ctx.Channel.TriggerTypingAsync();
            var avatar = target.GetAvatarUrl(DSharpPlus.ImageFormat.Png) ?? target.DefaultAvatarUrl;
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["url"] = avatar;
            await BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.invert, query);
        }
    }

    public class KissCommand : BaseCommandModule
    {
        [Command("kiss"), Description("Send kiss gif")]
        public async Task Kiss(CommandContext ctx)
        {
            await ctx.Channel.TriggerTypingAsync();
            await BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.kiss);
        }
    }

    public class NoteCommand : BaseCommandModule
    {
        [Command("note"), Description("*Gives a note*")]
        public async Task Note(CommandContext ctx, [RemainingText] string text)
        {
            await ctx.Channel.TriggerTypingAsync();
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["text"] = text;
            await BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.note, query);
        }
    }

    public class OranglyCommand : BaseCommandModule
    {
        [Command("orangly"), Description("*Gives a note*")]
        public async Task Orangly(CommandContext ctx, DiscordMember target)
        {
            await ctx.Channel.TriggerTypingAsync();
            var avatar = target.GetAvatarUrl(DSharpPlus.ImageFormat.Png) ?? target.DefaultAvatarUrl;
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["url"] = avatar;
            await BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.orangly, query);
        }
    }

    public class TriggeredCommand : BaseCommandModule
    {
        [Command("triggered"), Description("Triggers specified user")]
        public async Task Triggered(CommandContext ctx, DiscordMember target)
        {
            await ctx.Channel.TriggerTypingAsync();
            var avatar = target.GetAvatarUrl(DSharpPlus.ImageFormat.Png) ?? target.DefaultAvatarUrl;
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["url"] = avatar;
            await BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.triggered, query);
        }
    }

    public class TrumpCommand : BaseCommandModule
    {
        [Command("trump"), Description("Shows 100% scam, no legit Trump's tweet")]
        public async Task Trump(CommandContext ctx, [RemainingText] string text)
        {
            await ctx.Channel.TriggerTypingAsync();
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["text"] = text;
            await BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.trump, query);
        }
    }

    public class TweetCommand : BaseCommandModule
    {
        [Command("tweet"), Description("Shows specified user tweet")]
        public async Task Tweet(CommandContext ctx, DiscordMember target, [RemainingText] string text)
        {
            await ctx.Channel.TriggerTypingAsync();
            var avatar = target.GetAvatarUrl(DSharpPlus.ImageFormat.Png) ?? target.DefaultAvatarUrl;
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["text"] = text;
            query["url"] = avatar;
            query["username"] = target.DisplayName;
            await BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.tweet, query);
        }
    }

    public class WantedCommand : BaseCommandModule
    {
        [Command("wanted"), Description("Wants someone")]
        public async Task Wanted(CommandContext ctx, DiscordMember target)
        {
            await ctx.Channel.TriggerTypingAsync();
            var avatar = target.GetAvatarUrl(DSharpPlus.ImageFormat.Png) ?? target.DefaultAvatarUrl;
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["url"] = avatar;
            await BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.wanted, query);
        }
    }

    public class WastedCommand : BaseCommandModule
    {
        [Command("wasted"), Description("Kills somebody *in gta of course*")]
        public async Task Orangly(CommandContext ctx, DiscordMember target)
        {
            await ctx.Channel.TriggerTypingAsync();
            var avatar = target.GetAvatarUrl(DSharpPlus.ImageFormat.Png) ?? target.DefaultAvatarUrl;
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["url"] = avatar;
            await BadoszApi.SendEmbedImage(ctx, BadoszApi.BadoszEndpoint.wasted, query);
        }
    }
}