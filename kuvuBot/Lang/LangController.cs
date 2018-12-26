using DSharpPlus.CommandsNext;
using kuvuBot.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace kuvuBot.Lang
{
    public static class LangExt
    {
        public async static Task<string> Lang(this CommandContext ctx, string term)
        {
            var kuvuGuild = await ctx.Guild.GetKuvuGuild();
            var lang = kuvuGuild.Lang;
            return LangController.Get(term, lang);
        }
    }
    public class LangController
    {
        public static string Get(string term, string lang)
        {
            var path = term.Split('.');

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"kuvuBot.Lang.{lang}.json";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader sr = new StreamReader(stream))
            {
                return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(sr.ReadToEnd())[path[0]][path[1]];
            }
        }
    }
}