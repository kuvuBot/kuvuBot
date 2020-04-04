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
        public static List<ModuleAttribute> Modules = new List<ModuleAttribute>();
        public void Initialize(DiscordShardedClient client)
        {
            if (!Directory.Exists("modules"))
                Directory.CreateDirectory("modules");

            foreach (var modulePath in Directory.GetFiles("modules").Where(x => x.EndsWith(".dll")))
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

                    var constructor = moduleType.GetConstructors().FirstOrDefault();
                    var parameters = new object[constructor.GetParameters().Length];

                    foreach (var parameter in constructor.GetParameters())
                    {
                        if (parameter.ParameterType == typeof(Data.Config))
                        {
                            parameters[^1] = Program.Config;
                        }
                        else
                        {
                            parameters[^1] = null;
                        }
                    }


                    var module = constructor.Invoke(parameters);
                    var moduleAttribute = module.GetType().GetCustomAttribute<ModuleAttribute>();

                    foreach (var extension in Program.Commands.Values)
                    {
                        extension.RegisterCommands(moduleAssembly);
                    }

                    foreach(var featureManagerType in moduleAssembly.GetTypes().Where(t=>t.GetInterfaces().Any(x => x == typeof(IFeatureManager))))
                    {
                        ((IFeatureManager)Activator.CreateInstance(featureManagerType)).Initialize(client);
                    }


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
