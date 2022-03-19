using System;
using System.Runtime.CompilerServices;
using XIVComboPlus.Combos;

namespace XIVComboPlus.Attributes;

[AttributeUsage(AttributeTargets.Field)]
internal class CustomComboInfoAttribute : Attribute
{

    public string FancyName { get; }

    public string Description { get; }

    public byte JobID { get; }

    public string JobName { get; }

    internal CustomComboInfoAttribute(string fancyName, string description, Jobs job)
    {
        FancyName = fancyName;
        Description = description;
        JobID = (byte)job;
        JobName = job.ToString();
    }
}
