using System.IO;
using FuseDigital.QuickSetup.Text;
using FuseDigital.QuickSetup.UserFiles;

namespace FuseDigital.QuickSetup.Repositories;

public class UserFileRepository : TextRepository, IUserFileRepository
{
    public override string FileName => ".gitinclude";

    public override string FilePath => Path.Combine(Context.Options.UserProfile, FileName);

    public UserFileRepository(ITextContext context) : base(context)
    {
    }
}