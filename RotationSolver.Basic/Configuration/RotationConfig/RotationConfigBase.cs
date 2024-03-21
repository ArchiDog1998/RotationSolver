using ECommons.DalamudServices;

namespace RotationSolver.Basic.Configuration.RotationConfig;

internal abstract class RotationConfigBase
    : IRotationConfig
{
    readonly PropertyInfo _property;
    readonly ICustomRotation _rotation;
    public string Name { get; }
    public string DefaultValue { get; }
    public string DisplayName { get; }
    public CombatType Type { get; }

    public string Value 
    {
        get
        {
            if (!Service.Config.RotationConfigurations.TryGetValue(Name, out var config)) return DefaultValue;
            return config;
        }
        set
        {
            Service.Config.RotationConfigurations[Name] = value;
            SetValue(value);
        }
    }

    protected RotationConfigBase(ICustomRotation rotation, PropertyInfo property)
    {
        _property = property;
        _rotation = rotation;

        Name = property.Name;
        DefaultValue = property.GetValue(rotation)?.ToString() ?? string.Empty;
        var attr = property.GetCustomAttribute<RotationConfigAttribute>();
        if (attr != null)
        {
            DisplayName = attr.Name;
            Type = attr.Type;
        }
        else
        {
            DisplayName = Name;
            Type = CombatType.None;
        }

        //Set Up
        if (Service.Config.RotationConfigurations.TryGetValue(Name, out var value))
        {
            SetValue(value);
        }
    }

    private void SetValue(string value)
    {
        var type = _property.PropertyType;
        if (type == null) return;

        try
        {
            _property.SetValue(_rotation, ChangeType(value, type));
        }
        catch (Exception ex)
        {
            Svc.Log.Error(ex, "Failed to convert type.");
            _property.SetValue(_rotation, ChangeType(DefaultValue, type));
        }
    }

    private static object ChangeType(string value, Type type)
    {
        if (type.IsEnum)
        {
            return Enum.Parse(type, value);
        }
        else if(type == typeof(bool))
        {
            return bool.Parse(value);
        }

        return Convert.ChangeType(value, type);
    }

    public virtual bool DoCommand(IRotationConfigSet set, string str) => str.StartsWith(Name);

    public override string ToString() => Value;
}
