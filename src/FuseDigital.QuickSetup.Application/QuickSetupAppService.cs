using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using FuseDigital.QuickSetup.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;

namespace FuseDigital.QuickSetup;

public class QuickSetupAppService : ApplicationService
{
    private QuickSetupOptions Settings { get; }

    public QuickSetupAppService(IOptions<QuickSetupOptions> options)
    {
        Settings = options.Value;
    }

    public async Task RunAsync(IEnumerable<string> args)
    {
        Logger.LogInformation("Quick Setup CLI (https://github.com/fuse-digital/quick-setup)");
        Logger.LogDebug("User profile directory is set {UserProfile}", Settings.UserProfile);
        Logger.LogDebug("The base directory for QUP is set {BaseDirectory}", Settings.BaseDirectory);

        try
        {
            var parser = new CommandLine.Parser(with => with.HelpWriter = null);
            var parserResult = parser.ParseArguments(args, GetOptions());
            await parserResult.WithParsedAsync<IQupCommandOptions>(RunCommandAsync);
            await parserResult.WithNotParsedAsync(errors => DisplayHelpAsync(parserResult, errors));
        }
        catch (EntityNotFoundException exception)
        {
            Logger.LogBusinessException(exception);
        }
        catch (BusinessException exception)
        {
            Logger.LogBusinessException(exception);
        }
        catch (Exception exception)
        {
            Logger.LogException(exception);
            Logger.LogWithLevel(exception.GetLogLevel(), $"Please see the logs for more information.");
        }
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