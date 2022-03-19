using Microsoft.CodeAnalysis;
using System;
using System.Runtime.CompilerServices;

namespace XIVComboPlus.Attributes;

[CompilerGenerated]
[Embedded]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
internal sealed class NullableContextAttribute : Attribute
{
    public readonly byte Flag;

    public NullableContextAttribute(byte P_0)
    {
        Flag = P_0;
    }
}
