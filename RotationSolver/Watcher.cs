using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Hooking;
using Dalamud.Interface.Colors;
using Dalamud.Utility.Signatures;
using ImGuiNET;
using RotationSolver.Basic;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;
using RotationSolver.Localization;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Speech.Synthesis;
using System.Text;
using System.Text.RegularExpressions;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver;

public class Watcher : IDisposable
{
    private delegate void ReceiveAbilityDelegate(uint sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail);

    /// <summary>
    /// https://github.com/Tischel/ActionTimeline/blob/master/ActionTimeline/Helpers/TimelineManager.cs#L86
    /// </summary>
    [Signature("4C 89 44 24 ?? 55 56 41 54 41 55 41 56", DetourName = nameof(ReceiveAbilityEffect))]
    private static Hook<ReceiveAbilityDelegate> _receiveAbilityHook;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static ActionID LastAction { get; set; } = 0;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static ActionID LastGCD { get; set; } = 0;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static ActionID LastAbility { get; set; } = 0;

    public Watcher()
    {
        SignatureHelper.Initialise(this);

        _receiveAbilityHook?.Enable();
    }

    private static void ReceiveAbilityEffect(uint sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail)
    {
        _receiveAbilityHook.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail);

        //不是自己放出来的
        if (Service.Player == null || sourceId != Service.Player.ObjectId) return;

        //不是一个Spell
        if (Marshal.ReadByte(effectHeader, 31) != 1) return;

        //获得行为
        var action = Service.GetSheet<Action>().GetRow((uint)Marshal.ReadInt32(effectHeader, 0x8));

        //获得目标
        var tar = Service.ObjectTable.SearchById((uint)Marshal.ReadInt32(effectHeader)) ?? Service.Player;

        //获得身为技能是否正确flag
        var flag = Marshal.ReadByte(effectArray + 3);
        RecordAction(tar, action, flag);
    }

    private static unsafe void RecordAction(GameObject tar, Action action, byte flag)
    {
        DataCenter.AddActionRec(action);

        //Macro
        foreach (var item in Service.Config.Events)
        {
            if (!new Regex(item.Name).Match(action.Name).Success) continue;
            if (item.AddMacro(tar)) break;
        }

        if (flag != 0 && Service.Config.ShowActionFlag)
        {
            Service.FlyTextGui.AddFlyText(Dalamud.Game.Gui.FlyText.FlyTextKind.NamedIcon, 0, 0, 0, "Flag:" + flag.ToString(), "",
            ImGui.GetColorU32(ImGuiColors.DPSRed), 0, action.Icon);
        }

        //事后骂人！
        if (Service.Config.PositionalFeedback
            && ConfigurationHelper.ActionPositional.TryGetValue((ActionID)action.RowId, out var pos)
            && pos.Tags.Length > 0 && !pos.Tags.Contains(flag))
        {
            Service.FlyTextGui.AddFlyText(Dalamud.Game.Gui.FlyText.FlyTextKind.NamedIcon, 0, 0, 0, pos.Pos.ToName(), "",
                ImGui.GetColorU32(ImGuiColors.DPSRed), 94662, action.Icon);
            if (!string.IsNullOrEmpty(Service.Config.PositionalErrorText))
            {
                SpeechHelper.Speak(Service.Config.PositionalErrorText);
            }
        }
    }



    public void Dispose()
    {
        _receiveAbilityHook?.Dispose();
    }
}
