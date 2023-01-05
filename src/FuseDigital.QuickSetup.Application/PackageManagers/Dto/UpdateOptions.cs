using System;
using CommandLine;

namespace FuseDigital.QuickSetup.PackageManagers.Dto;

[Verb("update", HelpText = "update stuff")]
public class UpdateOptions : QupCommandOptions
{
    [Value(1, MetaName = "package-manager", Required = false, HelpText = "")]
    public string PackageManager { get; set; }

    public override Type GetCommandType()
    {
        return typeof(InstallCommand);
    }
}