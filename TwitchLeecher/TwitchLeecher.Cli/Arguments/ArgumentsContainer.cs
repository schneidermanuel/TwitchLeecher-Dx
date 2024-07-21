namespace TwitchLeecher.Cli.Arguments;

internal class ArgumentsContainer
{
    public CliAction Action { get; set; }
    public string Term { get; set; }
    private readonly IDictionary<string, string> _parsedArguments;
    private readonly IReadOnlyCollection<string> _flags;

    public ArgumentsContainer(
        CliAction action,
        string term,
        IDictionary<string, string> parsedArguments,
        IEnumerable<string> flags)
    {
        Action = action;
        Term = term;
        _parsedArguments = parsedArguments;
        _flags = flags.ToArray();
    }

    public string? GetArg(string key)
    {
        return _parsedArguments.TryGetValue(key, out var argument) ? argument : null;
    }

    public bool HasFlag(string key)
    {
        return _flags.Contains(key);
    }
}