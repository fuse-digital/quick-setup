using System;
using System.Threading.Tasks;
using Crayon;
using Figgle.Fonts;
using FuseDigital.QuickSetup.Platforms;
using static Crayon.Output;

namespace FuseDigital.QuickSetup.Cli;

public class QuickSetupConsoleService : IConsoleService
{
    public Task WriteTitleAsync(string format, params object[] arg)
    {
        var header = FiggleFonts.Univers.Render(string.Format(format, arg));
        Console.WriteLine(PrimaryColour().Text(header));
        return Task.CompletedTask;
    }

    public Task WriteHeadingAsync(string format, params object[] arg)
    {
        var message = string.Format(format, arg);
        Console.WriteLine(PrimaryColour().Bold(message));
        Console.WriteLine();
        return Task.CompletedTask;
    }

    public Task WriteLineAsync(string format = default, params object[] arg)
    {
        if (format != null)
        {
            Console.WriteLine(format, arg);
        }

        Console.WriteLine();

        return Task.CompletedTask;
    }

    private IOutput PrimaryColour()
    {
        return Rgb(237, 50, 102);
    }
}