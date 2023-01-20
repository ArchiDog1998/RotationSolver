using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Linq;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Helpers;
using RotationSolver.Data;
using RotationSolver.Actions;

namespace RotationSolver.Rotations.Basic;

internal abstract class DRG_Base : CustomRotation.CustomRotation

{
    private static DRGGauge JobGauge => Service.JobGauges.Get<DRGGauge>();

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Dragoon, ClassJobID.Lancer };

    /// <summary>
    /// æ´◊º¥Ã
    /// </summary>
    public static IBaseAction TrueThrust { get; } = new BaseAction(ActionID.TrueThrust);

    /// <summary>
    /// π·Õ®¥Ã
    /// </summary>
    public static IBaseAction VorpalThrust { get; } = new BaseAction(ActionID.VorpalThrust)
    {
        OtherIDsCombo = new[] { ActionID.RaidenThrust }
    };

    /// <summary>
    /// ÷±¥Ã
    /// </summary>
    public static IBaseAction FullThrust { get; } = new BaseAction(ActionID.FullThrust);

    /// <summary>
    /// ø™Ã≈«π
    /// </summary>
    public static IBaseAction Disembowel { get; } = new BaseAction(ActionID.Disembowel)
    {
        OtherIDsCombo = new[] { ActionID.RaidenThrust }
    };

    /// <summary>
    /// ”£ª®≈≠∑≈
    /// </summary>
    public static IBaseAction ChaosThrust { get; } = new BaseAction(ActionID.ChaosThrust);

    /// <summary>
    /// ¡˙—¿¡˙◊¶
    /// </summary>
    public static IBaseAction FangandClaw { get; } = new BaseAction(ActionID.FangandClaw)
    {
        StatusNeed = new StatusID[] { StatusID.SharperFangandClaw },
    };

    /// <summary>
    /// ¡˙Œ≤¥Ûªÿ–˝
    /// </summary>
    public static IBaseAction WheelingThrust { get; } = new BaseAction(ActionID.WheelingThrust)
    {
        StatusNeed = new StatusID[] { StatusID.EnhancedWheelingThrust },
    };

    /// <summary>
    /// π·¥©º‚
    /// </summary>
    public static IBaseAction PiercingTalon { get; } = new BaseAction(ActionID.PiercingTalon);

    /// <summary>
    /// À¿ÃÏ«π
    /// </summary>
    public static IBaseAction DoomSpike { get; } = new BaseAction(ActionID.DoomSpike);

    /// <summary>
    /// “ÙÀŸ¥Ã
    /// </summary>
    public static IBaseAction SonicThrust { get; } = new BaseAction(ActionID.SonicThrust)
    {
        OtherIDsCombo = new[] { ActionID.DraconianFury }
    };

    /// <summary>
    /// …Ωæ≥ø·–Ã
    /// </summary>
    public static IBaseAction CoerthanTorment { get; } = new BaseAction(ActionID.CoerthanTorment);

    /// <summary>
    /// ∆∆ÀÈ≥Â
    /// </summary>
    public static IBaseAction SpineshatterDive { get; } = new BaseAction(ActionID.SpineshatterDive);

    /// <summary>
    /// ¡˙—◊≥Â
    /// </summary>
    public static IBaseAction DragonfireDive { get; } = new BaseAction(ActionID.DragonfireDive);

    /// <summary>
    /// Ã¯‘æ
    /// </summary>
    public static IBaseAction Jump { get; } = new BaseAction(ActionID.Jump)
    {
        StatusProvide = new StatusID[] { StatusID.DiveReady },
    };

    /// <summary>
    /// ∏ﬂÃ¯
    /// </summary>
    public static IBaseAction HighJump { get; } = new BaseAction(ActionID.HighJump)
    {
        StatusProvide = Jump.StatusProvide,
    };

    /// <summary>
    /// ª√œÛ≥Â
    /// </summary>
    public static IBaseAction MirageDive { get; } = new BaseAction(ActionID.MirageDive)
    {
        StatusNeed = Jump.StatusProvide,
    };

    /// <summary>
    /// Œ‰…Ò«π
    /// </summary>
    public static IBaseAction Geirskogul { get; } = new BaseAction(ActionID.Geirskogul);

    /// <summary>
    /// À¿’ﬂ÷Æ∞∂
    /// </summary>
    public static IBaseAction Nastrond { get; } = new BaseAction(ActionID.Nastrond)
    {
        ActionCheck = b => JobGauge.IsLOTDActive,
    };

    /// <summary>
    /// ◊π–«≥Â
    /// </summary>
    public static IBaseAction Stardiver { get; } = new BaseAction(ActionID.Stardiver)
    {
        ActionCheck = b => JobGauge.IsLOTDActive,
    };

    /// <summary>
    /// ÃÏ¡˙µ„æ¶
    /// </summary>
    public static IBaseAction WyrmwindThrust { get; } = new BaseAction(ActionID.WyrmwindThrust)
    {
        ActionCheck = b => JobGauge.FirstmindsFocusCount == 2,
    };

    /// <summary>
    /// ¡˙Ω£
    /// </summary>
    public static IBaseAction LifeSurge { get; } = new BaseAction(ActionID.LifeSurge, true)
    {
        StatusProvide = new[] { StatusID.LifeSurge },

        ActionCheck = b => !IsLastAbility(true, LifeSurge),
    };

    /// <summary>
    /// √Õ«π
    /// </summary>
    public static IBaseAction LanceCharge { get; } = new BaseAction(ActionID.LanceCharge, true);

    /// <summary>
    /// æﬁ¡˙ ”œﬂ
    /// </summary>
    public static IBaseAction DragonSight { get; } = new BaseAction(ActionID.DragonSight, true)
    {
        ChoiceTarget = (Targets, mustUse) =>
        {
            Targets = Targets.Where(b => b.ObjectId != Service.ClientState.LocalPlayer.ObjectId &&
            !b.HasStatus(false, StatusID.Weakness, StatusID.BrinkofDeath)).ToArray();

            if (Targets.Count() == 0) return Player;

            return Targets.GetJobCategory(JobRole.Melee, JobRole.RangedMagicial, JobRole.RangedPhysical, JobRole.Tank).FirstOrDefault();
        },
    };

    /// <summary>
    /// ’Ω∂∑¡¨µª
    /// </summary>
    public static IBaseAction BattleLitany { get; } = new BaseAction(ActionID.BattleLitany, true)
    {
        StatusNeed = new[] { StatusID.PowerSurge },
    };
}
