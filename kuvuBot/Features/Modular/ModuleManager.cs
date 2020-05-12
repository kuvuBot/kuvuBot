using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using DSharpPlus;
using System.Reflection;
using kuvuBot.Core.Features;
using Microsoft.Extensions.DependencyInjection;

namespace kuvuBot.Features.Modular
{
    public class ModuleManager : IFeature
    {
        public static List<ModuleAttribute> Modules = new List<ModuleAttribute>();

        public ModuleManager(DiscordShardedClient client, IServiceCollection services)
        {
            if (!Directory.Exists("modules"))
                Directory.CreateDirectory("modules");

            foreach (var modulePath in Directory.GetFiles("modules").Where(x => x.EndsWith(".dll")))
            {
                var moduleAssembly = Assembly.LoadFrom(modulePath);
                var name = moduleAssembly.GetName().Name;
                try
                {
                    var (moduleType, moduleAttribute) = moduleAssembly.GetTypes().Select(t => (moduleType: t, moduleAttribute: t.GetCustomAttribute<ModuleAttribute>())).FirstOrDefault(x => x.moduleAttribute != null);
                    if (moduleAttribute == null)
                    {
                        client.DebugLogger.LogMessage(LogLevel.Warning, name, $"I'm not valid module", DateTime.Now);
                        return;
                    }

                    var servicesProvider = services.BuildServiceProvider();
                    foreach (var constructor in moduleType.GetConstructors())
                    {
                        var parameters = constructor.GetParameters().Select(x => servicesProvider.GetService(x.ParameterType)).ToArray();
                        var feature = constructor.Invoke(parameters);
                        services.AddSingleton(moduleType, feature);
                        break;
                    }

                    foreach (var extension in Program.Commands.Values)
                    {
                        extension.RegisterCommands(moduleAssembly);
                    }
                    services.RegisterFeatures(moduleAssembly);

                    client.DebugLogger.LogMessage(LogLevel.Info, name, $"Loaded ({moduleAttribute.DisplayVersion})", DateTime.Now);
                    Modules.Add(moduleAttribute);
                }
                catch (Exception e)
                {
                    client.DebugLogger.LogMessage(LogLevel.Error, name, $"Error\n{e}", DateTime.Now);
                }
            }
        }
    }
}
