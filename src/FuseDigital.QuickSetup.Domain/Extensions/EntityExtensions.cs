using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;

namespace FuseDigital.QuickSetup.Extensions;

public static class EntityExtensions
{
    public static void ValidateModel<TEntity>(this TEntity entity)
    {
        var modelValidationContext = new ValidationContext(entity, serviceProvider: null, items: null);
        var modelValidationResults = new List<ValidationResult>();
        Validator.TryValidateObject(entity, modelValidationContext, modelValidationResults, true);

        if (modelValidationResults.Count > 0)
        {
            throw new AbpValidationException(null, modelValidationResults);
        }
    }
}