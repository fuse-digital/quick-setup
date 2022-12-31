using System;
using CommandLine;

namespace FuseDigital.QuickSetup.UserFiles.Dto;

[Verb("checkout",HelpText = "Clones an existing QUP remote repository and checks out the specified branch as a local repository.")]
public class CheckoutOptions : IQupCommandOptions
{
    [Value(0, Required = true, HelpText = "The URL of the remote repository to clone.")]
    public string Repository { get; set; }
    
    [Value(1, Required = true, HelpText = "The name of the branch to check out.")]
    public string Branch { get; set; }
    
    public Type GetCommandType()
    {
        return typeof(CheckoutCommand);
    }
}