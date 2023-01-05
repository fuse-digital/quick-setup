namespace FuseDigital.QuickSetup.Extensions;

public static class StreamReaderExtensions
{
    public static async Task<IList<string>> ReadLinesAsync(this StreamReader reader)
    {
        var lines = new List<string>();
        while (await reader.ReadLineAsync() is { } line)
        {
            lines.Add(line);
        }

        return lines;
    }
    
    public static IList<string> ReadLines(this StreamReader reader)
    {
        var lines = new List<string>();
        while (reader.ReadLine() is { } line)
        {
            lines.Add(line);
        }

        return lines;
    }
}