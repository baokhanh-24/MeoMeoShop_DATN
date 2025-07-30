using System.Reflection;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MeoMeo.API.Extensions;

public class TrimStringsFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument == null)
                continue;
            TrimStringProperties(argument);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }

    private void TrimStringProperties(object obj)
    {
        var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite && p.PropertyType == typeof(string));

        foreach (var property in properties)
        {
            if (property.GetValue(obj) is string currentValue)
            {
                property.SetValue(obj, currentValue.Trim());
            }
        }
    }
}
