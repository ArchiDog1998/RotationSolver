using RotationSolver.Data;
using System.Collections.Generic;

namespace RotationSolver.Configuration.RotationConfig;

internal abstract class RotationConfigBase : IRotationConfig
{
    public string Name { get; }
    public string DefaultValue { get; }
    public string DisplayName { get; }

    public RotationConfigBase(string name, string value, string displayName)
    {
        Name = name;
        DefaultValue = value;
        DisplayName = displayName;
    }

    public string GetValue(ClassJobID job, string rotationName)
    {
        if (!Service.Configuration.RotationsConfigurations.TryGetValue((uint)job, out var jobDict)) return DefaultValue;
        if (!jobDict.TryGetValue(rotationName, out var configDict)) return DefaultValue;
        if (!configDict.TryGetValue(Name, out var config)) return DefaultValue;
        return config;
    }

    public virtual string GetDisplayValue(ClassJobID job, string rotationName) => GetValue(job, rotationName);

    public void SetValue(ClassJobID job, string rotationName, string value)
    {
        if (!Service.Configuration.RotationsConfigurations.TryGetValue((uint)job, out var jobDict))
        {
            jobDict = Service.Configuration.RotationsConfigurations[(uint)job] = new Dictionary<string, Dictionary<string, string>>();
        }

        if (!jobDict.TryGetValue(rotationName, out var configDict))
        {
            configDict = jobDict[rotationName] = new Dictionary<string, string>();
        }

        configDict[Name] = value;
    }

    public abstract void Draw(RotationConfigSet set, bool canAddButton);

    public virtual bool DoCommand(IRotationConfigSet set, string str) => str.StartsWith(Name);
}
