using System;
using CommandLine;

namespace FuseDigital.QuickSetup.UserFiles.Dto;

[Verb("checkout",HelpText = "Clones an existing QUP remote repository and checks out the specified branch as a local repository.")]
public class CheckoutOptions : QupCommandOptions
{
    [Value(0, MetaName = "repository", Required = true, HelpText = "The URL of the remote repository to clone.")]
    public string Repository { get; set; }
    
    [Value(1, MetaName = "branch", Required = true, HelpText = "The name of the branch to check out.")]
    public string Branch { get; set; }
    
    public override Type GetCommandType()
    {
        return typeof(CheckoutCommand);
    }
}