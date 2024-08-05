using TwitchLeecher.Cli.Arguments;

namespace TwitchLeecher.Cli.Handler;

internal interface ICliHandler
{
    public void Handle(ArgumentsContainer container);
}