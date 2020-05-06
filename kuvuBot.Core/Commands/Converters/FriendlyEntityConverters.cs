using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;

namespace kuvuBot.Core.Commands.Converters
{
    public static class ConverterHelper
    {
        public static Task<Optional<T>> ConvertAsync<T>(this IArgumentConverter<T> converter, string value,
            CommandContext ctx)
        {
            var method = converter.GetType().GetRuntimeMethods().FirstOrDefault(x => x.Name.EndsWith("ConvertAsync"));
            var task = method?.Invoke(converter, new object[] {value, ctx});

            return task is Task<Optional<T>> o ? o : null;
        }
    }

    public class FriendlyDiscordUserConverter : IArgumentConverter<DiscordUser>
    {
        public Task<Optional<DiscordUser>> ConvertAsync(string value, CommandContext ctx)
        {
            if (value == "me") return Task.FromResult<Optional<DiscordUser>>(ctx.User);
            return new DiscordUserConverter().ConvertAsync(value, ctx);
        }
    }

    public class FriendlyDiscordMemberConverter : IArgumentConverter<DiscordMember>
    {
        public Task<Optional<DiscordMember>> ConvertAsync(string value, CommandContext ctx)
        {
            if (value == "me") return Task.FromResult<Optional<DiscordMember>>(ctx.Member);
            return new DiscordMemberConverter().ConvertAsync(value, ctx);
        }
    }

    public class FriendlyDiscordChannelConverter : IArgumentConverter<DiscordChannel>
    {
        public Task<Optional<DiscordChannel>> ConvertAsync(string value, CommandContext ctx)
        {
            if (value == "here") return Task.FromResult<Optional<DiscordChannel>>(ctx.Channel);
            return new DiscordChannelConverter().ConvertAsync(value, ctx);
        }
    }

    public class FriendlyDiscordMessageConverter : IArgumentConverter<DiscordMessage>
    {
        public Task<Optional<DiscordMessage>> ConvertAsync(string value, CommandContext ctx)
        {
            if (value == "this") return Task.FromResult<Optional<DiscordMessage>>(ctx.Message);
            return new DiscordMessageConverter().ConvertAsync(value, ctx);
        }
    }

    public class FriendlyDiscordRoleConverter : IArgumentConverter<DiscordRole>
    {
        public Task<Optional<DiscordRole>> ConvertAsync(string value, CommandContext ctx)
        {
            return new DiscordRoleConverter().ConvertAsync(value, ctx);
        }

        public ulong ConvertToDatabaseFormat(DiscordRole value)
        {
            return value.Id;
        }
    }

    public class FriendlyBoolConverter : IArgumentConverter<bool>
    {
        public Task<Optional<bool>> ConvertAsync(string value, CommandContext ctx)
        {
            if (value == "on" || value == "enable" || value == "show" || value == "1")
                return Task.FromResult<Optional<bool>>(true);
            if (value == "off" || value == "disable" || value == "hide" || value == "0")
                return Task.FromResult<Optional<bool>>(false);
            return new BoolConverter().ConvertAsync(value, ctx);
        }
    }
}