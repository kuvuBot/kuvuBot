using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;

namespace kuvuBot.Panel
{
    public static class DiscordExtensions
    {
        public static async Task<string> GetIconUrl(this DiscordConnection discordConnection)
        {
            switch (discordConnection.Type)
            {
                case "steam":
                    return "https://discordapp.com/assets/f09c1c70a67ceaaeb455d163f3f9cbb8.png";
                case "twitch":
                    return "https://discordapp.com/assets/edbbf6107b2cd4334d582b26e1ac786d.png";
                case "twitter":
                    return "https://discordapp.com/assets/4662875160dc4c56954003ebda995414.png";
                case "reddit":
                    return "https://discordapp.com/assets/3abe9ce5a00cc24bd8aae04bf5968f4c.png";
                case "facebook":
                    return "https://discordapp.com//assets/8d8f815f3d81a33b1e70ec7c22e1b6fe.png";
                case "spotify":
                    return "https://discordapp.com/assets/f0655521c19c08c4ea4e508044ec7d8c.png";
                case "leagueoflegends":
                    return "https://discordapp.com/assets/806953fe1cc616477175cbcdf90d5cd3.png";
                case "youtube":
                    return "https://discordapp.com/assets/ff3516ac66b71ef616b1df63e20fee65.png";
                case "skype":
                    return "https://discordapp.com/assets/5be6cc17e596c02e7506f2776cfb1622.png";
                case "xbox":
                    return "https://discordapp.com/assets/0d44ba28e39303de3832db580a252456.png";
                default:
                    return "";
            }
        }

        static readonly Dictionary<string, DiscordRestClient> Clients = new Dictionary<string, DiscordRestClient>();
        public static async Task<DiscordRestClient> GetRestClient(this HttpContext httpContext)
        {
            var token = await httpContext.GetTokenAsync("access_token");

            if (Clients.ContainsKey(token))
            {
                return Clients[token];
            }
            else
            {
                var client = new DiscordRestClient(new DiscordConfiguration
                {
                    TokenType = TokenType.Bearer,
                    Token = token,
                });
                await client.InitializeCacheAsync();

                Clients[token] = client;
                return Clients[token];
            }
        }
    }
}
