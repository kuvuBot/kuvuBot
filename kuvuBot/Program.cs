using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Colorful;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using kuvuBot.Commands.Fun;
using kuvuBot.Commands.General;
using kuvuBot.Data;
using kuvuBot.Features;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Console = Colorful.Console;
using kuvuBot.Features.Modular;
using DSharpPlus.CommandsNext.Exceptions;
using kuvuBot.Commands.Converters;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using kuvuBot.Lang;
using kuvuBot.Commands.Attributes;
using kuvuBot.Commands.Music;
using System.Net.Sockets;
using System.Net.Http;
using System.Net.WebSockets;
using System.Runtime.Loader;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using kuvuBot.Commands;

namespace kuvuBot
{
    public class Program
    {
        private const string ConfigFilename = "config.json";

        public static DiscordShardedClient Client { get; set; }
        public static Config Config { get; set; }
        public static IReadOnlyDictionary<int, CommandsNextExtension> Commands { get; set; }
        public static IReadOnlyDictionary<int, LavalinkExtension> Lavalink { get; set; }
        private static bool Kill { get; set; } = false;
        private static bool Loaded { get; set; } = false;

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
                        Console.WriteLine($"Database migration error\n {e.ToString()}");
                    }
                }
                else
                {
                    Console.WriteLine($"Database error");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Database error\n {e.ToString()}");
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
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true,
                HttpTimeout = TimeSpan.FromSeconds(60)
            };

            Client = new DiscordShardedClient(conf);

            Lavalink = await Client.UseLavalinkAsync();

            Commands = await Client.UseCommandsNextAsync(new CommandsNextConfiguration
            {
                EnableDefaultHelp = true,
                PrefixResolver = async (msg) =>
                {
                    var kuvuGuild = await msg.Channel.Guild.GetKuvuGuild();
                    return msg.GetStringPrefixLength(kuvuGuild.Prefix, StringComparison.CurrentCultureIgnoreCase);
                },
            });

            foreach (var extension in Commands.Values)
            {
                extension.SetHelpFormatter<HelpFormatter>();
                extension.RegisterConverter(new FriendlyDiscordUserConverter());
                extension.RegisterConverter(new FriendlyDiscordMemberConverter());
                extension.RegisterConverter(new FriendlyDiscordChannelConverter());
                extension.RegisterConverter(new FriendlyDiscordMessageConverter());
                extension.RegisterConverter(new FriendlyBoolConverter());

                extension.CommandExecuted += Commands_CommandExecuted;
                extension.CommandErrored += Commands_CommandErrored;

                extension.RegisterCommands(Assembly.GetExecutingAssembly());
            }

            Client.ClientErrored += (e) =>
            {
                Console.WriteLine(e.Exception);
                return Task.CompletedTask;
            };
            Client.Ready += Client_Ready;
            Client.GuildCreated += Client_GuildEvents;
            Client.GuildDeleted += Client_GuildEvents;
            Client.GuildDownloadCompleted += (e) =>
            {
                Client.DebugLogger.LogMessage(LogLevel.Info, "kuvuBot", $"Guild download completed {e.Guilds.Count}", DateTime.Now);
                Loaded = true;
                return Client_GuildEvents(e);
            };
            Client.MessageReactionAdded += MinesweeperCommand.Client_MessageReactionAdded;
            Client.SocketErrored += Client_SocketErrored;

            IFeatureManager[] managers = { new StatisticManager(), new LogManager(), new LevelManager(), new ModuleManager() };
            foreach (var manager in managers)
            {
                manager.Initialize(Client);
            }

            UpdateDatabase();

            await Client.StartAsync();
            Client.Ready += e => e.Client.UpdateStatusAsync(GetDiscordActivity(), Config.Status.UserStatus);

            try
            {
                var endpoint = new ConnectionEndpoint { Hostname = Config.Lavalink.Ip, Port = Config.Lavalink.Port };
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

            Console.CancelKeyPress += async (s, e) =>
            {
                e.Cancel = true;
                await OnStop();
            };

            AppDomain.CurrentDomain.ProcessExit += async (s, e) =>
            {
                await OnStop();
            };

            AssemblyLoadContext.Default.Unloading += async ctx =>
            {
                await OnStop();
            };

            // prevent app from quit
            await Task.Run(() =>
            {
                while (!Kill) { }
            });
        }

        public static async Task OnStop()
        {
            if (Kill)
                return;

            Kill = true;
            Console.WriteLine("Stopping...");
            Loaded = false;
            await Client_GuildEvents(null);
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
                case ChecksFailedException ex when ex.FailedChecks.Any(x => x is RequireBotPermissionsAttribute):
                    {
                        var req = (RequireBotPermissionsAttribute)ex.FailedChecks.First(x => x is RequireBotPermissionsAttribute);
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
                        await new ModernEmbedBuilder
                        {
                            Title = "Command failed",
                            ColorRGB = (231, 76, 60),
                            Description = $"```{e.Exception}```"
                        }.Send(e.Context.Message.Channel);
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
                        await new ModernEmbedBuilder
                        {
                            Title = $"Error in command {errorId}",
                            ColorRGB = (231, 76, 60),
                            Description = $"```{e.Exception}```",
                            Fields =
                            {
                                ("Message", $"[Jump]({e.Context.Message.JumpLink}) `{e.Context.Message.Content}`", true),
                                ("User", $"`{e.Context.User.Id}`", true)
                            }
                        }.AddGeneratedForFooter(e.Context, false).Send(errorsChannel);
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
