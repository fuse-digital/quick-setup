using System;
using CommandLine;
using CommandLine.Text;

namespace FuseDigital.QuickSetup.Projects.Dto;

[Verb("clone",
    HelpText =
        "The clone command allows you to download a copy of a Git source repository to your local machine. " +
        "It automatically saves the repository information and sets up synchronization, so that you can easily work " +
        "with the repository across multiple machines."
)]
public class CloneOptions : QupCommandOptions
{
    [Value(0, MetaName = "repository", Required = true, HelpText = "The URL of the repository to clone.")]
    public string Repository { get; set; }

    [Value(1, MetaName = "path", Required = true,
        HelpText =
            "The local path where the repository will be cloned. This can be an absolute or relative path, and the " +
            "~ character can be used to represent the user profile folder (which is platform-agnostic)."
    )]
    public string LocalFolder { get; set; }

    public override Type GetCommandType()
    {
        return typeof(CloneCommand);
    }
}