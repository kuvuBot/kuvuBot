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
using Newtonsoft.Json.Linq;
using kuvuBot.Features.Modular;
using kuvuBot.Commands;
using DSharpPlus.CommandsNext.Exceptions;

namespace kuvuBot
{
    public class Program
    {
        const string ConfigFilename = "config.json";

        public static DiscordClient Client { get; set; }
        public static Config Config { get; set; }
        public static CommandsNextExtension Commands { get; set; }

        public static Config LoadConfig()
        {
            if (!File.Exists(ConfigFilename))
            {
                Console.WriteLine($"{ConfigFilename} not found. Generating one for you, fill it");
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("config.example.json"))
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

                if (File.Exists("config.migrate.json") && args.Length >= 2)
                {
                    Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.migrate.json"));

                    var legacyDb = LegacyGuild.FromJson(File.ReadAllText(args[1]));

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


            await Client.ConnectAsync();


            // prevent app from quit
            await Task.Delay(-1);
        }

        private static async Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            if (e.Exception is CommandNotFoundException)
                return;
            if (e.Exception is ArgumentException)
            {
                var cmd = e.Context.CommandsNext.FindCommand("help", out var args);
                var fctx = e.Context.CommandsNext.CreateFakeContext(e.Context.User, e.Context.Channel, "help", e.Context.Prefix, cmd, e.Command.Name);
                await e.Context.CommandsNext.ExecuteCommandAsync(fctx).ConfigureAwait(false);
                return;
            }

            Client.DebugLogger.LogMessage(LogLevel.Error, "DSP Test", $"An exception occured during {e.Context.User.Username}'s invocation of '{e.Context.Command.QualifiedName}': {e.Exception.GetType()}", DateTime.Now.Date, e.Exception);
        }

        private static Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            Client.DebugLogger.LogMessage(LogLevel.Info, "DSP Test", $"{e.Context.User.Username} executed '{e.Command.QualifiedName}' in {e.Context.Channel.Name}.", DateTime.Now);
            return Task.CompletedTask;
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Console.WriteLine("Closing kuvuBot...", Color.Black);
            Client.UpdateStatusAsync(new DiscordActivity("Restarting bot...", ActivityType.Watching), UserStatus.Idle);
        }

        private static void UpdateStatus()
        {
            Client.UpdateStatusAsync(new DiscordActivity($"{Config.DefualtPrefix}help | {Client.Guilds.Count} guilds", ActivityType.ListeningTo), UserStatus.Online);
        }

        private static Task Client_GuildEvents(EventArgs e)
        {
            UpdateStatus();
            return Task.CompletedTask;
        }

        private static Task Client_Ready(ReadyEventArgs e)
        {
            Client.DebugLogger.LogMessage(LogLevel.Info, "KuvuBot", "Discord client ready!", DateTime.Now);
            UpdateStatus();
            return Task.CompletedTask;
        }
    }
}
