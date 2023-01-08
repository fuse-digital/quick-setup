using System;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace FuseDigital.QuickSetup.LinkManagers.Dto;

[Verb("link", HelpText = "Create symbolic links between files or folders")]
public class LinkOptions : QupCommandOptions
{
    [Value(0, MetaName = "source", Required = false, HelpText = "The source path of the file or directory")]
    public string Source { get; set; }
    
    [Value(1, MetaName = "target", Required = false, HelpText = "The target path of the new link")]
    public string Target { get; set; }
    
    public override Type GetCommandType()
    {
        return typeof(LinkCommand);
    }
    
    [Usage(ApplicationAlias = ApplicationAlias)]
    public static IEnumerable<Example> Examples
    {
        get
        {
            yield return new Example("Create all the links for current operating system.",
                new LinkOptions());

            yield return new Example("Create and track a new link.",
                new LinkOptions {Source = "~/.qup/host", Target = "/etc/host"});
        }
    }
}