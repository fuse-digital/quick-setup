using System;
using CommandLine;

namespace FuseDigital.QuickSetup.UserFiles;

[Verb("init", HelpText = "Initializes a new local git repository and pushes it to a remote repository.")]
public class InitialiseOptions : IQupCommandOptions
{
    [Value(0, Required = true, HelpText = "The URL of the empty remote repository.")]
    public string Repository { get; set; }
    
    [Value(1, Required = true, HelpText = "The name of the default branch for the repository.")]
    public string DefaultBranchName { get; set; }

    public Type GetCommandType()
    {
        return typeof(InitialiseCommand);
    }
}