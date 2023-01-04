using System;
using CommandLine;

namespace FuseDigital.QuickSetup.UserFiles.Dto;

[Verb("sync", HelpText = "Keep your files up to date and synchronized across multiple machines.")]
public class SynchroniseOptions : QupCommandOptions
{
    public override Type GetCommandType()
    {
        return typeof(SynchroniseCommand);
    }
}