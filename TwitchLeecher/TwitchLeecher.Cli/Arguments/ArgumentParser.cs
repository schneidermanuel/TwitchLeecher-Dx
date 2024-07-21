namespace TwitchLeecher.Cli.Arguments;

internal class ArgumentParser
{
    public ArgumentsContainer? ParseCliLaunchArgs(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Invalid number of Arguments");
            DisplalyHelpAndQuit();
            return null;
        }

        var searchTerm = args.Last();
        var isActionValid = Enum.TryParse(args.First(), out CliAction action);
        if (!isActionValid)
        {
            Console.WriteLine($"Invalid action '{args.First()}'");
            DisplalyHelpAndQuit();
            return null;
        }

        var parsedFlags = new List<string>();
        var parsedArgs = new Dictionary<string, string>();
        for (var i = 1; i < args.Length - 1; i++)
        {
            var value = args[i];
            if (value.StartsWith("--"))
            {
                parsedFlags.Add(value.Substring("--".Length));
                continue;
            }

            if (value.StartsWith("-"))
            {
                i++;
                if (i == args.Length - 1 || args[i].StartsWith("-"))
                {
                    Console.WriteLine($"The Argument '{value.Substring("-".Length)}' has no value");
                    DisplalyHelpAndQuit();
                    return null;
                }

                var argKey = value.Substring("-".Length);
                var argValue = args[i];
                parsedArgs.Add(argKey, argValue);
            }
        }

        var container = new ArgumentsContainer(action, searchTerm, parsedArgs, parsedFlags);
        return container;
    }

    private void DisplalyHelpAndQuit()
    {
        Console.WriteLine();
        Console.WriteLine("Usage: tlcli <action> [optional: Flags] <term>");
        Console.WriteLine("Valid actions:");
        Console.WriteLine("Search");
        Console.WriteLine("Metadata");
        Console.WriteLine("Download");
    }
}