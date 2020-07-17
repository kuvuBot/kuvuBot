using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using Colorful;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using kuvuBot.Commands;
using kuvuBot.Commands.Attributes;
using kuvuBot.Commands.Fun;
using kuvuBot.Commands.General;
using kuvuBot.Commands.Music;
using kuvuBot.Core.Commands;
using kuvuBot.Core.Commands.Converters;
using kuvuBot.Core.Features;
using kuvuBot.Data;
using kuvuBot.Lang;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Console = Colorful.Console;

namespace kuvuBot
{
    public class Program
    {
        private const string ConfigFilename = "config.json";

        public static DiscordShardedClient Client { get; set; }
        public static Config Config { get; set; }
        public static IReadOnlyDictionary<int, CommandsNextExtension> Commands { get; set; }
        public static IReadOnlyDictionary<int, LavalinkExtension> Lavalink { get; set; }
        private static bool Kill { get; set; }
        private static bool Loaded { get; set; }

        public static Config LoadConfig()
        {
            if (!File.Exists(ConfigFilename))
            {
                throw new Exception($"{ConfigFilename} not found. Copy from example and fill it");
            }

            return Config ??= JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigFilename));
        }

        public static void UpdateDatabase()
        {
            Console.WriteLine("Checking database connection...");
            try
            {
                using var botContext = new BotContext();
                if (botContext.Database.CanConnect())
                {
                    Console.WriteLine("Database connection is OK");
                    Console.WriteLine("Migrating database...");
                    try
                    {
                        botContext.Database.Migrate();
                        Console.WriteLine("Database migration SUCCESS");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Database migration error\n {e}");
                    }
                }
                else
                {
                    Console.WriteLine("Database error");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Database error\n {e}");
            }
        }

        public static async Task Main(string[] args)
        {
            Console.WriteLine(new Figlet().ToAscii("kuvuBot"), Color.Cyan);

            LoadConfig();
            await LangController.LoadTranslations();

            var conf = new DiscordConfiguration
            {
                Token = Config.Token,
                TokenType = TokenType.Bot,

                AutoReconnect = true,
#if DEBUG
                LogLevel = LogLevel.Debug,
#else
                LogLevel = LogLevel.Info,
#endif
                UseInternalLogHandler = true,
                HttpTimeout = TimeSpan.FromSeconds(60)
            };

            Client = new DiscordShardedClient(conf);

            Lavalink = await Client.UseLavalinkAsync();

            var services = new ServiceCollection()
                .AddSingleton(Config)
                .AddSingleton(Client);
            services.AddSingleton(services);

            Commands = await Client.UseCommandsNextAsync(new CommandsNextConfiguration
            {
                EnableDefaultHelp = true,
                PrefixResolver = async msg =>
                {
                    var kuvuGuild = await msg.Channel.Guild.GetKuvuGuild();
                    return msg.GetStringPrefixLength(kuvuGuild.Prefix, StringComparison.CurrentCultureIgnoreCase);
                },
                Services = services.BuildServiceProvider()
            });

            foreach (var extension in Commands.Values)
            {
                extension.SetHelpFormatter<HelpFormatter>();
                extension.RegisterFriendlyConverters();

                extension.CommandExecuted += Commands_CommandExecuted;
                extension.CommandErrored += Commands_CommandErrored;

                extension.RegisterCommands(Assembly.GetExecutingAssembly());
            }

            Client.ClientErrored += e =>
            {
                Console.WriteLine(e.Exception);
                return Task.CompletedTask;
            };
            Client.Ready += Client_Ready;
            Client.GuildCreated += Client_GuildEvents;
            Client.GuildDeleted += Client_GuildEvents;
            Client.GuildDownloadCompleted += e =>
            {
                Client.DebugLogger.LogMessage(LogLevel.Info, "kuvuBot", $"Guild download completed {e.Guilds.Count}", DateTime.Now);
                Loaded = true;
                return Client_GuildEvents(e);
            };
            Client.MessageReactionAdded += MinesweeperCommand.Client_MessageReactionAdded;
            Client.SocketErrored += Client_SocketErrored;

            services.RegisterFeatures();

            UpdateDatabase();

            await Client.StartAsync();
            Client.Ready += e => e.Client.UpdateStatusAsync(GetDiscordActivity(), Config.Status.UserStatus);

            try
            {
                var endpoint = new ConnectionEndpoint {Hostname = Config.Lavalink.Ip, Port = Config.Lavalink.Port};
                foreach (var extension in Lavalink.Values)
                {
                    MusicCommand.Lavalink = await extension.ConnectAsync(new LavalinkConfiguration
                    {
                        Password = Config.Lavalink.Password,
                        RestEndpoint = endpoint,
                        SocketEndpoint = endpoint
                    });
                }
            }
            catch (Exception ex)
            {
                if (ex is SocketException || ex is HttpRequestException || ex is WebSocketException)
                {
                    Console.WriteLine("Can't connect to lavalink! (music commands are disabled)", Color.Red);
                }
                else
                {
                    throw;
                }
            }

            await Task.Delay(-1);
        }

        private static Task Client_SocketErrored(SocketErrorEventArgs e)
        {
            throw e.Exception;
        }

        private static async Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            switch (e.Exception)
            {
                case CommandNotFoundException _:
                    return;
                case ArgumentException _ when e.Exception.StackTrace.Trim().StartsWith("at DSharpPlus.CommandsNext.Command.ExecuteAsync"):
                case ArgumentException _ when e.Exception.Message == "Required text can't be null!":
                case InvalidOperationException _ when e.Exception.Message == "No matching subcommands were found, and this group is not executable.":
                {
                    var cmd = e.Context.CommandsNext.FindCommand("help", out var args);
                    var fctx = e.Context.CommandsNext.CreateFakeContext(e.Context.User, e.Context.Channel, "help", e.Context.Prefix, cmd, e.Command.Name);
                    await e.Context.CommandsNext.ExecuteCommandAsync(fctx).ConfigureAwait(false);
                    return;
                }
                case HttpRequestException _:
                    await new ModernEmbedBuilder
                    {
                        Title = "External web api error",
                        Color = DiscordColor.Red,
                        Description = "Please try again later."
                    }.AddGeneratedForFooter(e.Context, false).Send(e.Context.Message.Channel);
                    return;
                case BotKuvuUserException _:
                    await new ModernEmbedBuilder
                    {
                        Title = "Tried creating kuvuUser for bot account",
                        Color = DiscordColor.Red,
                        Description = "Sorry but this command doesn't support bot accounts."
                    }.AddGeneratedForFooter(e.Context, false).Send(e.Context.Message.Channel);
                    return;
                case ChecksFailedException ex when ex.FailedChecks.Any(x => x is RequireBotPermissionsAttribute):
                {
                    var req = (RequireBotPermissionsAttribute) ex.FailedChecks.First(x => x is RequireBotPermissionsAttribute);
                    var dm = await e.Context.Member.CreateDmChannelAsync();
                    await dm.SendMessageAsync($"I don't have `{req.Permissions.ToPermissionString()}` permissions, so I can't do it! Contact with guild administrator.");
                    return;
                }
                case ChecksFailedException ex when ex.FailedChecks.Any(x => x is RequireUserPermissionsAttribute || x is RequireGlobalRankAttribute):
                    await e.Context.RespondAsync(await e.Context.Lang("global.noPermissions"));
                    return;
                case ChecksFailedException ex when ex.FailedChecks.Any(x => x is MusicCommand.RequireLavalinkAttribute):
                    await e.Context.RespondAsync("Bot is not connected to lavalink!");
                    return;
                default:
                    Client.DebugLogger.LogMessage(LogLevel.Error, "kuvuLogging", $"An exception occured during {e.Context.User.Username}'s invocation of '{e.Context.Command.QualifiedName}': {e.Exception.GetType()}", DateTime.Now.Date, e.Exception);
                    var globalUser = await e.Context.Member.GetGlobalUser();
                    if (globalUser.GlobalRank >= KuvuGlobalRank.Admin)
                    {
                        await e.Context.Message.RespondWithFileAsync("crash.txt",
                            new MemoryStream(Encoding.ASCII.GetBytes(e.Exception.ToString())),
                            embed: new ModernEmbedBuilder
                            {
                                Title = "Command failed",
                                ColorRGB = (231, 76, 60),
                                Description = $"```{e.Exception.ToString().Truncate(2048 - 6, "...")}```"
                            });
                    }
                    else
                    {
                        var errorId = Convert.ToBase64String(Guid.NewGuid().ToByteArray())[..^2];

                        await new ModernEmbedBuilder
                        {
                            Title = "Internal error",
                            ColorRGB = (231, 76, 60),
                            Description = $"Error id #{errorId}"
                        }.AddGeneratedForFooter(e.Context, false).Send(e.Context.Message.Channel);

                        var errorsChannel = (await Client.GetGuildAsync(257599205693063168)).GetChannel(697574699063967784);
                        await errorsChannel.SendFileAsync("crash.txt",
                            new MemoryStream(Encoding.ASCII.GetBytes(e.Exception.ToString())),
                            embed: new ModernEmbedBuilder
                            {
                                Title = $"Error in command {errorId}",
                                ColorRGB = (231, 76, 60),
                                Description = $"```{e.Exception.ToString().Truncate(2048 - 6, "...")}```",
                                Fields =
                                {
                                    ("Message", $"[Jump]({e.Context.Message.JumpLink}) `{e.Context.Message.Content}`", true),
                                    ("User", $"`{e.Context.User.Id}`", true)
                                }
                            }.AddGeneratedForFooter(e.Context, false));
                    }

                    break;
            }
        }

        private static Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            Client.DebugLogger.LogMessage(LogLevel.Debug, "kuvuLogging", $"{e.Context.User.Username} executed '{e.Command.QualifiedName}' in {e.Context.Channel.Name}.", DateTime.Now);
            return Task.CompletedTask;
        }

        private static void UpdateStatus()
        {
            if (!Loaded)
            {
                Client.UpdateStatusAsync(new DiscordActivity("Rebooting..."), UserStatus.Idle, Process.GetCurrentProcess().StartTime);
                return;
            }

            Client.UpdateStatusAsync(GetDiscordActivity(), Config.Status.UserStatus);
        }

        public static DiscordActivity GetDiscordActivity()
        {
            return new DiscordActivity(Config.Status.Activity
                .Replace("%defualtprefix%", Config.DefualtPrefix)
                .Replace("%guilds%", Client.ShardClients.Values.Sum(client => client.Guilds.Count).ToString()), Config.Status.ActivityType);
        }

        private static Task Client_GuildEvents(EventArgs e)
        {
            UpdateStatus();
            return Task.CompletedTask;
        }

        private static Task Client_Ready(ReadyEventArgs e)
        {
            Client.DebugLogger.LogMessage(LogLevel.Info, "KuvuBot", "Discord client ready!", DateTime.Now);
            return Task.CompletedTask;
        }
    }
}