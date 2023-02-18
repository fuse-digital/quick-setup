using System;
using CommandLine;

namespace FuseDigital.QuickSetup.Projects.Dto;

[Verb("fetch",
    HelpText = "The fetch command clones or pulls all the repositories in the projects file in your local " +
               "environment. If a repository does not exist locally, it will be cloned from the remote repository. " +
               "If the repository already exists locally, a `git pull` will be performed to update it with the " +
               "latest changes."
)]
public class FetchOptions : QupCommandOptions
{
    public override Type GetCommandType()
    {
        return typeof(FetchCommand);
    }
}