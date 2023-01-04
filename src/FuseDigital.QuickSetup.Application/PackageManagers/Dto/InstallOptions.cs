using System;
using System.Collections.Generic;
using CommandLine;

namespace FuseDigital.QuickSetup.PackageManagers.Dto;

[Verb("install", HelpText = "install stuff")]
public class InstallOptions : QupCommandOptions
{
    [Value(1, MetaName = "package-manager", Required = false, HelpText = "")]
    public string PackageManager { get; set; }

    [Value(1, MetaName = "package", Required = false, HelpText = "")]
    public IEnumerable<string> Package { get; set; }

    public override Type GetCommandType()
    {
        return typeof(InstallCommand);
    }
}