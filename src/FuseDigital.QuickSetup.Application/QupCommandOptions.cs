using System;
using CommandLine;
using Microsoft.Extensions.Logging;

namespace FuseDigital.QuickSetup;

public abstract class QupCommandOptions : IQupCommandOptions
{
    [Option(longName: "verbosity", shortName: 'v', Default = LogLevel.Warning,
        HelpText = "Set the verbosity level. Allowed values are Trace, Debug, Information and Warning")]
    public LogLevel Verbosity { get; set; }

    public abstract Type GetCommandType();

    protected const string ApplicationAlias = "qup";
}