using System;
using CommandLine;

namespace FuseDigital.QuickSetup.UserFiles.Dto;

[Verb("add", HelpText = "Adds a file or folder pattern to be included, tracked, and pushed to the remote repository.")]
public class AddOptions : QupCommandOptions
{
    [Value(0, MetaName = "pattern", Required = true, HelpText = "The file/folder pattern to be included.")]
    public string Pattern { get; set; }

    public override Type GetCommandType()
    {
        return typeof(AddCommand);
    }
}