using TwitchLeecher.Cli.Arguments;
using TwitchLeecher.Core.Enums;
using TwitchLeecher.Core.Models;
using TwitchLeecher.Gui.Interfaces;

namespace TwitchLeecher.Cli.Handler;

internal class SearchCliHandler : ICliHandler
{
    private readonly ISearchService _searchService;

    public SearchCliHandler(ISearchService searchService)
    {
        _searchService = searchService;
    }

    public void Handle(ArgumentsContainer container)
    {
        var parameters = new SearchParameters(SearchType.Channel);
        parameters.Channel = container.Term;
        parameters.LoadLimitType = LoadLimitType.Timespan;
        parameters.LoadFrom = DateTime.Now.AddDays(-10);
        parameters.LoadTo = DateTime.Now;
        _searchService.PerformSearch(parameters);
    }
}