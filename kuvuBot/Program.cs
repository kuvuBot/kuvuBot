using System;
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json.Linq;
using kuvuBot.Features.Modular;
using kuvuBot.Commands;
using DSharpPlus.CommandsNext.Exceptions;
using kuvuBot.Commands.Converters;
using DSharpPlus.CommandsNext.Attributes;
using kuvuBot.Lang;
using kuvuBot.Commands.Attributes;

namespace kuvuBot
{
    public class Program
    {
        const string ConfigFilename = "config.json";

        public static DiscordClient Client { get; set; }
        public static Config Config { get; set; }
        public static CommandsNextExtension Commands { get; set; }
        private static bool Kill { get; set; } = false;

        public static Config LoadConfig()
        {
            if (!File.Exists(ConfigFilename))
            {
                Console.WriteLine($"{ConfigFilename} not found. Generating one for you, fill it");
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(Assembly.GetExecutingAssembly().GetManifestResourceNames().FirstOrDefault(x=>x.EndsWith("config.example.json"))))
                {
                    stream.CopyTo(File.Create(ConfigFilename));
                    System.Environment.Exit(1);
                }
            }
            return Config ?? (Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigFilename)));
        }

        public static void UpdateDatabase()
        {
            Console.WriteLine("Checking database connection...");
            try
            {
                var botContext = new BotContext();
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
            if (args != null && args.Length >= 1 && args[0] == "--migrate")
            {
                Console.WriteLine(new Figlet().ToAscii("Migration tool mode"), Color.Red);
                Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.migrate.json"));

                var botContext = new BotContext();
                UpdateDatabase();

                if (File.Exists("stats.json"))
                {
                    Console.WriteLine("Migrating stats...");
                    var legacyStats = JArray.Parse((File.ReadAllText("stats.json")));

                    foreach (JToken legacyStat in legacyStats.Children<JToken>())
                    {
                        botContext.Statistics.Add(new KuvuStat()
                        {
                            Date = DateTime.Parse(legacyStat.First.ToObject<string>()),

                            Guilds = (int)legacyStat.Last.ToObject<LegacyStat>().Guilds,
                            Channels = (int)legacyStat.Last.ToObject<LegacyStat>().Channels,
                            Users = (int)legacyStat.Last.ToObject<LegacyStat>().Users,
                        });
                    }
                    Console.WriteLine("Updating database...");
                    await botContext.SaveChangesAsync();

                }
                else
                {
                    Console.WriteLine("config.migrate.json not found - skipping");
                }

                if (File.Exists("config.migrate.json") && File.Exists("out.json"))
                {

                    var legacyDb = LegacyGuild.FromJson(File.ReadAllText("out.json"));

                    foreach (var legacyGuild in legacyDb)
                    {
                        if (ulong.TryParse(legacyGuild.Id, out ulong id) == false)
                            continue;

                        var kuvuGuild = new KuvuGuild()
                        {
                            GuildId = id,
                            Lang = legacyGuild.Lang ?? "en",
                            Prefix = legacyGuild.Prefix ?? "kb!",
                            ShowLevelUp = legacyGuild.Showlvl ?? true,
                        };
                        botContext.Guilds.Add(kuvuGuild);
                        if (legacyGuild.Users.HasValue && legacyGuild.Users.Value.UserMap != null)
                            foreach (var legacyUser in legacyGuild.Users.Value.UserMap)
                            {
                                if (ulong.TryParse(legacyUser.Key, out ulong userId) == false)
                                    continue;
                                var kuvuUser = new KuvuUser()
                                {
                                    DiscordUser = userId,
                                    Exp = KuvuUser.ConvertLevelToExp((int)legacyUser.Value.Lvl),
                                    Guild = kuvuGuild
                                };
                                botContext.Users.Add(kuvuUser);
                            }
                    }

                    Console.WriteLine("Updating database...");
                    await botContext.SaveChangesAsync();
                }
                else
                {
                    Console.WriteLine("config.migrate.json or out.json not found - skipping");
                }

                Console.WriteLine("Done! :3");
                Console.ReadKey();
                return;
            }

            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            Console.WriteLine(new Figlet().ToAscii("kuvuBot"), Color.Cyan);

            LoadConfig();

            var conf = new DiscordConfiguration
            {
                Token = Config.Token,
                TokenType = TokenType.Bot,

                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true,
                HttpTimeout = TimeSpan.FromSeconds(60)
            };

            Client = new DiscordClient(conf);

            Commands = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                EnableDefaultHelp = true,
                PrefixResolver = async (msg) =>
                {
                    var kuvuGuild = await msg.Channel.Guild.GetKuvuGuild();
                    return msg.GetStringPrefixLength(kuvuGuild.Prefix, StringComparison.CurrentCultureIgnoreCase);
                },
            });
            Commands.SetHelpFormatter<HelpFormatter>();
            Commands.RegisterConverter(new FriendlyDiscordUserConverter());
            Commands.RegisterConverter(new FriendlyDiscordMemberConverter());
            Commands.RegisterConverter(new FriendlyDiscordChannelConverter());
            Commands.RegisterConverter(new FriendlyDiscordMessageConverter());

            Commands.CommandExecuted += Commands_CommandExecuted;
            Commands.CommandErrored += Commands_CommandErrored;

            Commands.RegisterCommands(Assembly.GetExecutingAssembly());

            Client.Ready += Client_Ready;
            Client.GuildCreated += Client_GuildEvents;
            Client.GuildDeleted += Client_GuildEvents;
            Client.GuildDownloadCompleted += Client_GuildEvents;
            Client.MessageReactionAdded += MinesweeperCommand.Client_MessageReactionAdded;

            IFeatureManager[] managers = { new StatisticManager(), new LogManager(), new LevelManager(), new ModuleManager() };
            foreach (var manager in managers)
            {
                manager.Initialize(Client);
            }

            UpdateDatabase();


            await Client.ConnectAsync(GetDiscordActivity(), Config.Status.UserStatus);


            // prevent app from quit
            await Task.Run(() =>
            {
                while (!Kill) {}
            });
        }

        private static async Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            if (e.Exception is CommandNotFoundException)
                return;
            if (e.Exception is ArgumentException || (e.Exception is InvalidOperationException && e.Exception.Message == "No matching subcommands were found, and this group is not executable."))
            {
                var cmd = e.Context.CommandsNext.FindCommand("help", out var args);
                var fctx = e.Context.CommandsNext.CreateFakeContext(e.Context.User, e.Context.Channel, "help", e.Context.Prefix, cmd, e.Command.Name);
                await e.Context.CommandsNext.ExecuteCommandAsync(fctx).ConfigureAwait(false);
                return;
            }
            if (e.Exception is ChecksFailedException ex)
            {
                if ((ex.FailedChecks.Any(x => x is RequireBotPermissionsAttribute)))
                {
                    var req = (RequireBotPermissionsAttribute)ex.FailedChecks.First(x => x is RequireBotPermissionsAttribute);
                    var dm = await e.Context.Member.CreateDmChannelAsync();
                    await dm.SendMessageAsync($"I don't have `{req.Permissions.ToPermissionString()}` permissions, so I can't do it! Contact with guild administrator.");
                    return;
                }
                if ((ex.FailedChecks.Any(x => x is RequireUserPermissionsAttribute || x is RequireGlobalRankAttribute)))
                {
                    await e.Context.RespondAsync(await e.Context.Lang("global.nopermission"));
                    return;
                }
            }

            Client.DebugLogger.LogMessage(LogLevel.Error, "kuvuLogging", $"An exception occured during {e.Context.User.Username}'s invocation of '{e.Context.Command.QualifiedName}': {e.Exception.GetType()}", DateTime.Now.Date, e.Exception);
        }

        private static Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            Client.DebugLogger.LogMessage(LogLevel.Debug, "kuvuLogging", $"{e.Context.User.Username} executed '{e.Command.QualifiedName}' in {e.Context.Channel.Name}.", DateTime.Now);
            return Task.CompletedTask;
        }

        private static async void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            // TODO Ekhmm, do it less brutally
            Console.WriteLine("Closing kuvuBot...", Color.Black);
            await Client.UpdateStatusAsync(new DiscordActivity("Restarting bot...", ActivityType.Watching), UserStatus.Idle);
            await Client.DisconnectAsync();
            Client.Dispose();
            Kill = true;
            Process.GetCurrentProcess().Kill();
            Environment.Exit(-1);
        }

        private static void UpdateStatus()
        {
            Client.UpdateStatusAsync(GetDiscordActivity(), Config.Status.UserStatus);
        }

        public static DiscordActivity GetDiscordActivity()
        {
            return new DiscordActivity(Config.Status.Activity
                .Replace("%defualtprefix%", Config.DefualtPrefix)
                .Replace("%guilds%", Client.Guilds.Count.ToString()), Config.Status.ActivityType);
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
