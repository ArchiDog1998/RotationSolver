using ECommons.DalamudServices;
using ECommons.ExcelServices;
using RotationSolver.Basic.Traits;

namespace RotationSolver.Basic.Rotations.Basic;

/// <summary>
/// The base class of SAM.
/// </summary>
public abstract class SAM_Base : CustomRotation
{
    /// <summary>
    /// 
    /// </summary>
    public override MedicineType MedicineType => MedicineType.Strength;

    /// <summary>
    /// 
    /// </summary>
    public sealed override Job[] Jobs => new [] { Job.SAM };

    /// <summary>
    /// 
    /// </summary>
    public static bool HasMoon => Player.HasStatus(true, StatusID.Fugetsu);

    /// <summary>
    /// 
    /// </summary>
    public static bool HasFlower => Player.HasStatus(true, StatusID.Fuka);

    /// <summary>
    /// 
    /// </summary>
    public static bool IsMoonTimeLessThanFlower
        => Player.StatusTime(true, StatusID.Fugetsu) < Player.StatusTime(true, StatusID.Fuka);

    #region JobGauge
    static SAMGauge JobGauge => Svc.Gauges.Get<SAMGauge>();

    /// <summary>
    /// 
    /// </summary>
    public static bool HasSetsu => JobGauge.HasSetsu;

    /// <summary>
    /// 
    /// </summary>
    public static bool HasGetsu => JobGauge.HasGetsu;

    /// <summary>
    /// 
    /// </summary>
    public static bool HasKa => JobGauge.HasKa;

    /// <summary>
    /// 
    /// </summary>
    public static byte Kenki => JobGauge.Kenki;

    /// <summary>
    /// 
    /// </summary>
    public static byte MeditationStacks => JobGauge.MeditationStacks;

    /// <summary>
    /// 
    /// </summary>
    public static byte SenCount => (byte)((HasGetsu ? 1 : 0) + (HasSetsu ? 1 : 0) + (HasKa ? 1 : 0));
    #endregion

    #region Attack Single
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Hakaze { get; } = new BaseAction(ActionID.Hakaze);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Jinpu { get; } = new BaseAction(ActionID.Jinpu);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Gekko { get; } = new BaseAction(ActionID.Gekko);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Shifu { get; } = new BaseAction(ActionID.Shifu);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Kasha { get; } = new BaseAction(ActionID.Kasha);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Yukikaze { get; } = new BaseAction(ActionID.Yukikaze);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Shoha { get; } = new BaseAction(ActionID.Shoha)
    {
        ActionCheck = (b, m) => MeditationStacks == 3
    };
    #endregion

    #region Attack Area
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Fuga { get; } = new BaseAction(ActionID.Fuga);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Fuko { get; } = new BaseAction(ActionID.Fuko);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Mangetsu { get; } = new BaseAction(ActionID.Mangetsu)
    {
        ComboIds = new[]
        {
            ActionID.Fuga,ActionID.Fuko
        }
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Oka { get; } = new BaseAction(ActionID.Oka)
    {
        ComboIds = new[]
        {
            ActionID.Fuga,ActionID.Fuko
        }
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Shoha2 { get; } = new BaseAction(ActionID.Shoha2)
    {
        ActionCheck = (b, m) => MeditationStacks == 3
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction OgiNamikiri { get; } = new BaseAction(ActionID.OgiNamikiri)
    {
        StatusNeed = new[] { StatusID.OgiNamikiriReady },
        ActionCheck = (b, m) => !IsMovingOrJumping
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction KaeshiNamikiri { get; } = new BaseAction(ActionID.KaeshiNamikiri)
    {
        ActionCheck = (b, m) => JobGauge.Kaeshi == Kaeshi.NAMIKIRI
    };
    #endregion

    #region Sen
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Higanbana { get; } = new BaseAction(ActionID.Higanbana, ActionOption.Dot)
    {
        ActionCheck = (b, m) => !IsMovingOrJumping && SenCount == 1,
        TargetStatus = new[] { StatusID.Higanbana },
        TimeToDie = 40,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction TenkaGoken { get; } = new BaseAction(ActionID.TenkaGoken)
    {
        ActionCheck = (b, m) => !IsMovingOrJumping && SenCount == 2,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction MidareSetsugekka { get; } = new BaseAction(ActionID.MidareSetsugekka)
    {
        ActionCheck = (b, m) => !IsMovingOrJumping && SenCount == 3,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction TsubameGaeshi { get; } = new BaseAction(ActionID.TsubameGaeshi);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction KaeshiGoken { get; } = new BaseAction(ActionID.KaeshiGoken)
    {
        ActionCheck = (b, m) => JobGauge.Kaeshi == Kaeshi.GOKEN
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction KaeshiSetsugekka { get; } = new BaseAction(ActionID.KaeshiSetsugekka)
    {
        ActionCheck = (b, m) => JobGauge.Kaeshi == Kaeshi.SETSUGEKKA
    };
    #endregion

    #region Range
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction ThirdEye { get; } = new BaseAction(ActionID.ThirdEye, ActionOption.Defense);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Enpi { get; } = new BaseAction(ActionID.Enpi)
    {
        FilterForHostiles = TargetFilter.MeleeRangeTargetFilter,
        ActionCheck = (b, m) => !IsLastAction(IActionHelper.MovingActions),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction MeikyoShisui { get; } = new BaseAction(ActionID.MeikyoShisui)
    {
        StatusProvide = new[] { StatusID.MeikyoShisui },
        ActionCheck = (b, m) => IsLongerThan(8),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Hagakure { get; } = new BaseAction(ActionID.Hagakure)
    {
        ActionCheck = (b, m) => SenCount > 0
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Ikishoten { get; } = new BaseAction(ActionID.Ikishoten)
    {
        StatusProvide = new[] { StatusID.OgiNamikiriReady },
        ActionCheck = (b, m) => InCombat
    };
    #endregion

    #region Kenki
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction HissatsuShinten { get; } = new BaseAction(ActionID.HissatsuShinten)
    {
        ActionCheck = (b, m) => Kenki >= 25
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction HissatsuGyoten { get; } = new BaseAction(ActionID.HissatsuGyoten)
    {
        ActionCheck = (b, m) => Kenki >= 10 && !Player.HasStatus(true, StatusID.Bind1, StatusID.Bind2),
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction HissatsuYaten { get; } = new BaseAction(ActionID.HissatsuYaten)
    {
        ActionCheck = HissatsuGyoten.ActionCheck
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction HissatsuKyuten { get; } = new BaseAction(ActionID.HissatsuKyuten)
    {
        ActionCheck = (b, m) => Kenki >= 25
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction HissatsuGuren { get; } = new BaseAction(ActionID.HissatsuGuren)
    {
        ActionCheck = HissatsuKyuten.ActionCheck,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction HissatsuSenei { get; } = new BaseAction(ActionID.HissatsuSenei)
    {
        ActionCheck = HissatsuKyuten.ActionCheck,
    };
    #endregion

    #region Traits
    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait KenkiMastery2 { get; } = new BaseTrait(208);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait KenkiMastery { get; } = new BaseTrait(215);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedIaijutsu { get; } = new BaseTrait(277);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedFugetsuAndFuka { get; } = new BaseTrait(278);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedTsubameGaeshi { get; } = new BaseTrait(442);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedMeikyoShisui    { get; } = new BaseTrait(443);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedIkishoten    { get; } = new BaseTrait(514);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait FugaMastery    { get; } = new BaseTrait(519);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait WayOfTheSamurai    { get; } = new BaseTrait(520);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait WayOfTheSamurai2    { get; } = new BaseTrait(521);
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(ActionID.HissatsuGyoten)]
    protected sealed override bool MoveForwardAbility(out IAction act)
    {
        if (HissatsuGyoten.CanUse(out act)) return true;
        return base.MoveForwardAbility(out act);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(ActionID.Feint)]
    protected sealed override bool DefenseAreaAbility(out IAction act)
    {
        if (Feint.CanUse(out act)) return true;
        return base.DefenseAreaAbility(out act);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    [RotationDesc(ActionID.ThirdEye)]
    protected override bool DefenseSingleAbility(out IAction act)
    {
        if (ThirdEye.CanUse(out act)) return true;
        return base.DefenseSingleAbility(out act);
    }
}