using System;
using CommandLine;

namespace FuseDigital.QuickSetup.UserFiles.Dto;

[Verb("init", HelpText = "Initializes a new local git repository and pushes it to a remote repository.")]
public class InitialiseOptions : QupCommandOptions
{
    [Value(0, MetaName = $"repository", Required = true, HelpText = "The URL of the empty remote repository.")]
    public string Repository { get; set; }
    
    [Value(1, MetaName = "default-branch-name", Required = true, HelpText = "The name of the default branch for the repository.")]
    public string DefaultBranchName { get; set; }

    public override Type GetCommandType()
    {
        return typeof(InitialiseCommand);
    }
}