using DSharpPlus;

namespace kuvuBot.Features
{
    public interface IFeatureManager
    {
        void Initialize(DiscordClient client);
    }
}