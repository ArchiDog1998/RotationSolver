namespace RotationSolver.Basic.Rotations.Basic;

public abstract class SAM_Base : CustomRotation
{
    private static SAMGauge JobGauge => Service.JobGauges.Get<SAMGauge>();
    public override MedicineType MedicineType => MedicineType.Strength;

    protected static bool HasSetsu => JobGauge.HasSetsu;

    protected static bool HasGetsu => JobGauge.HasGetsu;

    protected static bool HasKa => JobGauge.HasKa;

    protected static byte Kenki => JobGauge.Kenki;

    protected static byte MeditationStacks => JobGauge.MeditationStacks;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Samurai };

    protected static byte SenCount => (byte)((HasGetsu ? 1 : 0) + (HasSetsu ? 1 : 0) + (HasKa ? 1 : 0));

    protected static bool HaveMoon => Player.HasStatus(true, StatusID.Fugetsu);
    protected static float MoonTime => Player.StatusTime(true, StatusID.Fugetsu);
    protected static bool HaveFlower => Player.HasStatus(true, StatusID.Fuka);
    protected static float FlowerTime => Player.StatusTime(true, StatusID.Fuka);

    #region Single
    public static IBaseAction Hakaze { get; } = new BaseAction(ActionID.Hakaze);

    public static IBaseAction Jinpu { get; } = new BaseAction(ActionID.Jinpu);

    public static IBaseAction Gekko { get; } = new BaseAction(ActionID.Gekko);

    public static IBaseAction Shifu { get; } = new BaseAction(ActionID.Shifu);

    public static IBaseAction Kasha { get; } = new BaseAction(ActionID.Kasha);

    public static IBaseAction Yukikaze { get; } = new BaseAction(ActionID.Yukikaze);

    public static IBaseAction Shoha { get; } = new BaseAction(ActionID.Shoha)
    {
        ActionCheck = b => MeditationStacks == 3
    };
    #endregion

    #region AoE
    public static IBaseAction Fuga { get; } = new BaseAction(ActionID.Fuga);

    public static IBaseAction Fuko { get; } = new BaseAction(ActionID.Fuko);

    public static IBaseAction Mangetsu { get; } = new BaseAction(ActionID.Mangetsu)
    {
        ComboIds = new[]
        {
            ActionID.Fuga,ActionID.Fuko
        }
    };

    public static IBaseAction Oka { get; } = new BaseAction(ActionID.Oka)
    {
        ComboIds = new[]
        {
            ActionID.Fuga,ActionID.Fuko
        }
    };

    public static IBaseAction Shoha2 { get; } = new BaseAction(ActionID.Shoha2)
    {
        ActionCheck = b => MeditationStacks == 3
    };

    public static IBaseAction OgiNamikiri { get; } = new BaseAction(ActionID.OgiNamikiri)
    {
        StatusNeed = new[] { StatusID.OgiNamikiriReady },
        ActionCheck = b => !IsMoving
    };

    public static IBaseAction KaeshiNamikiri { get; } = new BaseAction(ActionID.KaeshiNamikiri)
    {
        ActionCheck = b => JobGauge.Kaeshi == Dalamud.Game.ClientState.JobGauge.Enums.Kaeshi.NAMIKIRI
    };
    #endregion

    #region Sen
    public static IBaseAction Higanbana { get; } = new BaseAction(ActionID.Higanbana, isEot: true)
    {
        ActionCheck = b => !IsMoving && SenCount == 1,
        TargetStatus = new[] { StatusID.Higanbana },
    };

    public static IBaseAction TenkaGoken { get; } = new BaseAction(ActionID.TenkaGoken)
    {
        ActionCheck = b => !IsMoving && SenCount == 2,
    };

    public static IBaseAction MidareSetsugekka { get; } = new BaseAction(ActionID.MidareSetsugekka)
    {
        ActionCheck = b => !IsMoving && SenCount == 3,
    };
    public static IBaseAction TsubameGaeshi { get; } = new BaseAction(ActionID.TsubameGaeshi);

    public static IBaseAction KaeshiGoken { get; } = new BaseAction(ActionID.KaeshiGoken)
    {
        ActionCheck = b => JobGauge.Kaeshi == Kaeshi.GOKEN
    };

    public static IBaseAction KaeshiSetsugekka { get; } = new BaseAction(ActionID.KaeshiSetsugekka)
    {
        ActionCheck = b => JobGauge.Kaeshi == Kaeshi.SETSUGEKKA
    };
    #endregion

    #region Range
    public static IBaseAction ThirdEye { get; } = new BaseAction(ActionID.ThirdEye, true, isTimeline: true);

    public static IBaseAction Enpi { get; } = new BaseAction(ActionID.Enpi)
    {
        FilterForHostiles = TargetFilter.MeleeRangeTargetFilter,
        ActionCheck = b => !IsLastAction(IActionHelper.MovingActions),
    };

    public static IBaseAction MeikyoShisui { get; } = new BaseAction(ActionID.MeikyoShisui)
    {
        StatusProvide = new[] { StatusID.MeikyoShisui },
    };

    public static IBaseAction Hagakure { get; } = new BaseAction(ActionID.Hagakure)
    {
        ActionCheck = b => SenCount > 0
    };

    public static IBaseAction Ikishoten { get; } = new BaseAction(ActionID.Ikishoten)
    {
        StatusProvide = new[] { StatusID.OgiNamikiriReady },
        ActionCheck = b => InCombat
    };
    #endregion

    #region Kenki
    public static IBaseAction HissatsuShinten { get; } = new BaseAction(ActionID.HissatsuShinten)
    {
        ActionCheck = b => Kenki >= 25
    };

    public static IBaseAction HissatsuGyoten { get; } = new BaseAction(ActionID.HissatsuGyoten)
    {
        ActionCheck = b => Kenki >= 10 && !Player.HasStatus(true, StatusID.Bind1, StatusID.Bind2)
    };

    public static IBaseAction HissatsuYaten { get; } = new BaseAction(ActionID.HissatsuYaten)
    {
        ActionCheck = HissatsuGyoten.ActionCheck
    };

    public static IBaseAction HissatsuKyuten { get; } = new BaseAction(ActionID.HissatsuKyuten)
    {
        ActionCheck = b => Kenki >= 25
    };

    public static IBaseAction HissatsuGuren { get; } = new BaseAction(ActionID.HissatsuGuren)
    {
        ActionCheck = b => Kenki >= 25
    };

    public static IBaseAction HissatsuSenei { get; } = new BaseAction(ActionID.HissatsuSenei)
    {
        ActionCheck = b => Kenki >= 25
    };
    #endregion

    [RotationDesc(ActionID.HissatsuGyoten)]
    protected sealed override bool MoveForwardAbility(byte abilitiesRemaining, out IAction act, CanUseOption option = CanUseOption.None)
    {
        if (HissatsuGyoten.CanUse(out act, CanUseOption.EmptyOrSkipCombo | option)) return true;
        return false;
    }

    [RotationDesc(ActionID.Feint)]
    protected sealed override bool DefenseAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        if (Feint.CanUse(out act)) return true;
        return false;
    }

    [RotationDesc(ActionID.ThirdEye)]
    protected override bool DefenseSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        if (ThirdEye.CanUse(out act)) return true;
        return false;
    }
}