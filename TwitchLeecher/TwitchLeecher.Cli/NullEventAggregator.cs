using TwitchLeecher.Shared.Events;

namespace TwitchLeecher.Cli;

internal class NullEventAggregator : IEventAggregator
{
    public TEventType GetEvent<TEventType>() where TEventType : EventBase, new()
    {
        return new TEventType();
    }
}