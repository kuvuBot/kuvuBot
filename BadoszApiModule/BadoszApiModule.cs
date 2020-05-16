using kuvuBot.Data;
using kuvuBot.Features.Modular;

namespace BadoszApiModule
{
    [Module(typeof(BadoszApiModule), "Adds picture commands which uses badosz api")]
    public class BadoszApiModule
    {
        public static BadoszApi BadoszApi { get; internal set; }

        public BadoszApiModule(Config kuvuConfig)
        {
            BadoszApi = new BadoszApi(kuvuConfig.Apis["badoszapi"]);
        }
    }
}
