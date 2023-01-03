using FuseDigital.QuickSetup.Platforms.Dto;
using Volo.Abp.Domain.Services;

namespace FuseDigital.QuickSetup.Platforms;

public interface IShellDomainService : IDomainService
{
    Task<RunProgramResult> RunProcessAsync(params string[] arguments);
}