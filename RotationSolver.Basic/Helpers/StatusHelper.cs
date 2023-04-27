using Dalamud.Game.ClientState.Statuses;
using Dalamud.Logging;

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
        StatusID.Holmgang, StatusID.WillDead, StatusID.LivingDead
    };

    public static SortedSet<uint> DangerousStatus { get; set; } = new SortedSet<uint>()
    {
        (uint)StatusID.Doom,
        (uint)StatusID.Amnesia,
        (uint)StatusID.Stun,
        (uint)StatusID.Stun2,
        (uint)StatusID.Sleep,
        (uint)StatusID.Sleep2,
        (uint)StatusID.Sleep3,
        (uint)StatusID.Pacification,
        (uint)StatusID.Pacification2,
        (uint)StatusID.Silence,
        (uint)StatusID.Slow,
        (uint)StatusID.Slow2,
        (uint)StatusID.Slow3,
        (uint)StatusID.Slow4,
        (uint)StatusID.Slow5,
        (uint)StatusID.Blind,
        (uint)StatusID.Blind2,
        (uint)StatusID.Blind3,
        (uint)StatusID.Paralysis,
        (uint)StatusID.Paralysis2,
        (uint)StatusID.Nightmare,
        (uint)StatusID.Necrosis,
    };

    public static SortedSet<uint> InvincibleStatus { get; set; } = new SortedSet<uint>()
    {
        (uint)StatusID.StoneSkin,
        (uint)StatusID.IceSpikesInvincible,
        (uint)StatusID.VortexBarrier,
    };

    static string s_dangerousStatusFile => Service.Interface.ConfigDirectory.FullName + $"\\{nameof(DangerousStatus)}.json";

    static string s_invincibleStatusFile => Service.Interface.ConfigDirectory.FullName + $"\\{nameof(InvincibleStatus)}.json";
    public static async void Enable()
    {
        if(File.Exists(s_dangerousStatusFile))
        {
            try
            {
                DangerousStatus = JsonConvert.DeserializeObject<SortedSet<uint>>(await File.ReadAllTextAsync(s_dangerousStatusFile));
            }
            catch(Exception ex)
            {
                PluginLog.Warning(ex, "Failed to load Dangerous Status List.");
                SaveDangerousStatus();
            }
        }
        else SaveDangerousStatus();

        if (File.Exists(s_invincibleStatusFile))
        {
            try
            {
                InvincibleStatus = JsonConvert.DeserializeObject<SortedSet<uint>>(await File.ReadAllTextAsync(s_invincibleStatusFile));
            }
            catch (Exception ex)
            {
                PluginLog.Warning(ex, "Failed to load Invincible Status List.");
                SaveInvincibleStatus();
            }
        }
        else SaveInvincibleStatus();
    }

    public static void SaveDangerousStatus()
    {
        File.WriteAllTextAsync(s_dangerousStatusFile,
        JsonConvert.SerializeObject(DangerousStatus, Formatting.Indented));
    }

    public static void SaveInvincibleStatus()
    {
        File.WriteAllTextAsync(s_invincibleStatusFile,
        JsonConvert.SerializeObject(InvincibleStatus, Formatting.Indented));
    }


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
    /// Will any of <paramref name="statusIDs"/> be end after <paramref name="gcdCount"/> gcds add <paramref name="offset"/> seconds?
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="gcdCount"></param>
    /// <param name="offset"></param>
    /// <param name="isFromSelf"></param>
    /// <param name="statusIDs"></param>
    /// <returns></returns>
    public static bool WillStatusEndGCD(this BattleChara obj, uint gcdCount = 0, float offset = 0, bool isFromSelf = true, params StatusID[] statusIDs)
    {
        if (DataCenter.HasApplyStatus(obj?.ObjectId ?? 0, statusIDs)) return false;
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
        if (DataCenter.HasApplyStatus(obj?.ObjectId ?? 0, statusIDs)) return false;
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
        if (DataCenter.HasApplyStatus(obj?.ObjectId ?? 0, statusIDs)) return float.MaxValue;
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
        if(DataCenter.HasApplyStatus(obj?.ObjectId ?? 0, statusIDs)) return true;
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

    public static bool IsInvincible(this Status status)
    {
        if (status.GameData.Icon == 15024) return true;
        return InvincibleStatus.Any(id => (uint)id == status.StatusId);
    }

    public static bool IsDangerous(this Status status)
    {
        if (!status.CanDispel()) return false;
        if (status.StackCount > 2) return true;
        return DangerousStatus.Any(id => id == status.StatusId);
    }

    public static bool CanDispel(this Status status)
    {
        return status.GameData.CanDispel && status.RemainingTime > 1 + DataCenter.WeaponRemain;
    }
}
