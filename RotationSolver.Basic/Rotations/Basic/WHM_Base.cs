using Dalamud.Game.ClientState.Objects.SubKinds;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using RotationSolver.Basic.Traits;

namespace RotationSolver.Basic.Rotations.Basic;

/// <summary>
/// The base class of WHM
/// </summary>
public abstract class WHM_Base : CustomRotation
{
    /// <summary>
    /// 
    /// </summary>
    public sealed override Job[] Jobs => new Job[] { Job.WHM, Job.CNJ };

    /// <summary>
    /// 
    /// </summary>
    public override MedicineType MedicineType => MedicineType.Mind;

    #region Job Gauge
    private static WHMGauge JobGauge => Svc.Gauges.Get<WHMGauge>();

    /// <summary>
    /// 
    /// </summary>
    public static byte Lily => JobGauge.Lily;

    /// <summary>
    /// 
    /// </summary>
    public static byte BloodLily => JobGauge.BloodLily;

    static float LilyTimeRaw => JobGauge.LilyTimer / 1000f;

    /// <summary>
    /// 
    /// </summary>
    public static float LilyTime => LilyTimeRaw - DataCenter.WeaponRemain;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected static bool LilyAfter(float time) => LilyTime <= time;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gcdCount"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    protected static bool LilyAfterGCD(uint gcdCount = 0, float offset = 0)
        => LilyAfter(GCDTime(gcdCount, offset));
    #endregion

    #region Heal
    private protected sealed override IBaseAction Raise => Raise1;

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Raise1 { get; } = new BaseAction(ActionID.Raise, ActionOption.Friendly);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Cure { get; } = new BaseAction(ActionID.Cure, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Medica { get; } = new BaseAction(ActionID.Medica, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Cure2 { get; } = new BaseAction(ActionID.CureIi, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Medica2 { get; } = new BaseAction(ActionID.MedicaIi, ActionOption.Hot)
    {
        StatusProvide = [StatusID.MedicaIi, StatusID.TrueMedicaIi],
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Regen { get; } = new BaseAction(ActionID.Regen, ActionOption.Hot)
    {
        TargetStatus =
        [
            StatusID.Regen,
            StatusID.Regen_897,
            StatusID.Regen_1330,
        ]
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Cure3 { get; } = new BaseAction(ActionID.CureIii, ActionOption.Heal | ActionOption.EndSpecial);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Benediction { get; } = new BaseAction(ActionID.Benediction, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Asylum { get; } = new BaseAction(ActionID.Asylum, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction AfflatusSolace { get; } = new BaseAction(ActionID.AfflatusSolace, ActionOption.Heal | ActionOption.UseResources)
    {
        ActionCheck = (b, m) => Lily > 0,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Tetragrammaton { get; } = new BaseAction(ActionID.Tetragrammaton, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction DivineBenison { get; } = new BaseAction(ActionID.DivineBenison, ActionOption.Defense)
    {
        StatusProvide = [StatusID.DivineBenison],
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction AfflatusRapture { get; } = new BaseAction(ActionID.AfflatusRapture, ActionOption.Heal | ActionOption.UseResources)
    {
        ActionCheck = (b, m) => Lily > 0,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Aquaveil { get; } = new BaseAction(ActionID.Aquaveil, ActionOption.Defense)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction LiturgyOfTheBell { get; } = new BaseAction(ActionID.LiturgyOfTheBell, ActionOption.Heal);
    #endregion

    #region Attack
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Stone { get; } = new BaseAction(ActionID.Stone);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Aero { get; } = new BaseAction(ActionID.Aero, ActionOption.Dot)
    {
        TargetStatus =
        [
            StatusID.Aero,
            StatusID.AeroIi,
            StatusID.Dia,
        ],
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Holy { get; } = new BaseAction(ActionID.Holy);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Assize { get; } = new BaseAction(ActionID.Assize);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction AfflatusMisery { get; } = new BaseAction(ActionID.AfflatusMisery, ActionOption.UseResources)
    {
        ActionCheck = (b, m) => BloodLily == 3,
    };
    #endregion

    #region buff
    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PresenceOfMind { get; } = new BaseAction(ActionID.PresenceOfMind, ActionOption.Buff)
    {
        ActionCheck = (b, m) => !IsMoving && IsLongerThan(10),
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction ThinAir { get; } = new BaseAction(ActionID.ThinAir, ActionOption.Buff);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PlenaryIndulgence { get; } = new BaseAction(ActionID.PlenaryIndulgence, ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction Temperance { get; } = new BaseAction(ActionID.Temperance, ActionOption.Heal);
    #endregion

    #region Traits
    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait MaimAndMend { get; } = new BaseTrait(23);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait Freecure { get; } = new BaseTrait(25);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait MaimAndMend2 { get; } = new BaseTrait(26);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait StoneMastery { get; } = new BaseTrait(179);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait AeroMastery { get; } = new BaseTrait(180);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait StoneMastery2 { get; } = new BaseTrait(181);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait StoneMastery3 { get; } = new BaseTrait(182);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait SecretOfTheLily { get; } = new BaseTrait(196);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait AeroMastery2 { get; } = new BaseTrait(307);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait StoneMastery4 { get; } = new BaseTrait(308);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait TranscendentAfflatus { get; } = new BaseTrait(309);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedAsylum { get; } = new BaseTrait(310);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait GlareMastery { get; } = new BaseTrait(487);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait HolyMastery { get; } = new BaseTrait(488);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedHealingMagic { get; } = new BaseTrait(489);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseTrait EnhancedDivineBenison { get; } = new BaseTrait(490);
    #endregion
    
    #region PvP

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_Glare3 { get; } = new BaseAction(ActionID.GlareIii_29223);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_Cure2 { get; } = new BaseAction(ActionID.CureIi_29224,ActionOption.Heal);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_AfflatusMisery { get; } = new BaseAction(ActionID.AfflatusMisery_29226);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_Aquaveil { get; } = new BaseAction(ActionID.Aquaveil_29227,ActionOption.Defense);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_MiracleOfNature { get; } = new BaseAction(ActionID.MiracleOfNature);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_SeraphStrike { get; } = new BaseAction(ActionID.SeraphStrike);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_Cure3 { get; } = new BaseAction(ActionID.CureIii_29225,ActionOption.Heal)
    {
        StatusNeed = [StatusID.CureIiiReady],
    };

    /// <summary>
    /// 
    /// </summary>
    public static IBaseAction PvP_AfflatusPurgation { get; } = new BaseAction(ActionID.AfflatusPurgation)
    {
        FilterForHostiles = tars => tars.Where(t => t is PlayerCharacter),
        ActionCheck = (t, m) => LimitBreakLevel >= 1,
    };

    #endregion

    private protected override IBaseAction LimitBreak => PulseOfLife;

    /// <summary>
    /// LB
    /// </summary>
    public static IBaseAction PulseOfLife { get; } = new BaseAction(ActionID.PulseOfLife, ActionOption.Heal)
    {
        ActionCheck = (b, m) => LimitBreakLevel == 3,
    };
}