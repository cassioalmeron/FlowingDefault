using FlowingDefault.Core.Dtos;
using FlowingDefault.Core.Models;

namespace FlowingDefault.Core.Extensions;

public static class EntityBaseExtension
{
    public static T CopyTo<T>(this EntityBase source)
        where T : DtoBase
    {
        var dto = Activator.CreateInstance<T>();
        source.CopyTo(dto);
        return dto;
    }

    public static void CopyTo(this EntityBase source, DtoBase target)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(source, nameof(source));
        ArgumentNullExceptionHelper.ThrowIfNull(target, nameof(target));

        var sourceType = source.GetType();
        var targetType = target.GetType();

        var sourceProperties = sourceType.GetProperties();
        var targetProperties = targetType.GetProperties();

        foreach (var sourceProperty in sourceProperties)
        {
            var targetProperty = targetProperties.SingleOrDefault(x => x.Name == sourceProperty.Name);

            if (targetProperty == null)
            {
                var name = sourceProperty.Name;
                if (sourceProperty.PropertyType.BaseType == typeof(EntityBase))
                {
                    name += "Id";
                    targetProperty = targetProperties.SingleOrDefault(x => x.Name == name);
                }
            }

            if (targetProperty == null)
                continue;

            var sourceValue = sourceProperty.GetValue(source, null);
            var entityBase = sourceValue as EntityBase;

            if (entityBase != null)
                sourceValue = entityBase.Id;

            targetProperty?.SetValue(target, sourceValue);
        }
    }
}