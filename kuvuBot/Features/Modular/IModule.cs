using kuvuBot.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace kuvuBot.Features.Modular
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ModuleAttribute : Attribute
    {
        public string Name { get; }
        public double Version { get; }
        public string Author { get; }
        public string Description { get; }

        public ModuleAttribute(string name, double version, string author = null, string description = null)
        {
            Name = name;
            Version = version;
            Author = author;
            Description = description;
        }
    }
}
