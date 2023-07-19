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
    /// Get the value of this.
    /// </summary>
    /// <param name="job"></param>
    /// <param name="rotationName"></param>
    /// <returns></returns>
    string GetValue(Job job, string rotationName);

    /// <summary>
    /// Get the showing value of this config.
    /// </summary>
    /// <param name="job"></param>
    /// <param name="rotationName"></param>
    /// <returns></returns>
    string GetDisplayValue(Job job, string rotationName);

    /// <summary>
    /// Set this value.
    /// </summary>
    /// <param name="job"></param>
    /// <param name="rotationName"></param>
    /// <param name="value"></param>
    void SetValue(Job job, string rotationName, string value);

    /// <summary>
    /// Happend when it is on the command.
    /// </summary>
    /// <param name="set"></param>
    /// <param name="str"></param>
    /// <returns></returns>
    bool DoCommand(IRotationConfigSet set, string str);
}
