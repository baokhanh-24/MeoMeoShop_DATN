using System.Reflection;
using AutoMapper;

namespace MeoMeo.Contract.Extensions;

public static class AutomapperExtensions
{
    public static IMappingExpression<TSource, TDestination> IgnoreAllNonExisting<TSource, TDestination>(
        this IMappingExpression<TSource, TDestination> expression)
    {
        var sourceProperties = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var sourcePropertyNames = new HashSet<string>(sourceProperties.Select(p => p.Name));

        var destinationProperties = typeof(TDestination)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetIndexParameters().Length == 0);

        foreach (var destProp in destinationProperties)
        {
            if (!sourcePropertyNames.Contains(destProp.Name))
            {
                expression.ForMember(destProp.Name, opt => opt.Ignore());
            }
        }
        var destinationFields =
            typeof(TDestination).GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        foreach (var field in destinationFields)
        {
            expression.ForMember(field.Name, opt => opt.Ignore());
        }

        return expression;
    }

}