using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using RotationSolver.Commands;
using RotationSolver.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;


namespace RotationSolver.Helpers;

internal static class StatusHelper
{
    public static StatusID[] AreaHots { get; } = new StatusID[]
    {
        StatusID.AspectedHelios, StatusID.Medica2, StatusID.TrueMedica2
    };

    public static StatusID[] SingleHots { get; } = new StatusID[]
    {
        StatusID.AspectedBenefic, StatusID.Regen1, StatusID.Regen2, StatusID.Regen3
    };

    public static StatusID[] SheildStatus { get; } = new StatusID[]
    {
        StatusID.Grit, StatusID.RoyalGuard, StatusID.IronWill, StatusID.Defiance
    };

    public static StatusID[] NoNeedHealingStatus { get; } = new StatusID[]
    {
        StatusID.Holmgang, StatusID.WillDead, StatusID.WalkingDead,
    };

    public static bool NeedHealing(BattleChara p) => p.WillStatusEndGCD(2, 0, false, NoNeedHealingStatus);

    /// <summary>
    /// Will any of <paramref name="statusIDs"/> be end after <paramref name="gcdCount"/> gcds and <paramref name="abilityCount"/> abilities?
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="gcdCount"></param>
    /// <param name="abilityCount"></param>
    /// <param name="isFromSelf"></param>
    /// <param name="statusIDs"></param>
    /// <returns></returns>
    internal static bool WillStatusEndGCD(this BattleChara obj, uint gcdCount = 0, uint abilityCount = 0, bool isFromSelf = true, params StatusID[] statusIDs)
    {
        var remain = obj.StatusTime(isFromSelf, statusIDs);
        return CooldownHelper.RecastAfterGCD(remain, gcdCount, abilityCount);
    }


    /// <summary>
    /// Will any of <paramref name="statusIDs"/> be end after <paramref name="time"/> seconds?
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="time"></param>
    /// <param name="isFromSelf"></param>
    /// <param name="statusIDs"></param>
    /// <returns></returns>
    internal static bool WillStatusEnd(this BattleChara obj, float time, bool isFromSelf = true, params StatusID[] statusIDs)
    {
        var remain = obj.StatusTime(isFromSelf, statusIDs);
        return CooldownHelper.RecastAfter(remain, time);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static float StatusTime(this BattleChara obj, bool isFromSelf, params StatusID[] statusIDs)
    {
        var times = obj.StatusTimes(isFromSelf, statusIDs);
        if (times == null || !times.Any()) return 0;
        return times.Min();
    }

    private static IEnumerable<float> StatusTimes(this BattleChara obj, bool isFromSelf, params StatusID[] statusIDs)
    {
        return obj.GetStatus(isFromSelf, statusIDs).Select(status => status.RemainingTime == 0 ? float.MaxValue : status.RemainingTime);
    }

    internal static byte StatusStack(this BattleChara obj, bool isFromSelf, params StatusID[] statusIDs)
    {
        var stacks = obj.StatusStacks(isFromSelf, statusIDs);
        if (stacks == null || !stacks.Any()) return 0;
        return stacks.Min();
    }

    private static IEnumerable<byte> StatusStacks(this BattleChara obj, bool isFromSelf, params StatusID[] statusIDs)
    {
        return obj.GetStatus(isFromSelf, statusIDs).Select(status => status.StackCount == 0 ? byte.MaxValue : status.StackCount);
    }

    /// <summary>
    /// Has one status right now.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="isFromSelf"></param>
    /// <param name="statusIDs"></param>
    /// <returns></returns>
    internal static bool HasStatus(this BattleChara obj, bool isFromSelf, params StatusID[] statusIDs)
    {
        return obj.GetStatus(isFromSelf, statusIDs).Any();
    }

    internal static void StatusOff(StatusID status)
    {
        RSCommands.SubmitToChat($"/statusoff {GetStatusName(status)}");
    }

    internal static string GetStatusName(StatusID id)
    {
        return Service.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Status>().GetRow((uint)id).Name.ToString();
    }

    private static IEnumerable<Status> GetStatus(this BattleChara obj, bool isFromSelf, params StatusID[] statusIDs)
    {
        var newEffects = statusIDs.Select(a => (uint)a);
        return obj.GetAllStatus(isFromSelf).Where(status => newEffects.Contains(status.StatusId));
    }

    private static IEnumerable<Status> GetAllStatus(this BattleChara obj, bool isFromSelf)
    {
        if (obj == null) return new Status[0];

        return obj.StatusList.Where(status => isFromSelf ? status.SourceId == Service.ClientState.LocalPlayer.ObjectId
        || status.SourceObject?.OwnerId == Service.ClientState.LocalPlayer.ObjectId : true);
    }

    static readonly StatusID[] invincibalStatus = new StatusID[]
    {
        StatusID.StoneSkin,
    };

    internal static bool IsInvincible(this Status status)
    {
        if (status.GameData.Icon == 15024) return true;

        return invincibalStatus.Any(id => (uint)id == status.StatusId);
    }

    static readonly StatusID[] dangeriousStatus = new StatusID[]
    {
        StatusID.Doom,
        StatusID.Amnesia,
        StatusID.Stun,
        StatusID.Stun2,
        StatusID.Sleep,
        StatusID.Sleep2,
        StatusID.Sleep3,
        StatusID.Pacification,
        StatusID.Pacification2,
        StatusID.Silence,
        StatusID.Slow,
        StatusID.Slow2,
        StatusID.Slow3,
        StatusID.Slow4,
        StatusID.Slow5,
        StatusID.Blind,
        StatusID.Blind2,
        StatusID.Blind3,
        StatusID.Paralysis,
        StatusID.Paralysis2,
        StatusID.Nightmare,
        StatusID.Necrosis,
    };

    internal static bool IsDangerous(this Status status)
    {
        if(status.StackCount > 2) return true;
        return dangeriousStatus.Any(id => (uint)id == status.StatusId);
    }
}
