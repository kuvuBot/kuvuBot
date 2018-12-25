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
using kuvuBot.Data;
using Newtonsoft.Json;
using Console = Colorful.Console;

namespace kuvuBot
{
    class Program
    {
        static DiscordClient Client { get; set; }
        static CommandsNextExtension Commands { get; set; }

        public static async Task Main(string[] args)
        {
            Console.WriteLine(new Figlet().ToAscii("kuvuBot"), Color.Cyan);

            var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
            var conf = new DiscordConfiguration
            {
                Token = config.Token,
                TokenType = TokenType.Bot,

                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            };

            Client = new DiscordClient(conf);

            Commands = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefixes = new List<string>() { "kf!" },
                EnableDefaultHelp = true,
            });

            Commands.RegisterCommands(Assembly.GetExecutingAssembly());

            Client.Ready += Client_Ready;
            Client.GuildCreated += Client_GuildEvents;
            Client.GuildDeleted += Client_GuildEvents;
            Client.GuildDownloadCompleted += Client_GuildEvents;

            await Client.ConnectAsync();

            // prevent app from quit
            await Task.Delay(-1);
        }

        private static void UpdateStatus()
        {
            Client.UpdateStatusAsync(new DiscordActivity($"kb!help | {Client.Guilds.Count} guilds", DSharpPlus.Entities.ActivityType.ListeningTo),
                DSharpPlus.Entities.UserStatus.Online);
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
