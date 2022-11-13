using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Linq;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.Basic;

internal abstract class RDMCombo_Base<TCmd> : JobGaugeCombo<RDMGauge, TCmd> where TCmd : Enum
{

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.RedMage };
    protected override bool CanHealSingleSpell => TargetUpdater.PartyMembers.Length == 1 && base.CanHealSingleSpell;
    //¿´¿´ÏÖÔÚÓÐÃ»ÓÐ´Ù½ø

    private sealed protected override BaseAction Raise => Verraise;

    /// <summary>
    /// ³à¸´»î
    /// </summary>
    public static BaseAction Verraise { get; } = new(ActionID.Verraise, true);

    /// <summary>
    /// Õðµ´
    /// </summary>
    public static BaseAction Jolt { get; } = new(ActionID.Jolt)
    {
        BuffsProvide = Swiftcast.BuffsProvide.Union(new[] { StatusID.Acceleration }).ToArray(),
    };

    /// <summary>
    /// »Ø´Ì
    /// </summary>
    public static BaseAction Riposte { get; } = new(ActionID.Riposte)
    {
        OtherCheck = b => JobGauge.BlackMana >= 20 && JobGauge.WhiteMana >= 20,
    };

    /// <summary>
    /// ³àÉÁÀ×
    /// </summary>
    public static BaseAction Verthunder { get; } = new(ActionID.Verthunder)
    {
        BuffsNeed = Jolt.BuffsProvide,
    };

    /// <summary>
    /// ¶Ì±øÏà½Ó
    /// </summary>
    public static BaseAction CorpsAcorps { get; } = new(ActionID.CorpsAcorps, shouldEndSpecial: true)
    {
        BuffsProvide = new[]
        {
                 StatusID.Bind1,
                 StatusID.Bind2,
            }
    };

    /// <summary>
    /// ³à¼²·ç
    /// </summary>
    public static BaseAction Veraero { get; } = new(ActionID.Veraero)
    {
        BuffsNeed = Jolt.BuffsProvide,
    };

    /// <summary>
    /// É¢Ëé
    /// </summary>
    public static BaseAction Scatter { get; } = new(ActionID.Scatter)
    {
        BuffsNeed = Jolt.BuffsProvide,
    };

    /// <summary>
    /// ³àÕðÀ×
    /// </summary>
    public static BaseAction Verthunder2 { get; } = new(ActionID.Verthunder2)
    {
        BuffsProvide = Jolt.BuffsProvide,
    };

    /// <summary>
    /// ³àÁÒ·ç
    /// </summary>
    public static BaseAction Veraero2 { get; } = new(ActionID.Veraero2)
    {
        BuffsProvide = Jolt.BuffsProvide,
    };

    /// <summary>
    /// ³à»ðÑ×
    /// </summary>
    public static BaseAction Verfire { get; } = new(ActionID.Verfire)
    {
        BuffsNeed = new[] { StatusID.VerfireReady },
        BuffsProvide = Jolt.BuffsProvide,
    };

    /// <summary>
    /// ³à·ÉÊ¯
    /// </summary>
    public static BaseAction Verstone { get; } = new(ActionID.Verstone)
    {
        BuffsNeed = new[] { StatusID.VerstoneReady },
        BuffsProvide = Jolt.BuffsProvide,
    };

    /// <summary>
    /// ½»»÷Õ¶
    /// </summary>
    public static BaseAction Zwerchhau { get; } = new(ActionID.Zwerchhau)
    {
        OtherCheck = b => JobGauge.BlackMana >= 15 && JobGauge.WhiteMana >= 15,
    };

    /// <summary>
    /// ½»½£
    /// </summary>
    public static BaseAction Engagement { get; } = new(ActionID.Engagement);

    /// <summary>
    /// ·É½£
    /// </summary>
    public static BaseAction Fleche { get; } = new(ActionID.Fleche);

    /// <summary>
    /// Á¬¹¥
    /// </summary>
    public static BaseAction Redoublement { get; } = new(ActionID.Redoublement)
    {
        OtherCheck = b => JobGauge.BlackMana >= 15 && JobGauge.WhiteMana >= 15,
    };


    /// <summary>
    /// ´Ù½ø
    /// </summary>
    public static BaseAction Acceleration { get; } = new(ActionID.Acceleration)
    {
        BuffsProvide = new[] { StatusID.Acceleration },
    };

    /// <summary>
    /// »®Ô²Õ¶
    /// </summary>
    public static BaseAction Moulinet { get; } = new(ActionID.Moulinet)
    {
        OtherCheck = b => JobGauge.BlackMana >= 20 && JobGauge.WhiteMana >= 20,
    };

    /// <summary>
    /// ³àÖÎÁÆ
    /// </summary>
    public static BaseAction Vercure { get; } = new(ActionID.Vercure, true)
    {
        BuffsProvide = Swiftcast.BuffsProvide.Union(Acceleration.BuffsProvide).ToArray(),
    };

    /// <summary>
    /// Áù·Ö·´»÷
    /// </summary>
    public static BaseAction ContreSixte { get; } = new(ActionID.ContreSixte);

    /// <summary>
    /// ¹ÄÀø
    /// </summary>
    public static BaseAction Embolden { get; } = new(ActionID.Embolden, true);

    /// <summary>
    /// ÐøÕ¶
    /// </summary>
    public static BaseAction Reprise { get; } = new(ActionID.Reprise);

    /// <summary>
    /// ¿¹ËÀ
    /// </summary>
    public static BaseAction MagickBarrier { get; } = new(ActionID.MagickBarrier);

    /// <summary>
    /// ³àºË±¬
    /// </summary>
    public static BaseAction Verflare { get; } = new(ActionID.Verflare);

    /// <summary>
    /// ³àÉñÊ¥
    /// </summary>
    public static BaseAction Verholy { get; } = new(ActionID.Verholy);

    /// <summary>
    /// ½¹ÈÈ
    /// </summary>
    public static BaseAction Scorch { get; } = new(ActionID.Scorch)
    {
        OtherIDsCombo = new uint[] { Verholy.ID },
    };

    /// <summary>
    /// ¾ö¶Ï
    /// </summary>
    public static BaseAction Resolution { get; } = new(ActionID.Resolution);

    /// <summary>
    /// Ä§Ôª»¯
    /// </summary>
    public static BaseAction Manafication { get; } = new(ActionID.Manafication)
    {
        OtherCheck = b => JobGauge.WhiteMana <= 50 && JobGauge.BlackMana <= 50 && InCombat && JobGauge.ManaStacks == 0,
        OtherIDsNot = new uint[] { Riposte.ID, Zwerchhau.ID, Scorch.ID, Verflare.ID, Verholy.ID },
    };
}
