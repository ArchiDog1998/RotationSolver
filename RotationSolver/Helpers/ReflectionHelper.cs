using RotationSolver.Localization;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace RotationSolver.Helpers;

internal static class ReflectionHelper
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
        if (type == null) return new MethodInfo[0];

        var methods = from method in type.GetRuntimeMethods()
                      where method.IsStatic
                      && !method.IsConstructor && method.ReturnType == typeof(bool)
                      && checks(method)
                      select method;

        return methods.Union(type.BaseType.GetStaticBoolMethodInfo(checks)).ToArray();
    }

    internal static PropertyInfo GetPropertyInfo(this Type type, string name)
    {
        if (type == null) return null;

        foreach (var item in type.GetRuntimeProperties())
        {
            if (item.Name == name && item.GetMethod is MethodInfo info
               && info.IsStatic) return item;
        }

        return type.BaseType.GetPropertyInfo(name);
    }

    internal static MethodInfo GetMethodInfo(this Type type, string name)
    {
        if (type == null) return null;

        foreach (var item in type.GetRuntimeMethods())
        {
            if (item.Name == name && item.IsStatic
                      && !item.IsConstructor && item.ReturnType == typeof(bool)) return item;
        }

        return type.BaseType.GetMethodInfo(name);
    }

    internal static string GetMemberName(this MemberInfo info)
    {
        if (LocalizationManager.RightLang.MemberInfoName.TryGetValue(info.Name, out var memberName)) return memberName;

        return info.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? info.Name;
    }

    internal static string GetMemberDescription(this MemberInfo info)
    {
        if (LocalizationManager.RightLang.MemberInfoDesc.TryGetValue(info.Name, out var memberDesc)) return memberDesc;

        return info.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty;
    }
}
