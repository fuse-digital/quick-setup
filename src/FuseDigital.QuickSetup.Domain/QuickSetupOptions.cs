namespace FuseDigital.QuickSetup;

public class QuickSetupOptions
{
    public string UserProfile { get; set; } = "~";

    public string BaseDirectory { get; set; } = ".qup";

    public const char DirectorySeparatorChar = '/';

    public const string UserProfileChar = "~";

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