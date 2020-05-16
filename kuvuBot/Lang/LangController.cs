using DSharpPlus.CommandsNext;
using kuvuBot.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext.Converters;
using Newtonsoft.Json.Linq;
using Console = Colorful.Console;

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
            if (LangController.Languages.Keys.Contains(value))
            {
                return Task.FromResult(new Optional<string>(value));
            }
            else
            {
                foreach (var lang in LangController.Languages.Keys)
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
        public static Dictionary<string, JObject> Languages = new Dictionary<string, JObject>();

        public static async Task LoadTranslations()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var regex = new Regex($@"{typeof(LangController).Namespace}.(\w+).json");
            var languages = assembly.GetManifestResourceNames().Select(x => regex.Match(x)).Where(x => x.Groups.Count > 1);

            foreach (var match in languages)
            {
                var language = match.Groups[1].Value;
                await using var stream = assembly.GetManifestResourceStream(match.Value);
                if (stream != null)
                {
                    using var reader = new StreamReader(stream);
                    var json = JObject.Parse(await reader.ReadToEndAsync());
                    Languages.Add(language, json);
                    Console.WriteLine($"Loaded {language} translation");
                }
            }
        }

        public static string Get(string path, string lang)
        {
            return Languages[lang].SelectToken(path)?.ToString();
        }
    }
}