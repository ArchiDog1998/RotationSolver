using ECommons.ExcelServices;

namespace RotationSolver.Basic.Configuration.RotationConfig;

/// <summary>
/// The single config of rotation.
/// </summary>
public interface IRotationConfig
{
    /// <summary>
    /// The name of this setting.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The showing name about this.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Default Value for this configuration.
    /// </summary>
    string DefaultValue { get; }

    /// <summary>
    /// Type of this config, pvp, pve, or both.
    /// </summary>
    CombatType Type { get; }

    /// <summary>
    /// The value.
    /// </summary>
    string Value { get; set; }

    /// <summary>
    /// Happened when it is on the command.
    /// </summary>
    /// <param name="set"></param>
    /// <param name="str"></param>
    /// <returns></returns>
    bool DoCommand(IRotationConfigSet set, string str);
}
