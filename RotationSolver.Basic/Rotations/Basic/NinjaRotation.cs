namespace RotationSolver.Basic.Rotations.Basic;

partial class NinjaRotation
{
    /// <inheritdoc/>
    public override MedicineType MedicineType => MedicineType.Dexterity;

    #region Job Gauge
    /// <summary>
    /// 
    /// </summary>
    public static byte Ninki => JobGauge.Ninki;

    static float HutonTimeRaw => JobGauge.HutonTimer / 1000f;

    /// <summary>
    /// 
    /// </summary>
    public static float HutonTime => HutonTimeRaw - DataCenter.WeaponRemain;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected static bool HutonEndAfter(float time) => HutonTime <= time;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gctCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    protected static bool HutonEndAfterGCD(uint gctCount = 0, float offset = 0)
        => HutonEndAfter(GCDTime(gctCount, offset));
    #endregion

    static partial void ModifyArmorCrushPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => HutonEndAfter(25) && !HutonEndAfterGCD();
    }

    static partial void ModifyHuraijinPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => HutonEndAfterGCD();
    }

    static partial void ModifyPhantomKamaitachiPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.PhantomKamaitachiReady];
    }

    static partial void ModifyThrowingDaggerPvE(ref ActionSetting setting)
    {
        setting.IsMeleeRange = true;
    }

    static partial void ModifyBhavacakraPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Ninki >= 50;
    }

    static partial void ModifyHellfrogMediumPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Ninki >= 50;
    }

    static partial void ModifyMeisuiPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Suiton];
        setting.ActionCheck = () => Ninki <= 50;
    }

    static partial void ModifyMugPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => JobGauge.Ninki <= 60 && IsLongerThan(10);
    }

    static partial void ModifyTrickAttackPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.Suiton, StatusID.Hidden];
        setting.CreateConfig = () => new ActionConfig()
        {
            TimeToKill = 10,
        };
    }

    static partial void ModifyBunshinPvE(ref ActionSetting setting)
    {
        setting.ActionCheck = () => Ninki >= 50;
    }

    static partial void ModifyTenChiJinPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.Kassatsu, StatusID.TenChiJin];
        setting.ActionCheck = () => !HutonEndAfterGCD(2);
    }

    static partial void ModifyKassatsuPvE(ref ActionSetting setting)
    {
        setting.StatusProvide = [StatusID.Kassatsu, StatusID.TenChiJin];
    }

    static partial void ModifyFumaShurikenPvE(ref ActionSetting setting)
    {
        setting.Ninjutsu = [ActionID.TenPvE];
    }

    static partial void ModifyKatonPvE(ref ActionSetting setting)
    {
        setting.Ninjutsu = [ActionID.ChiPvE, ActionID.TenPvE];
    }

    static partial void ModifyRaitonPvE(ref ActionSetting setting)
    {
        setting.Ninjutsu = [ActionID.TenPvE, ActionID.ChiPvE];
    }

    static partial void ModifyHyotonPvE(ref ActionSetting setting)
    {
        setting.Ninjutsu = [ActionID.TenPvE, ActionID.JinPvE];
    }

    static partial void ModifyHutonPvE(ref ActionSetting setting)
    {
        setting.Ninjutsu = [ActionID.JinPvE, ActionID.ChiPvE, ActionID.TenPvE];
        setting.ActionCheck = () => HutonEndAfterGCD();
    }

    static partial void ModifyDotonPvE(ref ActionSetting setting)
    {
        setting.Ninjutsu = [ActionID.JinPvE, ActionID.TenPvE, ActionID.ChiPvE];
        setting.StatusProvide = [StatusID.Doton];
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.ShukuchiPvE)]
    protected sealed override bool MoveForwardAbility(out IAction? act)
    {
        if (ShukuchiPvE.CanUse(out act)) return true;
        return base.MoveForwardAbility(out act);
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.FeintPvE)]
    protected sealed override bool DefenseAreaAbility(out IAction? act)
    {
        if (FeintPvE.CanUse(out act)) return true;
        return base.DefenseAreaAbility(out act);
    }

    /// <inheritdoc/>
    [RotationDesc(ActionID.ShadeShiftPvE)]
    protected override bool DefenseSingleAbility(out IAction? act)
    {
        if (ShadeShiftPvE.CanUse(out act)) return true;
        return base.DefenseSingleAbility(out act);
    }

    static partial void ModifySuitonPvE(ref ActionSetting setting)
    {
        setting.Ninjutsu = [ActionID.TenPvE, ActionID.ChiPvE, ActionID.JinPvE];
        setting.StatusProvide = [StatusID.Suiton];
    }

    static partial void ModifyGokaMekkyakuPvE(ref ActionSetting setting)
    {
        setting.Ninjutsu = [ActionID.ChiPvE, ActionID.TenPvE];
    }

    static partial void ModifyHyoshoRanryuPvE(ref ActionSetting setting)
    {
        setting.Ninjutsu = [ActionID.TenPvE, ActionID.JinPvE];
    }

    static partial void ModifyFleetingRaijuPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.RaijuReady];
    }

    static partial void ModifyForkedRaijuPvE(ref ActionSetting setting)
    {
        setting.StatusNeed = [StatusID.RaijuReady];
    }
}


///// <summary>
///// The base class of Nin.
///// </summary>
//public abstract class NIN_Base : CustomRotation
//{
//    #region Ninjutsu

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBaseAction FumaShurikenTen { get; } = new BaseAction(ActionID.FumaShuriken_18873);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBaseAction FumaShurikenJin { get; } = new BaseAction(ActionID.FumaShuriken_18875);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBaseAction KatonTen { get; } = new BaseAction(ActionID.Katon_18876);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBaseAction RaitonChi { get; } = new BaseAction(ActionID.Raiton_18877);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBaseAction DotonChi { get; } = new BaseAction(ActionID.Doton_18880);

//    /// <summary>
//    /// 
//    /// </summary>
//    public static IBaseAction SuitonJin { get; } = new BaseAction(ActionID.Suiton_18881);

//    /// <summary>
//    /// 
//    /// </summary>
//    public class NinAction : BaseAction, INinAction
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        public IBaseAction[] Ninjutsu { get; }

//        internal NinAction(ActionID actionID, params IBaseAction[] ninjutsu)
//            : base(actionID)
//        {
//            Ninjutsu = ninjutsu;
//        }
//    }
//    #endregion
//}
