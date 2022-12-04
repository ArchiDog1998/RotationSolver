using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Linq;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.Basic;

internal abstract class DRGCombo_Base<TCmd> : CustomCombo<TCmd> where TCmd : Enum
{
    private static DRGGauge JobGauge => Service.JobGauges.Get<DRGGauge>();

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Dragoon, ClassJobID.Lancer };

    /// <summary>
    /// æ´◊º¥Ã
    /// </summary>
    public static BaseAction TrueThrust { get; } = new(ActionID.TrueThrust);

    /// <summary>
    /// π·Õ®¥Ã
    /// </summary>
    public static BaseAction VorpalThrust { get; } = new(ActionID.VorpalThrust)
    {
        OtherIDsCombo = new[] { ActionID.RaidenThrust }
    };

    /// <summary>
    /// ÷±¥Ã
    /// </summary>
    public static BaseAction FullThrust { get; } = new(ActionID.FullThrust);

    /// <summary>
    /// ø™Ã≈«π
    /// </summary>
    public static BaseAction Disembowel { get; } = new(ActionID.Disembowel)
    {
        OtherIDsCombo = new[] { ActionID.RaidenThrust }
    };

    /// <summary>
    /// ”£ª®≈≠∑≈
    /// </summary>
    public static BaseAction ChaosThrust { get; } = new(ActionID.ChaosThrust);

    /// <summary>
    /// ¡˙—¿¡˙◊¶
    /// </summary>
    public static BaseAction FangandClaw { get; } = new(ActionID.FangandClaw)
    {
        BuffsNeed = new StatusID[] { StatusID.SharperFangandClaw },
    };

    /// <summary>
    /// ¡˙Œ≤¥Ûªÿ–˝
    /// </summary>
    public static BaseAction WheelingThrust { get; } = new(ActionID.WheelingThrust)
    {
        BuffsNeed = new StatusID[] { StatusID.EnhancedWheelingThrust },
    };

    /// <summary>
    /// π·¥©º‚
    /// </summary>
    public static BaseAction PiercingTalon { get; } = new(ActionID.PiercingTalon);

    /// <summary>
    /// À¿ÃÏ«π
    /// </summary>
    public static BaseAction DoomSpike { get; } = new(ActionID.DoomSpike);

    /// <summary>
    /// “ÙÀŸ¥Ã
    /// </summary>
    public static BaseAction SonicThrust { get; } = new(ActionID.SonicThrust)
    {
        OtherIDsCombo = new[] { ActionID.DraconianFury }
    };

    /// <summary>
    /// …Ωæ≥ø·–Ã
    /// </summary>
    public static BaseAction CoerthanTorment { get; } = new(ActionID.CoerthanTorment);

    /// <summary>
    /// ∆∆ÀÈ≥Â
    /// </summary>
    public static BaseAction SpineshatterDive { get; } = new(ActionID.SpineshatterDive);

    /// <summary>
    /// ¡˙—◊≥Â
    /// </summary>
    public static BaseAction DragonfireDive { get; } = new(ActionID.DragonfireDive);

    /// <summary>
    /// Ã¯‘æ
    /// </summary>
    public static BaseAction Jump { get; } = new(ActionID.Jump)
    {
        BuffsProvide = new StatusID[] { StatusID.DiveReady },
    };

    /// <summary>
    /// ∏ﬂÃ¯
    /// </summary>
    public static BaseAction HighJump { get; } = new(ActionID.HighJump)
    {
        BuffsProvide = Jump.BuffsProvide,
    };

    /// <summary>
    /// ª√œÛ≥Â
    /// </summary>
    public static BaseAction MirageDive { get; } = new(ActionID.MirageDive)
    {
        BuffsNeed = Jump.BuffsProvide,
    };

    /// <summary>
    /// Œ‰…Ò«π
    /// </summary>
    public static BaseAction Geirskogul { get; } = new(ActionID.Geirskogul);

    /// <summary>
    /// À¿’ﬂ÷Æ∞∂
    /// </summary>
    public static BaseAction Nastrond { get; } = new(ActionID.Nastrond)
    {
        ActionCheck = b => JobGauge.IsLOTDActive,
    };

    /// <summary>
    /// ◊π–«≥Â
    /// </summary>
    public static BaseAction Stardiver { get; } = new(ActionID.Stardiver)
    {
        ActionCheck = b => JobGauge.IsLOTDActive,
    };

    /// <summary>
    /// ÃÏ¡˙µ„æ¶
    /// </summary>
    public static BaseAction WyrmwindThrust { get; } = new(ActionID.WyrmwindThrust)
    {
        ActionCheck = b => JobGauge.FirstmindsFocusCount == 2,
    };

    /// <summary>
    /// ¡˙Ω£
    /// </summary>
    public static BaseAction LifeSurge { get; } = new(ActionID.LifeSurge, true)
    {
        BuffsProvide = new[] { StatusID.LifeSurge },

        ActionCheck = b => !IsLastAbility(true, LifeSurge),
    };

    /// <summary>
    /// √Õ«π
    /// </summary>
    public static BaseAction LanceCharge { get; } = new(ActionID.LanceCharge, true);

    /// <summary>
    /// æﬁ¡˙ ”œﬂ
    /// </summary>
    public static BaseAction DragonSight { get; } = new(ActionID.DragonSight, true)
    {
        ChoiceTarget = Targets =>
        {
            Targets = Targets.Where(b => b.ObjectId != Service.ClientState.LocalPlayer.ObjectId &&
            !b.HasStatus(false, StatusID.Weakness, StatusID.BrinkofDeath)).ToArray();

            return Targets.GetJobCategory(JobRole.Melee, JobRole.RangedMagicial, JobRole.RangedPhysical, JobRole.Tank).FirstOrDefault();
        },
    };

    /// <summary>
    /// ’Ω∂∑¡¨µª
    /// </summary>
    public static BaseAction BattleLitany { get; } = new(ActionID.BattleLitany, true)
    {
        BuffsNeed = new[] { StatusID.PowerSurge },
    };
}
