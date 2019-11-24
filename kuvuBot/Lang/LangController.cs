using DSharpPlus.CommandsNext;
using kuvuBot.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext.Converters;

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

    public class LangConverter : IArgumentConverter<string>
    {
        public Task<Optional<string>> ConvertAsync(string value, CommandContext ctx)
        {
            if (LangController.Languages.Contains(value))
            {
                return Task.FromResult(new Optional<string>(value));
            }
            else
            {
                foreach (var lang in LangController.Languages)
                {
                    var aliases = LangController.Get("lang.aliases", lang).Split("|");
                    if (aliases.Contains(value))
                    {
                        return Task.FromResult(new Optional<string>(lang));
                    }
                }
                return Task.FromResult(new Optional<string>());
            }
        }
    }

    public static class LangController
    {
        public static List<string> Languages = new List<string> { "en", "pl", "de", "fr" };

        // TODO rework this, json path maybe?
        public static string Get(string term, string lang)
        {
            // Split term to content.term
            var path = term.Split('.');

            // Get lang file from assembly
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"kuvuBot.Lang.{lang}.json";

            try
            {
                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream != null)
                {
                    using var sr = new StreamReader(stream);
                    var result = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(sr.ReadToEnd())[path[0]][path[1]];
                    return result;
                }
            }
            catch
            {
                // ignored
            }

            return null;
        }
    }
}