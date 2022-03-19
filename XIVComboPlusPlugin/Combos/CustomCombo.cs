using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using Dalamud.Utility;
using XIVComboPlus.Attributes;

namespace XIVComboPlus.Combos;

internal abstract class CustomCombo
{
    #region Job

    internal abstract uint JobID { get; }

    public abstract string JobName { get; }
    #endregion

    #region Combo
    protected internal abstract uint[] ActionIDs { get; }
    public abstract string ComboFancyName { get; }

    public abstract string Description { get; }

    public virtual string[] ConflictingCombos => new string[0];
    public virtual string ParentCombo => string.Empty;

    public virtual bool SecretCombo => false;
    public virtual byte Priority => byte.MaxValue;
    public bool IsEnabled { get; set; } = false;

    #endregion
    protected static PlayerCharacter LocalPlayer => Service.ClientState.LocalPlayer;

    protected static GameObject CurrentTarget => Service.TargetManager.Target;

    protected CustomCombo()
    {
        //CustomComboInfoAttribute attribute = Preset.GetAttribute<CustomComboInfoAttribute>();
        //JobID = attribute.JobID;
        //ClassID = JobID switch
        //{
        //    0 => 0,
        //    25 => 7,
        //    23 => 5,
        //    22 => 4,
        //    20 => 2,
        //    30 => 29,
        //    19 => 1,
        //    28 => 15,
        //    27 => 26,
        //    21 => 3,
        //    24 => 6,
        //    _ => byte.MaxValue,
        //};
    }

    public bool TryInvoke(uint actionID, uint lastComboActionID, float comboTime, byte level, out uint newActionID)
    {
        newActionID = 0u;
        if (!IsEnabled)
        {
            return false;
        }
        if (!ActionIDs.Contains(actionID))
        {
            return false;
        }
        uint num2 = Invoke(actionID, lastComboActionID, comboTime, level);
        if (num2 == 0 || actionID == num2)
        {
            return false;
        }
        newActionID = num2;
        return true;
    }

    protected static uint CalcBestAction(uint original, params uint[] actions)
    {
        return actions.Select(new Func<uint, (uint, IconReplacer.CooldownData)>(Selector)).Aggregate(((uint ActionID, IconReplacer.CooldownData Data) a1, (uint ActionID, IconReplacer.CooldownData Data) a2) => Compare(original, a1, a2)).Item1;
        static (uint ActionID, IconReplacer.CooldownData Data) Compare(uint original, (uint ActionID, IconReplacer.CooldownData Data) a1, (uint ActionID, IconReplacer.CooldownData Data) a2)
        {
            if (!a1.Data.IsCooldown && !a2.Data.IsCooldown)
            {
                if (original != a1.ActionID)
                {
                    return a2;
                }
                return a1;
            }
            if (a1.Data.IsCooldown && a2.Data.IsCooldown)
            {
                if (!(a1.Data.CooldownRemaining < a2.Data.CooldownRemaining))
                {
                    return a2;
                }
                return a1;
            }
            if (!a1.Data.IsCooldown)
            {
                return a1;
            }
            return a2;
        }
        static (uint ActionID, IconReplacer.CooldownData Data) Selector(uint actionID)
        {
            return (actionID, GetCooldown(actionID));
        }
    }

    protected abstract uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level);

    protected static uint OriginalHook(uint actionID)
    {
        return Service.IconReplacer.OriginalHook(actionID);
    }

    protected static bool HasCondition(ConditionFlag flag)
    {
        //IL_0005: Unknown result type (might be due to invalid IL or missing references)
        return Service.Conditions[flag];
    }

    protected static bool HasEffect(ushort effectID)
    {
        return FindEffect(effectID) != null;
    }

    protected static Status FindEffect(ushort effectID)
    {
        PlayerCharacter localPlayer = LocalPlayer;
        PlayerCharacter localPlayer2 = LocalPlayer;
        return FindEffect(effectID, (GameObject)(object)localPlayer, localPlayer2 != null ? new uint?(localPlayer2.ObjectId) : null);
    }

    protected static bool TargetHasEffect(ushort effectID)
    {
        return FindTargetEffect(effectID) != null;
    }

    protected static Status FindTargetEffect(ushort effectID)
    {
        GameObject currentTarget = CurrentTarget;
        PlayerCharacter localPlayer = LocalPlayer;
        return FindEffect(effectID, currentTarget, localPlayer != null ? new uint?(localPlayer.ObjectId) : null);
    }

    protected static bool HasEffectAny(ushort effectID)
    {
        return FindEffectAny(effectID) != null;
    }

    protected static Status FindEffectAny(ushort effectID)
    {
        return FindEffect(effectID, (GameObject)(object)LocalPlayer, null);
    }

    protected static bool TargetHasEffectAny(ushort effectID)
    {
        return FindTargetEffectAny(effectID) != null;
    }

    protected static Status FindTargetEffectAny(ushort effectID)
    {
        return FindEffect(effectID, CurrentTarget, null);
    }

    protected static Status FindEffect(ushort effectID, GameObject obj, uint? sourceID)
    {
        if (obj == null)
        {
            return null;
        }
        BattleChara val = (BattleChara)(object)(obj is BattleChara ? obj : null);
        if (val == null)
        {
            return null;
        }
        foreach (Status status in val.StatusList)
        {
            if (status.StatusId == effectID && (!sourceID.HasValue || status.SourceID == 0 || status.SourceID == 3758096384u || status.SourceID == sourceID))
            {
                return status;
            }
        }
        return null;
    }

    protected float? TargetBuffDuration(ushort effectId)
    {
        return FindTargetEffect(effectId)?.RemainingTime;
    }

    protected byte BuffStacks(ushort effectId)
    {
        Status val = FindEffect(effectId);
        if (val != null)
        {
            return val.StackCount;
        }
        return 0;
    }

    protected float? BuffDuration(ushort effectId)
    {
        return FindEffect(effectId)?.RemainingTime;
    }

    protected static IconReplacer.CooldownData GetCooldown(uint actionID)
    {
        return Service.IconReplacer.GetCooldown(actionID);
    }

    protected abstract JobGaugeBase GetJobGaugeBase();
}
