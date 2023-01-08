using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace FuseDigital.QuickSetup.Entities;

public abstract class QupEntity : Entity
{
    public override object[] GetKeys()
    {
        return GetType().GetProperties()
            .Where(prop => prop.IsDefined(typeof(KeyAttribute), false))
            .Select(x => x.GetValue(this))
            .ToArray();
    }
}