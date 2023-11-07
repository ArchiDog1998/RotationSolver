namespace RotationSolver.Basic.Configuration.RotationConfig;

/// <summary>
/// The config set about this config..
/// </summary>
public interface IRotationConfigSet : IEnumerable<IRotationConfig>
{
    /// <summary>
    /// The configs.
    /// </summary>
    HashSet<IRotationConfig> Configs { get; }

    /// <summary>
    /// Set the double.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="displayName"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="speed"></param>
    /// <returns></returns>
    [Obsolete("Please use the one with types!", true)]
    IRotationConfigSet SetFloat(string name, float value, string displayName, float min = 0, float max = 1, float speed = 0.002f);

    /// <summary>
    /// Set the float.
    /// </summary>
    /// <param name="unit">tye unit type</param>
    /// <param name="type">the combat type</param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="displayName"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="speed"></param>
    IRotationConfigSet SetFloat(ConfigUnitType unit, CombatType type, string name, float value, string displayName, float min = 0, float max = 1, float speed = 0.002f);

    /// <summary>
    /// Set the string.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="displayName"></param>
    /// <returns></returns>
    [Obsolete("Please use the one with types!", true)]
    IRotationConfigSet SetString(string name, string value, string displayName);

    /// <summary>
    /// Set the string.
    /// </summary>
    /// <param name="type">the combat type</param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="displayName"></param>
    /// <returns></returns>
    IRotationConfigSet SetString(CombatType type, string name, string value, string displayName);

    /// <summary>
    /// Set the bool.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="displayName"></param>
    /// <returns></returns>
    [Obsolete("Please use the one with types!", true)]
    IRotationConfigSet SetBool(string name, bool value, string displayName);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type">the combat type</param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="displayName"></param>
    /// <returns></returns>
    IRotationConfigSet SetBool(CombatType type, string name, bool value, string displayName);

    /// <summary>
    /// Set the combo.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="displayName"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    [Obsolete("Please use the one with types!", true)]
    IRotationConfigSet SetCombo(string name, int value, string displayName, params string[] items);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="displayName"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    IRotationConfigSet SetCombo(CombatType type, string name, int value, string displayName, params string[] items);

    /// <summary>
    /// Set the int.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="displayName"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="speed"></param>
    /// <returns></returns>
    [Obsolete("Please use the one with types!", true)]
    IRotationConfigSet SetInt(string name, int value, string displayName, int min = 0, int max = 10, int speed = 1);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="displayName"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="speed"></param>
    /// <returns></returns>
    IRotationConfigSet SetInt(CombatType type, string name, int value, string displayName, int min = 0, int max = 10, int speed = 1);

    /// <summary>
    /// Set the value.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    void SetValue(string name, string value);

    /// <summary>
    /// Get the combo.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    int GetCombo(string name);

    /// <summary>
    /// Get the bool.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool GetBool(string name);

    /// <summary>
    /// Get the float.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    float GetFloat(string name);

    /// <summary>
    /// Get the int.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    int GetInt(string name);

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
