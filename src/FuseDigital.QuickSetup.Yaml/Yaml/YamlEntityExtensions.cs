using Volo.Abp.Domain.Entities;

namespace FuseDigital.QuickSetup.Yaml;

public static class YamlEntityExtensions
{
    public static string GetUniqueIdentifier(this IEntity entity)
    {
        return entity.GetKeys().GetUniqueIdentifier();
    }

    public static string GetUniqueIdentifier(this object[] keys)
    {
        return string.Join('~', keys);
    }
}