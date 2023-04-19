using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Hooking;
using Dalamud.Interface.Colors;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game;
using RotationSolver.Localization;
using System.Text.RegularExpressions;

namespace RotationSolver;

public class Watcher : IDisposable
{
    private unsafe delegate void ReceiveAbilityDelegate(uint sourceId, IntPtr sourceCharacter, Vector3* pos, ActionEffectHeader* effectHeader, ActionEffect* effectArray, ulong* effectTargets);

    /// <summary>
    /// https://github.com/Tischel/ActionTimeline/blob/master/ActionTimeline/Helpers/TimelineManager.cs#L86
    /// </summary>
    [Signature("4C 89 44 24 ?? 55 56 41 54 41 55 41 56", DetourName = nameof(ReceiveAbilityEffect))]
    private static Hook<ReceiveAbilityDelegate> _receiveAbilityHook;

    public Watcher()
    {
        SignatureHelper.Initialise(this);

        _receiveAbilityHook?.Enable();
    }

    public static string ShowStrSelf { get; private set; } = string.Empty;
    public static string ShowStrEnemy { get; private set; } = string.Empty;

    private static unsafe void ReceiveAbilityEffect(uint sourceId, IntPtr sourceCharacter, Vector3* pos, ActionEffectHeader* effectHeader, ActionEffect* effectArray, ulong* effectTargets)
    {
        _receiveAbilityHook.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTargets);
        if (Service.Player == null) return;

        try
        {
            var set = new ActionEffectSet(sourceId, effectHeader, effectArray, effectTargets);

            ActionFromSelf(sourceId, set, effectHeader->actionId);
            ActionFromEnemy(sourceId, set);
        }
        catch(Exception ex) 
        {
            PluginLog.Error(ex, "Error at Ability Receive.");
        }
    }

    private static void ActionFromEnemy(uint sourceId, ActionEffectSet set)
    {
        //Check Source.
        var source = Service.ObjectTable.SearchById(sourceId);
        if (source == null) return;
        if (source is not BattleChara battle) return;
        if (battle is PlayerCharacter) return;
        if (battle.SubKind == 9) return; //Friend!
        if (Service.ObjectTable.SearchById(battle.ObjectId) is PlayerCharacter) return;

        var damageRatio = set.TargetEffects
            .Where(e => e.Target == Service.Player)
            .SelectMany(e => new ActionEffect[]
            {
                e[0], e[1], e[2], e[3], e[4], e[5], e[6], e[7]
            })
            .Where(e => e.Type == ActionEffectType.Damage)
            .Sum(e => (float)e.Value / Service.Player.MaxHp);

        DataCenter.AddDamageRec(damageRatio);

        ShowStrEnemy = $"Damage Ratio: {damageRatio}\n{set}";
    }

    private static void ActionFromSelf(uint sourceId, ActionEffectSet set, uint id)
    {
        if (sourceId != Service.Player.ObjectId) return;
        if (set.Type != ActionType.Spell && set.Type != ActionType.Item) return;
        if ((ActionCate)set.Action?.ActionCategory.Value.RowId == ActionCate.AutoAttack) return;

        IActionHelper.AnimationLockTime[id] = set.AnimationLock;

        if (!set.TargetEffects.Any()) return;
        var flag = set.TargetEffects.FirstOrDefault()[0].Param2;

        var action = set.Action;
        var tar = set.Target;

        if (tar == null || action == null) return;

        //Record
        DataCenter.AddActionRec(set.Action);
        DataCenter.NextActionMinTime = DateTime.Now.AddSeconds(set.AnimationLock);
        ShowStrSelf = set.ToString();

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

        //Positional
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
