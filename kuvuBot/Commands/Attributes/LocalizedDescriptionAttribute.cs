using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using kuvuBot.Data;
using kuvuBot.Lang;

namespace kuvuBot.Commands.Attributes
{
    public static class LocalizedDescriptionExt
    {
        public static string LocalizedDescription(this Command command, string lang)
        {
            if (!(command.CustomAttributes.FirstOrDefault(x => x is LocalizedDescriptionAttribute) is LocalizedDescriptionAttribute desc))
                return command.Description ?? "No description provided";

            return LangController.Get(desc.Term, lang) ?? command.Description ?? "No description provided";
        }

        public static string LocalizedDescription(this Command command, KuvuGuild guild)
        {
            if (!(command.CustomAttributes.FirstOrDefault(x => x is LocalizedDescriptionAttribute) is LocalizedDescriptionAttribute desc))
                return command.Description ?? "No description provided";

            return guild.Lang(desc.Term) ?? command.Description ?? "No description provided";
        }

        public static string LocalizedDescription(this CommandArgument command, KuvuGuild guild)
        {
            if (!(command.CustomAttributes.FirstOrDefault(x => x is LocalizedDescriptionAttribute) is LocalizedDescriptionAttribute desc))
                return command.Description ?? "No description provided";

            return guild.Lang(desc.Term) ?? command.Description ?? "No description provided";
        }
    }

    /// <summary>
    /// Gives this command, group, or argument a localized description, which is used when listing help.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class LocalizedDescriptionAttribute : Attribute
    {
        /// <summary>
        /// Gets the term for this command, group, or argument.
        /// </summary>
        public string Term { get; }

        /// <summary>
        /// Gives this command, group, or argument a localized description, which is used when listing help.
        /// </summary>
        /// <param name="description"></param>
        public LocalizedDescriptionAttribute(string term)
        {
            this.Term = term;
        }
    }
}
