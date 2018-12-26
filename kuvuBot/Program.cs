using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Colorful;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using kuvuBot.Commands;
using kuvuBot.Commands.General;
using kuvuBot.Commands.Moderation;
using kuvuBot.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Console = Colorful.Console;

namespace kuvuBot
{
    class Program
    {
        static DiscordClient Client { get; set; }
        public static Config Config { get; set; }
        public static CommandsNextExtension Commands { get; set; }

        public static async Task Main(string[] args)
        {
            Console.WriteLine(new Figlet().ToAscii("kuvuBot"), Color.Cyan);

            Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
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
                PrefixResolver = async (msg) => {
                    var kuvuGuild = await msg.Channel.Guild.GetKuvuGuild();
                    return CommandsNextUtilities.GetStringPrefixLength(msg, kuvuGuild.Prefix, StringComparison.CurrentCultureIgnoreCase);
                },
            });

            Commands.SetHelpFormatter<HelpFormatter>();
            Commands.RegisterCommands(Assembly.GetExecutingAssembly());

            Client.Ready += Client_Ready;
            Client.GuildCreated += Client_GuildEvents;
            Client.GuildDeleted += Client_GuildEvents;
            Client.GuildDownloadCompleted += Client_GuildEvents;
            StatisticManager.Initialize(Client);

            Client.DebugLogger.LogMessage(LogLevel.Info, "MySQL", "Checking database connection...", DateTime.Now);
            try
            {
                var botContext = new BotContext();
                if (botContext.Database.CanConnect())
                {
                    Client.DebugLogger.LogMessage(LogLevel.Info, "MySQL", "Database connection is OK", DateTime.Now);
                    Client.DebugLogger.LogMessage(LogLevel.Info, "MySQL", "Migrating database...", DateTime.Now);
                    try
                    {
                        botContext.Database.Migrate();
                        Client.DebugLogger.LogMessage(LogLevel.Info, "MySQL", "Database migration SUCCESS", DateTime.Now);
                    }
                    catch (Exception e)
                    {
                        Client.DebugLogger.LogMessage(LogLevel.Critical, "MySQL", $"Database migration error\n {e.ToString()}", DateTime.Now);
                    }
                }
                else
                {
                    Client.DebugLogger.LogMessage(LogLevel.Critical, "MySQL", $"Database error", DateTime.Now);
                }
            }
            catch (Exception e)
            {
                Client.DebugLogger.LogMessage(LogLevel.Critical, "MySQL", $"Database error\n {e.ToString()}", DateTime.Now);
            }
            await Client.ConnectAsync();

            // prevent app from quit
            await Task.Delay(-1);
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
