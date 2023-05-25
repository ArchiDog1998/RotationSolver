using Dalamud;
using Dalamud.Game;
using Dalamud.Game.DutyState;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Hooking;
using Dalamud.IoC;
using Dalamud.Utility.Signatures;
using ECommons.DalamudServices;
using FFXIVClientStructs.Attributes;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiScene;
using Lumina.Excel;
using RotationSolver.Basic.Configuration;
using System.Runtime.InteropServices;

namespace RotationSolver.Basic;

public static class Service
{
    public const string Command = "/rotation";

    public static float CountDownTime => Countdown.TimeRemaining;

    public static PluginConfiguration Config { get; set; }
    public static PluginConfiguration Default { get; } = new PluginConfiguration();

    public static ActionID GetAdjustedActionId(ActionID id)
        => (ActionID)GetAdjustedActionId((uint)id);

    public static unsafe uint GetAdjustedActionId(uint id)
    => ActionManager.Instance()->GetAdjustedActionId(id);

    public unsafe static IEnumerable<IntPtr> GetAddons<T>() where T : struct
    {
        if(typeof(T).GetCustomAttribute<Addon>() is not Addon on) return Array.Empty<nint>();

        return on.AddonIdentifiers
            .Select(str => Svc.GameGui.GetAddonByName(str, 1))
            .Where(ptr => ptr != IntPtr.Zero);
    }

    public static ExcelSheet<T> GetSheet<T>() where T : ExcelRow => Svc.Data.GetExcelSheet<T>();
}
