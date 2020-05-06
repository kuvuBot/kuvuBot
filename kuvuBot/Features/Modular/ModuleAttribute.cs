using System;
using System.Diagnostics;
using System.Reflection;

namespace kuvuBot.Features.Modular
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ModuleAttribute : Attribute
    {
        public string Name { get; }
        public Version Version { get; }
        public string DisplayVersion => Version.ToString(3);
        public string Author { get; }
        public string Description { get; }

        public ModuleAttribute(string name, string version, string author = null, string description = null)
        {
            Name = name;
            Version = new Version(version);
            Author = author;
            Description = description;
        }

        public ModuleAttribute(Type target, string description = null)
        {
            var assembly = Assembly.GetAssembly(target);
            var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            var assemblyName = assembly.GetName();
            Name = assemblyName.Name;
            Version = assemblyName.Version;
            Author = versionInfo.CompanyName;
            Description = description;
        }
    }
}
