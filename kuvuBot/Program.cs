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

namespace kuvuBot
{
    public class Program
    {
        public static DiscordClient Client { get; set; }
        public static Config Config { get; set; }
        public static CommandsNextExtension Commands { get; set; }

        public static Config LoadConfig()
        {
            return Config ?? (Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json")));
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
            if (args.Contains("--migrate"))
            {
                Console.WriteLine(new Figlet().ToAscii("Migration tool mode"), Color.Red);

                if (File.Exists("config.migrate.json"))
                    Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.migrate.json"));

                UpdateDatabase();
                var legacyDb = LegacyGuild.FromJson(File.ReadAllText(args[1]));
                var botContext = new BotContext();

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
            Commands.RegisterCommands(Assembly.GetExecutingAssembly());

            Client.Ready += Client_Ready;
            Client.GuildCreated += Client_GuildEvents;
            Client.GuildDeleted += Client_GuildEvents;
            Client.GuildDownloadCompleted += Client_GuildEvents;
            Client.MessageReactionAdded += MinesweeperCommand.Client_MessageReactionAdded;

            IFeatureManager[] managers = { new StatisticManager(), new LogManager(), new LevelManager() };
            foreach (var manager in managers)
            {
                manager.Initialize(Client);
            }

            UpdateDatabase();


            await Client.ConnectAsync();


            // prevent app from quit
            await Task.Delay(-1);
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
