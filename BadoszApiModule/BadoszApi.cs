﻿using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BadoszApiModule
{
    public static class BadoszApi
    {
        public enum BadoszEndpoint { advice, bird, blurple, cat, changemymind, chucknorris, cuddle, dadjoke, dog, endpoints, excuseme, fact, fox, hug, invert, kiss, note, orangly, pat, shibe, triggered, trump, tweet, wanted, wasted, why, yomomma }

        internal static WebClient GetWebClient()
        {
            var wc = new WebClient();
            wc.Headers.Add("Authorization", BadoszApiModule.KuvuConfig.Apis["badoszapi"]);
            return wc;
        }

        public static async Task<string> GetJson(BadoszEndpoint endpoint, NameValueCollection parameters = null)
        {
            using (var wc = GetWebClient())
            {
                var uri = new UriBuilder("https" + $"://api.badosz.com/{Enum.GetName(typeof(BadoszEndpoint), endpoint)}")
                {
                    Query = parameters?.ToString()
                };
                return await wc.DownloadStringTaskAsync(uri.Uri);
            }
        }

        public static async Task<Stream> GetStream(BadoszEndpoint endpoint, NameValueCollection parameters = null)
        {
            using (var wc = GetWebClient())
            {
                var uri = new UriBuilder("https" + $"://api.badosz.com/{Enum.GetName(typeof(BadoszEndpoint), endpoint)}")
                {
                    Query = parameters?.ToString()
                };
                return new MemoryStream(await wc.DownloadDataTaskAsync(uri.Uri));
            }
        }

        //HttpUtility.ParseQueryString(string.Empty)
        public static async Task<DiscordMessage> SendEmbedImage(CommandContext ctx, BadoszEndpoint endpoint, NameValueCollection parameters = null)
        {
            using (var stream = await GetStream(endpoint, parameters))
            {
                string str = Enum.GetName(typeof(BadoszEndpoint), endpoint);
                var embed = new ModernEmbedBuilder
                {
                    Title = char.ToUpper(str[0]) + str.Substring(1),
                    Color = new DuckColor(33, 150, 243),
                    Timestamp = DuckTimestamp.Now,
                    Footer = ($"Generated for {ctx.User.Username}#{ctx.User.Discriminator}", ctx.User.AvatarUrl),
                    ImageUrl = "attachment://image.gif"
                };
                return await ctx.RespondWithFileAsync(embed: embed.Build(), fileData: stream, fileName: $"image.gif");
            }
        }
    }
}