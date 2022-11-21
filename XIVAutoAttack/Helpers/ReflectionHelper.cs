using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XIVAutoAttack.Actions.BaseAction;

namespace XIVAutoAttack.Helpers;

internal static class ReflectionHelper
{
    internal class ReflectableMemberAttribute : Attribute
    {

    }
    internal static PropertyInfo[] GetStaticProperties<T>(this Type type)
    {
        if (type == null) return new PropertyInfo[0];

        var props = from prop in type.GetRuntimeProperties()
                    where typeof(T).IsAssignableFrom(prop.PropertyType)
                            && prop.GetMethod is MethodInfo info
                            && (info.IsStatic || info.GetCustomAttribute<ReflectableMemberAttribute>() != null)
                    select prop;

        return props.Union(GetStaticProperties<T>(type.BaseType)).ToArray();
    }

    internal static MethodInfo[] GetStaticBoolMethodInfo(this Type type, Func<MethodInfo, bool> checks)
    {
        if (type == null) return new MethodInfo[0];

        var methods = from method in type.GetRuntimeMethods()
                      where (method.IsStatic || method.GetCustomAttribute<ReflectableMemberAttribute>() != null)
                      && !method.IsConstructor && method.ReturnType == typeof(bool)
                      && checks(method)
                      select method;

        return methods.Union(GetStaticBoolMethodInfo(type.BaseType, checks)).ToArray();
    }

    internal static string GetMemberName(this MemberInfo info)
    {
        return info.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName 
            ?? info.Name;
    }
    internal static string GetMemberDescription(this MemberInfo info)
    {
        return info.GetCustomAttribute<DescriptionAttribute>()?.Description
            ?? string.Empty;
    }
}
