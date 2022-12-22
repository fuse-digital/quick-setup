namespace FuseDigital.QuickSetup;

public class QuickSetupOptions
{
    public string UserProfile { get; set; } = "~";

    public string BaseDirectory { get; set; } = ".qup";

    public char DirectorySeparatorChar  { get; set; } = '/';
}