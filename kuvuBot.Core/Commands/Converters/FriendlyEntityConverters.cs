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
        public static void RegisterFriendlyConverters(this CommandsNextExtension commands)
        {
            commands.RegisterConverter(new FriendlyDiscordUserConverter());
            commands.RegisterConverter(new FriendlyDiscordMemberConverter());
            commands.RegisterConverter(new FriendlyDiscordChannelConverter());
            commands.RegisterConverter(new FriendlyDiscordMessageConverter());
            commands.RegisterConverter(new FriendlyBoolConverter());
        }

        public static Task<Optional<T>> ConvertAsync<T>(this IArgumentConverter<T> converter, string value, CommandContext ctx)
        {
            return converter.ConvertAsync(value, ctx);
        }
    }

    public interface ITwoWayConverter : IArgumentConverter {  }

    public interface ITwoWayConverter<T, TResult> : ITwoWayConverter, IArgumentConverter<T>
    {
        Task<TResult> ConvertAsync(T value);
    }

    public abstract class TwoWayConverter<T> : ITwoWayConverter<T, T>
    {
        public Task<T> ConvertAsync(T value)
        {
            return Task.FromResult(value);
        }

        public abstract Task<Optional<T>> ConvertAsync(string value, CommandContext ctx);
    }

    public abstract class SnowflakeObjectConverter<T> : ITwoWayConverter<T, ulong?> where T : SnowflakeObject
    {
        public Task<ulong?> ConvertAsync(T value)
        {
            return Task.FromResult(value?.Id);
        }

        public abstract Task<Optional<T>> ConvertAsync(string value, CommandContext ctx);
    }

    public class FriendlyDiscordUserConverter : SnowflakeObjectConverter<DiscordUser>
    {
        public override Task<Optional<DiscordUser>> ConvertAsync(string value, CommandContext ctx)
        {
            if (value == "me") return Task.FromResult<Optional<DiscordUser>>(ctx.User);
            return new DiscordUserConverter().ConvertAsync(value, ctx);
        }
    }

    public class FriendlyDiscordMemberConverter : SnowflakeObjectConverter<DiscordMember>
    {
        public override Task<Optional<DiscordMember>> ConvertAsync(string value, CommandContext ctx)
        {
            if (value == "me") return Task.FromResult<Optional<DiscordMember>>(ctx.Member);
            return new DiscordMemberConverter().ConvertAsync(value, ctx);
        }
    }

    public class FriendlyDiscordChannelConverter : SnowflakeObjectConverter<DiscordChannel>
    {
        public override Task<Optional<DiscordChannel>> ConvertAsync(string value, CommandContext ctx)
        {
            if (value == "here") return Task.FromResult<Optional<DiscordChannel>>(ctx.Channel);
            return new DiscordChannelConverter().ConvertAsync(value, ctx);
        }
    }

    public class FriendlyDiscordMessageConverter : SnowflakeObjectConverter<DiscordMessage>
    {
        public override Task<Optional<DiscordMessage>> ConvertAsync(string value, CommandContext ctx)
        {
            if (value == "this") return Task.FromResult<Optional<DiscordMessage>>(ctx.Message);
            return new DiscordMessageConverter().ConvertAsync(value, ctx);
        }
    }

    public class FriendlyDiscordRoleConverter : SnowflakeObjectConverter<DiscordRole>
    {
        public override Task<Optional<DiscordRole>> ConvertAsync(string value, CommandContext ctx)
        {
            return new DiscordRoleConverter().ConvertAsync(value, ctx);
        }
    }

    public class FriendlyBoolConverter : TwoWayConverter<bool>
    {
        public override Task<Optional<bool>> ConvertAsync(string value, CommandContext ctx)
        {
            if (value == "on" || value == "enable" || value == "show" || value == "1")
                return Task.FromResult<Optional<bool>>(true);
            if (value == "off" || value == "disable" || value == "hide" || value == "0")
                return Task.FromResult<Optional<bool>>(false);
            return new BoolConverter().ConvertAsync(value, ctx);
        }
    }
}