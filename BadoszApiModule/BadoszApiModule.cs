using System;
using System.Reflection;
using kuvuBot.Data;
using kuvuBot.Features.Modular;

namespace BadoszApiModule
{
    [Module("BadoszApi", 1.1, "js6pak, Badosz", "Adds picture commands what uses badosz api")]
    public class BadoszApiModule
    {
        public static Config KuvuConfig { get; set; }
        public BadoszApiModule(Config kuvuConfig)
        {
            KuvuConfig = kuvuConfig;
        }
    }
}
