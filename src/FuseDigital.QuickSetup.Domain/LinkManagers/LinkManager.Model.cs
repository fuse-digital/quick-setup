using System.ComponentModel.DataAnnotations;
using FuseDigital.QuickSetup.Platforms;

namespace FuseDigital.QuickSetup.LinkManagers;

public partial class LinkManager
{
    [Key]
    [Display(Description = "The target operating system")]
    public PlatformOperatingSystem RunsOn { get; set; }

    [Display(Description = "The description of the actions performed by the specified create link command.")]
    public string Description { get; set; } = "Create a symbolic link from the target to the source";

    [Required]
    [Display(Description = "The command format to create the symbolic links with ${source} and ${target} placeholders")]
    public string CreateLinkFormat { get; set; }

    [Required]
    [Display(Description = "The operating system that this link applies on")]
    public IList<Link> Links { get; set; } = new List<Link>();
}