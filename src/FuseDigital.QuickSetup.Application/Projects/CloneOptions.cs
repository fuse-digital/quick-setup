using System;
using System.Threading.Tasks;
using CommandLine;

namespace FuseDigital.QuickSetup.Projects;

[Verb("clone", HelpText = "The clone command allows you to download a copy of a Git source repository to your local machine. It automatically saves the repository information and sets up synchronization, so that you can easily work with the repository across multiple machines.")]
public class CloneOptions : IQupCommandOptions
{
    [Value(0, Required = true, HelpText = "The URL of the repository to clone.")]
    public string Repository { get; set; }
    
    [Value(1, Required = true, HelpText = "The directory to clone the repository into.")]
    public string LocalFolder { get; set; }

    public Type GetCommandType()
    {
        return typeof(CloneCommand);
    }
}