using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;

namespace kuvuBot
{
    class Config
    {
        [JsonProperty("token")] public string Token { get; set; }
    }

    class Program
    {
        static DiscordClient Client { get; set; }
        static async void Main(string[] args)
        {
            var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
            var cfg = new DiscordConfiguration
            {
                Token = config.Token,
                TokenType = TokenType.Bot,

                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            };

            Client = new DiscordClient(cfg);

            Client.Ready += Client_Ready;

            await Client.ConnectAsync();

            // prevent app from quit
            await Task.Delay(-1);
        }

        private static Task Client_Ready(ReadyEventArgs e)
        {
            Console.WriteLine("Discord bot is ready!");
            return Task.CompletedTask;
        }
    }
}
