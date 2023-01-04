namespace FuseDigital.QuickSetup;

public class QuickSetupOptions
{
    public string UserProfile { get; set; } = "~";

    public string BaseDirectory { get; set; } = ".qup";

    public string LogDirectory { get; set; } = "logs";

    private const char DirectorySeparatorChar = '/';

    private const string UserProfileChar = "~";

    public string GetAbsolutePath(string path)
    {
        return (path.Equals(".") ? Environment.CurrentDirectory : path)
            .Replace(UserProfileChar, UserProfile)
            .Replace(DirectorySeparatorChar, Path.DirectorySeparatorChar);
    }

    public string GetRelativePath(string path)
    {
        return (path.Equals(".") ? Environment.CurrentDirectory : path)
            .Replace(UserProfile, UserProfileChar)
            .Replace(Path.DirectorySeparatorChar, DirectorySeparatorChar);
    }
}