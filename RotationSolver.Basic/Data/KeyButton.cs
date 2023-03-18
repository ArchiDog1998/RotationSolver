using Dalamud.Game.ClientState.GamePad;
using Dalamud.Game.ClientState.Keys;

namespace RotationSolver.Basic.Data;

public record KeyRecord(VirtualKey key, bool control, bool alt, bool shift);
public record ButtonRecord(GamepadButtons button, bool l2, bool r2);

public static class RecordExtension
{
    public static string ToStr(this ButtonRecord record)
    {
        string result = "";
        if (record.l2) result += "LT + ";
        if (record.r2) result += "RT + ";
        return result + record.button.ToString();
    }

    public static string ToStr(this KeyRecord record)
    {
        string result = "";
        if (record.control) result += "Ctrl + ";
        if (record.alt) result += "Alt + ";
        if (record.shift) result += "Shift + ";
        return result + record.key.ToString();
    }
}
