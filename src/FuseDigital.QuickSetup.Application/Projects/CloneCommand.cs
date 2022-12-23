using System;
using System.Threading.Tasks;
using FuseDigital.QuickSetup.VersionControlSystems;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace FuseDigital.QuickSetup.Projects;

public class CloneCommand : QupCommandAsync, ITransientDependency
{
    private readonly QuickSetupOptions _options;
    private readonly IProjectRepository _repository;
    private readonly IVersionControlDomainService _sourceControl;

    public CloneCommand(IOptions<QuickSetupOptions> options,
        IProjectRepository repository,
        IVersionControlDomainService sourceControl)
    {
        _options = options.Value;
        _repository = repository;
        _sourceControl = sourceControl;
    }

    public override async Task ExecuteAsync(IQupCommandOptions options)
    {
        var cloneOptions = (CloneOptions)options;
        var project = new Project
        {
            Repository = cloneOptions.Repository,
            RelativePath = _options.GetRelativePath(cloneOptions.LocalFolder),
        };

        var exist = await _repository.FindAsync(src => src.RelativePath.Equals(project.RelativePath));
        if (exist != null)
        {
            const string message = "The working directory {0} is already being tracked";
            Logger.LogInformation(message, project.RelativePath);
            Console.WriteLine(message, project.RelativePath);
            return;
        }

        project.Clone(_sourceControl);
        await _repository.InsertAsync(project);
    }
}