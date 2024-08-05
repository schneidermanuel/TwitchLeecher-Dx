using Ninject;
using TwitchLeecher.Cli.Arguments;
using TwitchLeecher.Cli.Handler;
using TwitchLeecher.Gui.Interfaces;
using TwitchLeecher.Gui.Modules;
using TwitchLeecher.Services.Modules;
using TwitchLeecher.Shared.Events;

namespace TwitchLeecher.Cli;

public static class CliTool
{
    public static void Main(string[] args)
    {
        var kernel = new StandardKernel();
        kernel.Bind<IEventAggregator>().To<NullEventAggregator>();
        kernel.Bind<ICliHandler>().To<SearchCliHandler>().Named(CliAction.Search.ToString());
        kernel.Bind<ICliHandler>().To<DownloadCliHandler>().Named(CliAction.Download.ToString());
        kernel.Bind<ICliHandler>().To<MetadataCliHandler>().Named(CliAction.Metadata.ToString());
        kernel.Load<ServiceModule>();
        kernel.Load<GuiModule>();
        kernel.Rebind<INavigationService>().To<NullNavigator>();
        var parser = new ArgumentParser();
        var argumentContainer = parser.ParseCliLaunchArgs(args);
        if (argumentContainer == null)
        {
            Console.WriteLine("Exiting...");
            return;
        }

        var handler = kernel.Get<ICliHandler>(argumentContainer.Action.ToString());
        handler.Handle(argumentContainer);
        Task.Delay(10).GetAwaiter().GetResult();
    }
}