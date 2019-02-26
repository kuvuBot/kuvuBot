using DSharpPlus.CommandsNext;
using kuvuBot.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace kuvuBot.Lang
{
    public static class LangExt
    {
        public static string Lang(this KuvuGuild kuvuGuild, string term)
        {
            var lang = kuvuGuild.Lang;
            return LangController.Get(term, lang);
        }

        public static async Task<string> Lang(this DiscordGuild guild, string term)
        {
            var kuvuGuild = await guild.GetKuvuGuild();
            return kuvuGuild.Lang(term);
        }

        public static async Task<string> Lang(this CommandContext ctx, string term)
        {
            return await ctx.Guild.Lang(term);
        }
    }
    public static class LangController
    {
        // Arghh its shitty
        public static string Get(string term, string lang)
        {
            // Split term to content.term
            var path = term.Split('.');

            // Get lang file from assembly
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"kuvuBot.Lang.{lang}.json";

            try
            {
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                using (var sr = new StreamReader(stream))
                {
                    //     convert object                           context            term   translated      stream     context   term
                    var result = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(sr.ReadToEnd())[path[0]][path[1]];
                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}