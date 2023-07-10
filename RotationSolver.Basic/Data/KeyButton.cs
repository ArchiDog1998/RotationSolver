using Dalamud.Game.ClientState.GamePad;
using Dalamud.Game.ClientState.Keys;
using ECommons.Gamepad;

namespace RotationSolver.Basic.Data;

public record KeyRecord(VirtualKey Key, bool Control, bool Alt, bool Shift);
public record ButtonRecord(GamepadButtons Button, bool L2, bool R2);

public static class RecordExtension
{
    public static string ToStr(this ButtonRecord record)
    {
        string result = "";
        if (record.L2) result += GamePad.ControllerButtons[GamepadButtons.L2] + " + ";
        if (record.R2) result += GamePad.ControllerButtons[GamepadButtons.R2] + " + ";
        return result + GamePad.ControllerButtons[record.Button];
    }

    public static string ToStr(this KeyRecord record)
    {
        string result = "";
        if (record.Control) result += "Ctrl + ";
        if (record.Alt) result += "Alt + ";
        if (record.Shift) result += "Shift + ";
        return result + record.Key.ToString();
    }
}
