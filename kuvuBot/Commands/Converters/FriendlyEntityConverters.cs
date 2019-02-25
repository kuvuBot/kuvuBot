using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;

namespace kuvuBot.Commands.Converters
{
    public class FriendlyDiscordUserConverter : IArgumentConverter<DiscordUser>
    {
        public Task<Optional<DiscordUser>> ConvertAsync(string value, CommandContext ctx)
        {
            if (value == "me")
            {
                return Task.FromResult<Optional<DiscordUser>>(ctx.User);
            }
            return new DiscordUserConverter().ConvertAsync(value, ctx);
        }
    }

    public class FriendlyDiscordMemberConverter : IArgumentConverter<DiscordMember>
    {
        public Task<Optional<DiscordMember>> ConvertAsync(string value, CommandContext ctx)
        {
            if (value == "me")
            {
                return Task.FromResult<Optional<DiscordMember>>(ctx.Member);
            }
            return new DiscordMemberConverter().ConvertAsync(value, ctx);
        }
    }

    public class FriendlyDiscordChannelConverter : IArgumentConverter<DiscordChannel>
    {
        public Task<Optional<DiscordChannel>> ConvertAsync(string value, CommandContext ctx)
        {
            if (value == "here")
            {
                return Task.FromResult<Optional<DiscordChannel>>(ctx.Channel);
            }
            return new DiscordChannelConverter().ConvertAsync(value, ctx);
        }
    }

    public class FriendlyDiscordMessageConverter : IArgumentConverter<DiscordMessage>
    {
        public Task<Optional<DiscordMessage>> ConvertAsync(string value, CommandContext ctx)
        {
            if (value == "this")
            {
                return Task.FromResult<Optional<DiscordMessage>>(ctx.Message);
            }
            return new DiscordMessageConverter().ConvertAsync(value, ctx);
        }
    }

    public class FriendlyBoolConverter : IArgumentConverter<bool>
    {
        public Task<Optional<bool>> ConvertAsync(string value, CommandContext ctx)
        {
            if (value == "on" || value == "enable" || value == "show" || value == "1")
            {
                return Task.FromResult<Optional<bool>>(true);
            }
            else if (value == "off" || value == "disable" || value == "hide" || value == "0")
            {
                return Task.FromResult<Optional<bool>>(false);
            }
            return new BoolConverter().ConvertAsync(value, ctx);
        }
    }
}
