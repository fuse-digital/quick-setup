using System;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace FuseDigital.QuickSetup.PackageManagers.Dto;

[Verb("update", HelpText = "Updates all packages for the specified package manager, or all package managers if none is specified.")]
public class UpdateOptions : QupCommandOptions
{
    [Value(0, MetaName = "package-manager", Required = false, HelpText = "The package manager to update packages for (e.g., pip, npm).")]
    public string PackageManager { get; set; }

    public override Type GetCommandType()
    {
        return typeof(InstallCommand);
    }
    
    [Usage(ApplicationAlias = ApplicationAlias)]
    public static IEnumerable<Example> Examples
    {
        get
        {
            yield return new Example("Update packages for all package managers",
                new UpdateOptions());

            yield return new Example("Update packages for a specific package manager",
                new UpdateOptions {PackageManager = "apt"});
        }
    }
}