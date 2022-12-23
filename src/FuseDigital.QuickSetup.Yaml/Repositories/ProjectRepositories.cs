using System;
using FuseDigital.QuickSetup.Projects;
using FuseDigital.QuickSetup.Yaml;

namespace FuseDigital.QuickSetup.Repositories;

public class ProjectRepository : YamlRepository<Project>, IProjectRepository
{
    public ProjectRepository(IYamlContext context) : base(context)
    {
    }

    public override Func<Project, object> SortOrder<TKey>() => project => project.RelativePath;
}