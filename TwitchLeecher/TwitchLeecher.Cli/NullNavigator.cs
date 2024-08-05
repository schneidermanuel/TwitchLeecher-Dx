using Ninject;
using TwitchLeecher.Core.Models;
using TwitchLeecher.Gui.Interfaces;

namespace TwitchLeecher.Cli;

public class NullNavigator : INavigationService
{
    private readonly IKernel _kernel;

    public NullNavigator(IKernel kernel)
    {
        _kernel = kernel;
    }

    public void ShowAuth()
    {
    }

    public void ShowWelcome()
    {
    }

    public void ShowLoading()
    {
    }

    public void ShowSearch()
    {
    }

    public void ShowSearchResults()
    {
        var found = _kernel.Get<ISearchService>().Videos.ToArray();
        foreach (var video in found)
        {
            Console.WriteLine(video.Title);
        }
    }

    public void ShowDownload(DownloadParameters downloadParams)
    {
    }

    public void ShowDownloads()
    {
    }

    public void ShowPreferences()
    {
    }

    public void ShowInfo()
    {
    }

    public void ShowLog(TwitchVideoDownload download)
    {
    }

    public void NavigateBack()
    {
    }

    public void ShowAuthSubOnly()
    {
    }
}