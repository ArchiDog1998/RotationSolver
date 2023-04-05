namespace RotationSolver.Basic.Configuration.RotationConfig;

public abstract class RotationConfigBase : IRotationConfig
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
        if (!Service.Config.RotationsConfigurations.TryGetValue((uint)job, out var jobDict)) return DefaultValue;
        if (!jobDict.TryGetValue(rotationName, out var configDict)) return DefaultValue;
        if (!configDict.TryGetValue(Name, out var config)) return DefaultValue;
        return config;
    }

    public virtual string GetDisplayValue(ClassJobID job, string rotationName) => GetValue(job, rotationName);

    public void SetValue(ClassJobID job, string rotationName, string value)
    {
        if (!Service.Config.RotationsConfigurations.TryGetValue((uint)job, out var jobDict))
        {
            jobDict = Service.Config.RotationsConfigurations[(uint)job] = new Dictionary<string, Dictionary<string, string>>();
        }

        if (!jobDict.TryGetValue(rotationName, out var configDict))
        {
            configDict = jobDict[rotationName] = new Dictionary<string, string>();
        }

        configDict[Name] = value;
    }

    public virtual bool DoCommand(IRotationConfigSet set, string str) => str.StartsWith(Name);
}
