using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Hooking;
using Dalamud.Interface.Colors;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using RotationSolver.Basic;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;
using RotationSolver.Localization;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Speech.Synthesis;
using System.Text;
using System.Text.RegularExpressions;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver;

public class Watcher : IDisposable
{
    private unsafe delegate void ReceiveAbilityDelegate(uint sourceId, IntPtr sourceCharacter, Vector3* pos, ActionEffectHeader* effectHeader, ActionEffect* effectArray, ulong* effectTargets);

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

    public static string ShowStr { get; private set; } = string.Empty;

    private static unsafe void ReceiveAbilityEffect(uint sourceId, IntPtr sourceCharacter, Vector3* pos, ActionEffectHeader* effectHeader, ActionEffect* effectArray, ulong* effectTargets)
    {
        _receiveAbilityHook.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTargets);

        //不是自己放出来的
        if (Service.Player == null || sourceId != Service.Player.ObjectId) return;

        var set = new ActionEffectSet(effectHeader, effectArray, effectTargets);

        //不是一个Spell
        if (set.Type != ActionType.Spell) return;
        if ((ActionCate)set.Action?.ActionCategory.Value.RowId == ActionCate.AutoAttack) return;
        ShowStr = set.ToString();

        //获得身为技能是否正确flag
        RecordAction(set.Target, set.Action, effectArray->Param2);
    }

    private static unsafe void RecordAction(GameObject tar, Action action, byte flag)
    {
        if (tar == null || action == null) return;

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
