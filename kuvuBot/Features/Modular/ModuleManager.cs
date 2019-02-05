using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using DSharpPlus;
using System.Reflection;
using System.Linq.Expressions;

namespace kuvuBot.Features.Modular
{
    public class ModuleManager : IFeatureManager
    {
        public void Initialize(DiscordClient client)
        {
            if (!Directory.Exists("modules"))
                Directory.CreateDirectory("modules");

            foreach (var modulePath in Directory.GetFiles("modules").Where(x=>x.EndsWith(".dll")))
            {
                var moduleAssembly = Assembly.LoadFrom(modulePath);
                var name = moduleAssembly.GetName().Name;
                try
                {
                    var moduleType = moduleAssembly.GetTypes().FirstOrDefault(t => t.GetCustomAttributes(true).Any(x => x.GetType() == typeof(ModuleAttribute)));
                    if (moduleType == null)
                    {
                        client.DebugLogger.LogMessage(LogLevel.Warning, name, $"I'm not valid module", DateTime.Now);
                        return;
                    }

                    var module = Activator.CreateInstance(moduleType, new object[] { Program.Config });
                    var moduleAttribute = module.GetType().GetCustomAttribute<ModuleAttribute>();

                    

                    Program.Commands.RegisterCommands(moduleAssembly);

                    client.DebugLogger.LogMessage(LogLevel.Info, name, $"Loaded ({moduleAttribute.Version})", DateTime.Now);
                }
                catch (Exception e)
                {
                    client.DebugLogger.LogMessage(LogLevel.Error, name, $"Error\n{e.ToString()}", DateTime.Now);
                }
            }
        }
    }
}
