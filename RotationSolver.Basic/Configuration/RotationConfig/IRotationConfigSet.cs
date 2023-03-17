using System.Collections.Generic;

namespace RotationSolver.Basic.Configuration.RotationConfig;

public interface IRotationConfigSet : IEnumerable<IRotationConfig>
{
    HashSet<IRotationConfig> Configs { get; }

    IRotationConfigSet SetFloat(string name, float value, string displayName, float min = 0, float max = 1, float speed = 0.002f);

    IRotationConfigSet SetString(string name, string value, string displayName);

    IRotationConfigSet SetBool(string name, bool value, string displayName);

    IRotationConfigSet SetCombo(string name, int value, string displayName, params string[] items);

    void SetValue(string name, string value);

    int GetCombo(string name);

    bool GetBool(string name);

    float GetFloat(string name);

    /// <summary>
    /// Get the raw string value in the saved dictionary, is not readable.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    string GetString(string name);

    /// <summary>
    /// Get the readable string for display.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    string GetDisplayString(string name);
}
