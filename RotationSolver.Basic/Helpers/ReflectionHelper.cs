using System.ComponentModel;
using System.Reflection;

namespace RotationSolver.Basic.Helpers;

public static class ReflectionHelper
{
    internal static PropertyInfo[] GetStaticProperties<T>(this Type type)
    {
        if (type == null) return new PropertyInfo[0];

        var props = from prop in type.GetRuntimeProperties()
                    where typeof(T).IsAssignableFrom(prop.PropertyType)
                            && prop.GetMethod is MethodInfo info
                            && info.IsStatic
                    select prop;

        return props.Union(type.BaseType.GetStaticProperties<T>()).ToArray();
    }

    internal static MethodInfo[] GetStaticBoolMethodInfo(this Type type, Func<MethodInfo, bool> checks)
    {
        return type.GetAllMethodInfo().Where(m => checks(m) && m.ReturnType == typeof(bool) && m.IsStatic).ToArray();
    }

    public static IEnumerable<MethodInfo> GetAllMethodInfo(this Type type)
    {
        if (type == null) return new MethodInfo[0];

        var methods = from method in type.GetRuntimeMethods()
                      where !method.IsConstructor
                      select method;

        return methods.Union(type.BaseType.GetAllMethodInfo());
    }

    public static PropertyInfo GetPropertyInfo(this Type type, string name)
    {
        if (type == null) return null;

        foreach (var item in type.GetRuntimeProperties())
        {
            if (item.Name == name && item.GetMethod is MethodInfo info
               && info.IsStatic) return item;
        }

        return type.BaseType.GetPropertyInfo(name);
    }

    public static MethodInfo GetMethodInfo(this Type type, string name)
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
