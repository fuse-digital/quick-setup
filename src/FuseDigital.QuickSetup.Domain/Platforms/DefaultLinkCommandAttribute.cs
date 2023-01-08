namespace FuseDigital.QuickSetup.Platforms;

public class DefaultLinkCommandAttribute : Attribute
{
    public string Format { get; set; }

    public DefaultLinkCommandAttribute(string format)
    {
        Format = format;
    }
}