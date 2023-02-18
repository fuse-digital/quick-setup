using System.Collections.Generic;
using System.Threading.Tasks;
using FuseDigital.QuickSetup.Platforms;
using FuseDigital.QuickSetup.VersionControlSystems;
using Volo.Abp.DependencyInjection;

namespace FuseDigital.QuickSetup.Projects;

public class FetchCommand : QupCommandAsync, ITransientDependency
{
    private readonly IProjectRepository _repository;
    private readonly IVersionControlDomainService _sourceControl;
    private readonly IConsoleService _console;

    public FetchCommand(IProjectRepository repository,
        IVersionControlDomainService sourceControl,
        IConsoleService console)
    {
        _repository = repository;
        _sourceControl = sourceControl;
        _console = console;
    }

    public override async Task ExecuteAsync(IQupCommandOptions options)
    {
        await base.ExecuteAsync(options);

        await _console.WriteTitleAsync("fetch");

        var projects = await _repository.GetListAsync();
        for (var index = 0; index < projects.Count; index++)
        {
            var project = projects[index];

            await _console.WriteLineAsync();
            await _console.WriteLineAsync("[{0}/{1}] - Fetching {2}", index + 1, projects.Count, project.RelativePath);

            project.Fetch(_sourceControl);
        }
    }
}