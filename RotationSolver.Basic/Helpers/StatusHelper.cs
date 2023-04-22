using Dalamud.Game.ClientState.Statuses;


namespace RotationSolver.Basic.Helpers;

public static class StatusHelper
{
    public static StatusID[] AreaHots { get; } = new StatusID[]
    {
        StatusID.AspectedHelios, StatusID.Medica2, StatusID.TrueMedica2
    };

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
        StatusID.Holmgang, StatusID.WillDead, StatusID.WalkingDead,
    };


    internal record Burst2MinsInfo( StatusID status, bool isOnHostile, byte level, params ClassJobID[] jobs);

    internal static Burst2MinsInfo[] Burst2Mins { get; } = new Burst2MinsInfo[]
    {
        new Burst2MinsInfo(StatusID.Divination, false, AST_Base.Divination.Level, ClassJobID.Astrologian),
        new Burst2MinsInfo(StatusID.ChainStratagem, true, SCH_Base.ChainStratagem.Level, ClassJobID.Scholar),
        new Burst2MinsInfo(StatusID.Brotherhood, false, MNK_Base.Brotherhood.Level, ClassJobID.Monk),
        new Burst2MinsInfo(StatusID.BattleLitany, false, DRG_Base.BattleLitany.Level, ClassJobID.Dragoon),
        new Burst2MinsInfo(StatusID.ArcaneCircle, false, RPR_Base.ArcaneCircle.Level, ClassJobID.Reaper),
        new Burst2MinsInfo(StatusID.BattleVoice, false, BRD_Base.BattleVoice.Level, ClassJobID.Bard),
        new Burst2MinsInfo(StatusID.TechnicalFinish, false, DNC_Base.TechnicalStep.Level, ClassJobID.Dancer),
        new Burst2MinsInfo(StatusID.SearingLight, false, SMN_Base.SearingLight.Level, ClassJobID.Summoner),
        new Burst2MinsInfo(StatusID.Embolden, false, RDM_Base.Embolden.Level, ClassJobID.RedMage),
        new Burst2MinsInfo(StatusID.Mug, true, NIN_Base.Mug.Level, ClassJobID.Ninja, ClassJobID.Rogue),
    };

    public static bool NeedHealing(this BattleChara p) => p.WillStatusEndGCD(2, 0, false, NoNeedHealingStatus);

    /// <summary>
    /// Will any of <paramref name="statusIDs"/> be end after <paramref name="gcdCount"/> gcds add <paramref name="offset" times/> ?
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="gcdCount"></param>
    /// <param name="offset"></param>
    /// <param name="isFromSelf"></param>
    /// <param name="statusIDs"></param>
    /// <returns></returns>
    public static bool WillStatusEndGCD(this BattleChara obj, uint gcdCount = 0, float offset = 0, bool isFromSelf = true, params StatusID[] statusIDs)
    {
        var remain = obj.StatusTime(isFromSelf, statusIDs);
        //as infinite
        if (remain < 0 && obj.HasStatus(isFromSelf, statusIDs)) return false;
        return CooldownHelper.RecastAfterGCD(remain, gcdCount, offset);
    }


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
        var remain = obj.StatusTime(isFromSelf, statusIDs);
        return CooldownHelper.RecastAfter(remain, time);
    }

    /// <summary>
    /// Please Do NOT use it!
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="isFromSelf"></param>
    /// <param name="statusIDs"></param>
    /// <returns></returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static float StatusTime(this BattleChara obj, bool isFromSelf, params StatusID[] statusIDs)
    {
        var times = obj.StatusTimes(isFromSelf, statusIDs);
        if (times == null || !times.Any()) return 0;
        return times.Min();
    }

    private static IEnumerable<float> StatusTimes(this BattleChara obj, bool isFromSelf, params StatusID[] statusIDs)
    {
        return obj.GetStatus(isFromSelf, statusIDs).Select(status => status.RemainingTime == 0 ? float.MaxValue : status.RemainingTime);
    }

    public static byte StatusStack(this BattleChara obj, bool isFromSelf, params StatusID[] statusIDs)
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
    public static bool HasStatus(this BattleChara obj, bool isFromSelf, params StatusID[] statusIDs)
    {
        return obj.GetStatus(isFromSelf, statusIDs).Any();
    }

    public static void StatusOff(StatusID status)
    {
        if (!Service.Player?.HasStatus(false, status) ?? true) return;
        Service.SubmitToChat($"/statusoff {GetStatusName(status)}");
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
        if (obj == null) return new Status[0];

        return obj.StatusList.Where(status => isFromSelf ? status.SourceId == Service.Player.ObjectId
        || status.SourceObject?.OwnerId == Service.Player.ObjectId : true);
    }

    static readonly StatusID[] invincibleStatus = new StatusID[]
    {
        StatusID.StoneSkin,
        StatusID.IceSpikesInvincible,
        StatusID.VortexBarrier,
    };

    public static bool IsInvincible(this Status status)
    {
        if (status.GameData.Icon == 15024) return true;

        return invincibleStatus.Any(id => (uint)id == status.StatusId);
    }

    static readonly StatusID[] dangerousStatus = new StatusID[]
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

    public static bool IsDangerous(this Status status)
    {
        if (!status.CanDispel()) return false;
        if (status.StackCount > 2) return true;
        if (status.RemainingTime > 8 + DataCenter.WeaponRemain) return true;
        return dangerousStatus.Any(id => (uint)id == status.StatusId);
    }

    public static bool CanDispel(this Status status)
    {
        return status.GameData.CanDispel && status.RemainingTime > 2;
    }
}
