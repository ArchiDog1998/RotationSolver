namespace RotationSolver.Basic.Configuration.RotationConfig;

internal abstract class RotationConfigBase(string name, string value, string displayName, CombatType type) 
    : IRotationConfig
{
    public string Name { get; } = name;
    public string DefaultValue { get; } = value;
    public string DisplayName { get; } = displayName;
    public CombatType Type { get; } = type;

    public string Value 
    {
        get
        {
            if (!Service.Config.RotationsConfigurations.TryGetValue(Name, out var config)) return DefaultValue;
            return config;
        }
        set
        {
            Service.Config.RotationsConfigurations[Name] = value;
        }
    }

    public virtual bool DoCommand(IRotationConfigSet set, string str) => str.StartsWith(Name);

    public override string ToString() => Value;
}
