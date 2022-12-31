using FuseDigital.QuickSetup.Text;
using FuseDigital.QuickSetup.UserFiles;

namespace FuseDigital.QuickSetup.Repositories;

public class UserFileRepository : TextRepository, IUserFileRepository
{
    public UserFileRepository(ITextContext context) : base(context)
    {
    }
}