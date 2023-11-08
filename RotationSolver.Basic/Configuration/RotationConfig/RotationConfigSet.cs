using ECommons.ExcelServices;
using System.Collections;

namespace RotationSolver.Basic.Configuration.RotationConfig;

internal class RotationConfigSet : IRotationConfigSet
{
    readonly Job _job;
    readonly string _rotationName;
    public HashSet<IRotationConfig> Configs { get; } = new HashSet<IRotationConfig>(new RotationConfigComparer());

    public RotationConfigSet(Job job, string rotationName)
    {
        _job = job;
        _rotationName = rotationName;
    }

    #region Set
    [Obsolete("Please use the one with types!", true)]
    public IRotationConfigSet SetFloat(string name, float value, string displayName, float min = 0, float max = 1, float speed = 0.002f)
    {
        return SetFloat(ConfigUnitType.None, CombatType.Both, name, value, displayName, min, max, speed);
    }

    public IRotationConfigSet SetFloat(ConfigUnitType unit, CombatType type, string name, float value, string displayName, float min = 0, float max = 1, float speed = 0.002f)
    {
        Configs.Add(new RotationConfigFloat(name, value, displayName, min, max, speed, unit, type));
        return this;
    }

    [Obsolete("Please use the one with types!", true)]
    public IRotationConfigSet SetString(string name, string value, string displayName)
    {
        return SetString(CombatType.Both, name, value, displayName);
    }

    public IRotationConfigSet SetString(CombatType type, string name, string value, string displayName)
    {
        Configs.Add(new RotationConfigString(name, value, displayName, type));
        return this;
    }

    [Obsolete("Please use the one with types!", true)]
    public IRotationConfigSet SetBool(string name, bool value, string displayName)
    {
        return SetBool(CombatType.Both, name, value, displayName);
    }

    public IRotationConfigSet SetBool(CombatType type, string name, bool value, string displayName)
    {
        Configs.Add(new RotationConfigBoolean(name, value, displayName, type));
        return this;
    }

    [Obsolete("Please use the one with types!", true)]
    public IRotationConfigSet SetCombo(string name, int value, string displayName, params string[] items)
    {
        return SetCombo(CombatType.Both, name, value, displayName, items);
    }

    public IRotationConfigSet SetCombo(CombatType type, string name, int value, string displayName, params string[] items)
    {
        Configs.Add(new RotationConfigCombo(name, value, displayName, items, type));
        return this;
    }

    [Obsolete("Please use the one with types!", true)]
    public IRotationConfigSet SetInt(string name, int value, string displayName, int min = 0, int max = 10, int speed = 1)
    {
        return SetInt(CombatType.Both, name, value, displayName, min, max, speed);
    }

    public IRotationConfigSet SetInt(CombatType type, string name, int value, string displayName, int min = 0, int max = 10, int speed = 1)
    {
        Configs.Add(new RotationConfigInt(name, value, displayName, min, max, speed, type));
        return this;
    }

    public void SetValue(string name, string value)
    {
        var config = Configs.FirstOrDefault(config => config.Name == name);
        if (config == null) return;
        config.SetValue(_job, _rotationName, value);
    }
    #endregion

    #region Get
    public int GetCombo(string name)
    {
        var result = GetString(name);
        if (int.TryParse(result, out var f)) return f;
        return 0;
    }

    public bool GetBool(string name)
    {
        var result = GetString(name);
        if (bool.TryParse(result, out var f)) return f;
        return false;
    }

    public float GetFloat(string name)
    {
        var result = GetString(name);
        if (float.TryParse(result, out var f)) return f;
        return float.NaN;
    }

    public string GetString(string name)
    {
        var config = GetConfig(name);
        return config?.GetValue(_job, _rotationName);
    }

    public int GetInt(string name)
    {
        var result = GetString(name);
        if (int.TryParse(result, out var f)) return f;
        return 0;
    }

    public string GetDisplayString(string name)
    {
        var config = GetConfig(name);
        return config?.GetDisplayValue(_job, _rotationName);
    }

    private IRotationConfig GetConfig(string name) => Configs.FirstOrDefault(config => config.Name == name);
    #endregion


    public IEnumerator<IRotationConfig> GetEnumerator() => Configs.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Configs.GetEnumerator();


}
