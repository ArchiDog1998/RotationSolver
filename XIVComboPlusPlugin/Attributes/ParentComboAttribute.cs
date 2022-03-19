using System;
using XIVComboPlus;

namespace XIVComboPlus.Attributes;

[AttributeUsage(AttributeTargets.Field)]
internal class ParentComboAttribute : Attribute
{
    public CustomComboPreset ParentPreset { get; }

    internal ParentComboAttribute(CustomComboPreset parentPreset)
    {
        ParentPreset = parentPreset;
    }
}
