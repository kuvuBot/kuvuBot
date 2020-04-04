using DSharpPlus;

namespace kuvuBot.Features
{
    public interface IFeatureManager
    {
        void Initialize(DiscordShardedClient client);
    }
}