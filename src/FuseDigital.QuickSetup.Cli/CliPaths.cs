using System;
using System.IO;

namespace FuseDigital.QuickSetup.Cli;

public static class CliPaths
{
    public static string Log => Path.Combine(QupRootPath, "cli", "logs");

    private static readonly string UserProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    private static readonly string QupRootPath = Path.Combine(UserProfilePath, ".qup");
}