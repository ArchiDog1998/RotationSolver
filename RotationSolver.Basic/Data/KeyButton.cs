using Dalamud.Game.ClientState.GamePad;
using Dalamud.Game.ClientState.Keys;
using ECommons.Gamepad;

namespace RotationSolver.Basic.Data;

/// <summary>
/// The record value about key
/// </summary>
/// <param name="Key">Key name</param>
/// <param name="Control">Needs control</param>
/// <param name="Alt">Needs alt</param>
/// <param name="Shift">Needs shift</param>
public record KeyRecord(VirtualKey Key, bool Control, bool Alt, bool Shift);

/// <summary>
/// The record value about button
/// </summary>
/// <param name="Button">Button itself.</param>
/// <param name="L2">LT</param>
/// <param name="R2">RT</param>
public record ButtonRecord(GamepadButtons Button, bool L2, bool R2);

/// <summary>
/// The name about record.
/// </summary>
public static class RecordExtension
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="record"></param>
    /// <returns></returns>
    public static string ToStr(this ButtonRecord record)
    {
        string result = "";
        if (record.L2) result += GamePad.ControllerButtons[GamepadButtons.L2] + " + ";
        if (record.R2) result += GamePad.ControllerButtons[GamepadButtons.R2] + " + ";
        return result + GamePad.ControllerButtons[record.Button];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="record"></param>
    /// <returns></returns>
    public static string ToStr(this KeyRecord record)
    {
        string result = "";
        if (record.Control) result += "Ctrl + ";
        if (record.Alt) result += "Alt + ";
        if (record.Shift) result += "Shift + ";
        return result + record.Key.ToString();
    }
}
