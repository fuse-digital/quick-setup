using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.Application.Services;

namespace FuseDigital.QuickSetup;

public class QuickSetupAppService : ApplicationService
{
    private QuickSetupOptions _options;

    public QuickSetupAppService(IOptions<QuickSetupOptions> options)
    {
        _options = options.Value;
    }

    public async Task RunAsync(IEnumerable<string> args)
    {
        Logger.LogInformation("Quick Setup CLI (https://github.com/fuse-digital/quick-setup)");
        Logger.LogInformation("User profile directory is set {UserProfile}", _options.UserProfile);
        Logger.LogInformation("The base directory for QUP is set {BaseDirectory}", _options.BaseDirectory);
        
        var parser = new CommandLine.Parser(with => with.HelpWriter = null);
        var parserResult = parser.ParseArguments(args, GetOptions());
        await parserResult.WithParsedAsync<IQupCommandOptions>(RunCommandAsync);
        await parserResult.WithNotParsedAsync(errors => DisplayHelpAsync(parserResult, errors));
    }

    private Task DisplayHelpAsync<T>(ParserResult<T> result, IEnumerable<Error> errors)
    {
        var helpText = HelpText.AutoBuild(result, h =>
        {
            h.AdditionalNewLineAfterOption = true;
            h.Heading = "QuickSetup (QUP)";
            h.Copyright = $"Copyright (c) {DateTime.Now.Year} by Fuse Digital (PTY) Limited";
            return HelpText.DefaultParsingErrorsHandler(result, h);
        }, e => e);
        Console.WriteLine(helpText);

        return Task.CompletedTask;
    }

    private async Task RunCommandAsync(IQupCommandOptions options)
    {
        var command = (IQupCommandAsync) LazyServiceProvider.LazyGetRequiredService(options.GetCommandType());
        await command.ExecuteAsync(options);
    }

    private static Type[] GetOptions()
    {
        return Assembly.GetExecutingAssembly().GetTypes()
            .Where(t =>
                t.GetInterfaces().Any(i => i.IsAssignableFrom(typeof(IQupCommandOptions)))
                && t.GetCustomAttribute<VerbAttribute>() != null)
            .ToArray();
    }
}