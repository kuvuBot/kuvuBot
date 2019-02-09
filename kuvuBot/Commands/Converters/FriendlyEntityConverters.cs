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
}
