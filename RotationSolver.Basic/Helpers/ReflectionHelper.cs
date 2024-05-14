namespace RotationSolver.Basic.Helpers;

internal static class ReflectionHelper
{
    internal static PropertyInfo[] GetStaticProperties<T>(this Type? type)
    {
        if (type == null) return [];

        var props = from prop in type.GetRuntimeProperties()
                    where typeof(T).IsAssignableFrom(prop.PropertyType)
                            && prop.GetMethod is MethodInfo info
                            && info.IsPublic && info.IsStatic
                            && info.GetCustomAttribute<ObsoleteAttribute>() == null
                    select prop;

        return props.Union(type.BaseType.GetStaticProperties<T>()).ToArray();
    }

    internal static IEnumerable<MethodInfo> GetAllMethodInfo(this Type? type)
    {
        if (type == null) return [];

        var methods = from method in type.GetRuntimeMethods()
                      where !method.IsConstructor
                      select method;

        return methods.Union(type.BaseType.GetAllMethodInfo());
    }

    internal static PropertyInfo? GetPropertyInfo(this Type type, string name)
    {
        if (type == null) return null;

        foreach (var item in type.GetRuntimeProperties())
        {
            if (item.Name == name && item.GetMethod is MethodInfo info
               && info.IsStatic) return item;
        }

        return type.BaseType?.GetPropertyInfo(name);
    }

    internal static MethodInfo? GetMethodInfo(this Type? type, string name)
    {
        if (type == null) return null;

        foreach (var item in type.GetRuntimeMethods())
        {
            if (item.Name == name && item.IsStatic
                      && !item.IsConstructor && item.ReturnType == typeof(bool)) return item;
        }

        return type.BaseType.GetMethodInfo(name);
    }
}
