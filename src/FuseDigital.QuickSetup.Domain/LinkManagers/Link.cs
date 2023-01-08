using System.ComponentModel.DataAnnotations;
using FuseDigital.QuickSetup.Entities;

namespace FuseDigital.QuickSetup.LinkManagers;

public class Link : QupEntity
{
    [Key]
    [Required]
    public string Target { get; set; }

    [Key]
    [Required]
    public string Source { get; set; }

    public Link()
    {
    }

    public Link(string source, string target)
    {
        Source = source;
        Target = target;
    }
}