using Dalamud.Game.ClientState.Statuses;
using ECommons.Automation;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using RotationSolver.Basic.Configuration;

namespace RotationSolver.Basic.Helpers;

/// <summary>
/// The helper for the status.
/// </summary>
public static class StatusHelper
{
    /// <summary>
    /// 
    /// </summary>
    public static StatusID[] AreaHots { get; } = new StatusID[]
    {
        StatusID.AspectedHelios, StatusID.Medica2, StatusID.TrueMedica2
    };

    /// <summary>
    /// 
    /// </summary>
    public static StatusID[] SingleHots { get; } = new StatusID[]
    {
        StatusID.AspectedBenefic, StatusID.Regen1, StatusID.Regen2, StatusID.Regen3
    };

    internal static StatusID[] TankStanceStatus { get; } = new StatusID[]
    {
        StatusID.Grit, StatusID.RoyalGuard, StatusID.IronWill, StatusID.Defiance
    };

    internal static StatusID[] NoNeedHealingStatus { get; } = new StatusID[]
    {
        StatusID.Holmgang, StatusID.LivingDead, //StatusID.WalkingDead,
    };

    internal record Burst2MinsInfo(StatusID Status, bool IsOnHostile, byte Level, params Job[] Jobs);

    internal static Burst2MinsInfo[] Burst2Mins { get; } = new Burst2MinsInfo[]
    {
        new Burst2MinsInfo(StatusID.Divination, false, AST_Base.Divination.Level, Job.AST),
        new Burst2MinsInfo(StatusID.ChainStratagem, true, SCH_Base.ChainStratagem.Level, Job.SCH),
        new Burst2MinsInfo(StatusID.Brotherhood, false, MNK_Base.Brotherhood.Level, Job.MNK),
        new Burst2MinsInfo(StatusID.BattleLitany, false, DRG_Base.BattleLitany.Level, Job.DRG),
        new Burst2MinsInfo(StatusID.ArcaneCircle, false, RPR_Base.ArcaneCircle.Level, Job.RPR),
        new Burst2MinsInfo(StatusID.BattleVoice, false, BRD_Base.BattleVoice.Level,Job.BRD ),
        new Burst2MinsInfo(StatusID.TechnicalFinish, false, DNC_Base.TechnicalStep.Level, Job.DNC),
        new Burst2MinsInfo(StatusID.SearingLight, false, SMN_Base.SearingLight.Level, Job.SMN),
        new Burst2MinsInfo(StatusID.Embolden, false, RDM_Base.Embolden.Level, Job.RDM),
        new Burst2MinsInfo(StatusID.Mug, true, NIN_Base.Mug.Level, Job.NIN, Job.ROG),
    };

    /// <summary>
    /// Check whether the target needs to be healing.
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public static bool NeedHealing(this BattleChara p) => p.WillStatusEndGCD(2, 0, false, NoNeedHealingStatus);

    /// <summary>
    /// Will any of <paramref name="statusIDs"/> be end after <paramref name="gcdCount"/> gcds add <paramref name="offset"/> seconds?
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="gcdCount"></param>
    /// <param name="offset"></param>
    /// <param name="isFromSelf"></param>
    /// <param name="statusIDs"></param>
    /// <returns></returns>
    public static bool WillStatusEndGCD(this BattleChara obj, uint gcdCount = 0, float offset = 0, bool isFromSelf = true, params StatusID[] statusIDs)
        => WillStatusEnd(obj, DataCenter.GCDTime(gcdCount, offset), isFromSelf, statusIDs);


    /// <summary>
    /// Will any of <paramref name="statusIDs"/> be end after <paramref name="time"/> seconds?
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="time"></param>
    /// <param name="isFromSelf"></param>
    /// <param name="statusIDs"></param>
    /// <returns></returns>
    public static bool WillStatusEnd(this BattleChara obj, float time, bool isFromSelf = true, params StatusID[] statusIDs)
    {
        if (DataCenter.HasApplyStatus(obj?.ObjectId ?? 0, statusIDs)) return false;
        var remain = obj.StatusTime(isFromSelf, statusIDs);
        if (remain < 0 && obj.HasStatus(isFromSelf, statusIDs)) return false;
        return remain <= time;
    }

    /// <summary>
    /// Please Do NOT use it!
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="isFromSelf"></param>
    /// <param name="statusIDs"></param>
    /// <returns></returns>
    internal static float StatusTime(this BattleChara obj, bool isFromSelf, params StatusID[] statusIDs)
    {
        try
        {
            if (DataCenter.HasApplyStatus(obj?.ObjectId ?? 0, statusIDs)) return float.MaxValue;
            var times = obj.StatusTimes(isFromSelf, statusIDs);
            if (times == null || !times.Any()) return 0;
            return Math.Max(0, times.Min() - DataCenter.WeaponRemain);
        }
        catch
        {
            return 0;
        }
    }

    private static IEnumerable<float> StatusTimes(this BattleChara obj, bool isFromSelf, params StatusID[] statusIDs)
    {
        return obj.GetStatus(isFromSelf, statusIDs).Select(status => status.RemainingTime == 0 ? float.MaxValue : status.RemainingTime);
    }

    /// <summary>
    /// Get the stack of the status.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="isFromSelf"></param>
    /// <param name="statusIDs"></param>
    /// <returns></returns>
    public static byte StatusStack(this BattleChara obj, bool isFromSelf, params StatusID[] statusIDs)
    {
        if (DataCenter.HasApplyStatus(obj?.ObjectId ?? 0, statusIDs)) return byte.MaxValue;
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
    public static bool HasStatus(this BattleChara obj, bool isFromSelf, params StatusID[] statusIDs)
    {
        if (DataCenter.HasApplyStatus(obj?.ObjectId ?? 0, statusIDs)) return true;
        return obj.GetStatus(isFromSelf, statusIDs).Any();
    }

    /// <summary>
    /// Take the status Off.
    /// </summary>
    /// <param name="status"></param>
    public static void StatusOff(StatusID status)
    {
        if (!Player.Object?.HasStatus(false, status) ?? true) return;
        Chat.Instance.SendMessage($"/statusoff {GetStatusName(status)}");
    }

    internal static string GetStatusName(StatusID id)
    {
        return Service.GetSheet<Lumina.Excel.GeneratedSheets.Status>().GetRow((uint)id).Name.ToString();
    }

    private static IEnumerable<Status> GetStatus(this BattleChara obj, bool isFromSelf, params StatusID[] statusIDs)
    {
        var newEffects = statusIDs.Select(a => (uint)a);
        return obj.GetAllStatus(isFromSelf).Where(status => newEffects.Contains(status.StatusId));
    }

    private static IEnumerable<Status> GetAllStatus(this BattleChara obj, bool isFromSelf)
    {
        if (obj == null) return Array.Empty<Status>();

        return obj.StatusList.Where(status => !isFromSelf
                                              || status.SourceId == Player.Object.ObjectId
                                              || status.SourceObject?.OwnerId == Player.Object.ObjectId);
    }

    /// <summary>
    /// Is status Invincible.
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    public static bool IsInvincible(this Status status)
    {
        if (status.GameData.Icon == 15024) return true;
        return OtherConfiguration.InvincibleStatus.Any(id => (uint)id == status.StatusId);
    }

    /// <summary>
    /// Is the status the priority one.
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    public static bool IsPriority(this Status status)
    {
        return OtherConfiguration.PriorityStatus.Any(id => (uint)id == status.StatusId);
    }

    /// <summary>
    /// Is status needs to be dispel immediately.
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    public static bool IsDangerous(this Status status)
    {
        if (!status.CanDispel()) return false;
        if (status.StackCount > 2) return true;
        return OtherConfiguration.DangerousStatus.Any(id => id == status.StatusId);
    }

    /// <summary>
    /// Can the status be dispel.
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    public static bool CanDispel(this Status status)
    {
        return status.GameData.CanDispel && status.RemainingTime > 1 + DataCenter.WeaponRemain;
    }
}
