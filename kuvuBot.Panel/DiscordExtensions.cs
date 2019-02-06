using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;

namespace kuvuBot.Panel
{
    public static class DiscordExtensions
    {
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
