using System;
using System.Collections.Generic;
using System.Text;

namespace kuvuBot.Commands.Attributes
{
    /// <summary>
    /// Gives this command or group a category, which is used when listing help.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class CategoryAttribute : Attribute
    {
        /// <summary>
        /// Gets the category for this command or group.
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Gives this command or group a category, which is used when listing help.
        /// </summary>
        /// <param name="category"></param>
        public CategoryAttribute(string category)
        {
            this.Category = category;
        }
    }
}
