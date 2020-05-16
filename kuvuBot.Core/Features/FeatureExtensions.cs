using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace kuvuBot.Core.Features
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class PriorityAttribute : Attribute
    {
        public int Priority { get; }

        public PriorityAttribute(int priority)
        {
            Priority = priority;
        }
    }

    public static class FeatureExtensions
    {
        public static void RegisterFeatures(this IServiceCollection services)
        {
            services.RegisterFeatures(Assembly.GetCallingAssembly());
        }

        public static void RegisterFeatures(this IServiceCollection services, Assembly assembly)
        {
            var serviceProvider = services.BuildServiceProvider();
            foreach (var type in assembly.GetTypes().OrderByDescending(x => x.GetCustomAttribute<PriorityAttribute>()?.Priority ?? 0))
            {
                if (typeof(IFeature).IsAssignableFrom(type))
                {
                    foreach (var constructor in type.GetConstructors())
                    {
                        var parameters = constructor.GetParameters().Select(x => serviceProvider.GetService(x.ParameterType)).ToArray();
                        var feature = (IFeature)constructor.Invoke(parameters);
                        services.AddSingleton(type, feature);
                        break;
                    }
                }
            }
        }
    }
}
