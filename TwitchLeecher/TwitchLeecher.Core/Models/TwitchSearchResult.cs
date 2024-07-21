using System.Collections.Generic;
using System.Linq;

namespace TwitchLeecher.Core.Models;

public class TwitchSearchResult
{
    public TwitchSearchResult(IEnumerable<TwitchVideo> results, IEnumerable<string> logs)
    {
        Results = results.ToArray();
        Logs = logs.ToArray();
    }

    public IReadOnlyCollection<TwitchVideo> Results { get; }

    public IReadOnlyCollection<string> Logs { get; }
}