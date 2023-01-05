using System;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace FuseDigital.QuickSetup.PackageManagers.Dto;

[Verb("install", HelpText = "Install a specific package, packages for the specified package manager, or all package managers if none is specified.")]
public class InstallOptions : QupCommandOptions
{
    [Value(0, MetaName = "package-manager", Required = false, HelpText = "The package manager to install packages for (e.g., pip, npm).")]
    public string PackageManager { get; set; }

    [Value(1, MetaName = "package", Required = false, HelpText = "The package to install and add to the package manager repository.")]
    public IEnumerable<string> Package { get; set; }

    public override Type GetCommandType()
    {
        return typeof(InstallCommand);
    }

    [Usage(ApplicationAlias = ApplicationAlias)]
    public static IEnumerable<Example> Examples
    {
        get
        {
            yield return new Example("Install packages for all package managers",
                new InstallOptions());

            yield return new Example("Install packages for a specific package manager",
                new InstallOptions {PackageManager = "apt"});

            yield return new Example("Install and track a specific package.",
                new InstallOptions {PackageManager = "apt", Package = new[] {"curl"}});
        }
    }
}