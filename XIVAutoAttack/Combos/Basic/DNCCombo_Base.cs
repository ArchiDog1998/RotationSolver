using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.Basic;
internal abstract class DNCCombo_Base<TCmd> : CustomCombo<TCmd> where TCmd : Enum
{
    private static DNCGauge JobGauge => Service.JobGauges.Get<DNCGauge>();

    /// <summary>
    /// ÕýÔÚÌøÎè
    /// </summary>
    protected static bool IsDancing => JobGauge.IsDancing;

    /// <summary>
    /// ÁæÀþ
    /// </summary>
    protected static byte Esprit => JobGauge.Esprit;

    /// <summary>
    /// »ÃÉÈÊý
    /// </summary>
    protected static byte Feathers => JobGauge.Feathers;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Dancer };

    /// <summary>
    /// ÆÙÐº
    /// </summary>
    public static BaseAction Cascade { get; } = new(ActionID.Cascade)
    {
        BuffsProvide = new[] { StatusID.SilkenSymmetry }
    };

    /// <summary>
    /// ÅçÈª
    /// </summary>
    public static BaseAction Fountain { get; } = new(ActionID.Fountain)
    {
        BuffsProvide = new[] { StatusID.SilkenFlow }
    };

    /// <summary>
    /// ÄæÆÙÐº
    /// </summary>
    public static BaseAction ReverseCascade { get; } = new(ActionID.ReverseCascade)
    {
        BuffsNeed = new[] { StatusID.SilkenSymmetry, StatusID.SilkenSymmetry2 },
    };

    /// <summary>
    /// ×¹ÅçÈª
    /// </summary>
    public static BaseAction Fountainfall { get; } = new(ActionID.Fountainfall)
    {
        BuffsNeed = new[] { StatusID.SilkenFlow, StatusID.SilkenFlow2 }
    };

    /// <summary>
    /// ÉÈÎè¡¤Ðò
    /// </summary>
    public static BaseAction FanDance { get; } = new(ActionID.FanDance)
    {
        ActionCheck = b => JobGauge.Feathers > 0,
        BuffsProvide = new[] { StatusID.ThreefoldFanDance },
    };

    /// <summary>
    /// ·ç³µ
    /// </summary>
    public static BaseAction Windmill { get; } = new(ActionID.Windmill)
    {
        BuffsProvide = Cascade.BuffsProvide,
    };

    /// <summary>
    /// ÂäÈÐÓê
    /// </summary>
    public static BaseAction Bladeshower { get; } = new(ActionID.Windmill)
    {
        BuffsProvide = Fountain.BuffsProvide,
    };

    /// <summary>
    /// Éý·ç³µ
    /// </summary>
    public static BaseAction RisingWindmill { get; } = new(ActionID.RisingWindmill)
    {
        BuffsNeed = ReverseCascade.BuffsNeed,
    };

    /// <summary>
    /// ÂäÑªÓê
    /// </summary>
    public static BaseAction Bloodshower { get; } = new(ActionID.Bloodshower)
    {
        BuffsNeed = Fountainfall.BuffsNeed,
    };

    /// <summary>
    /// ÉÈÎè¡¤ÆÆ
    /// </summary>
    public static BaseAction FanDance2 { get; } = new(ActionID.FanDance2)
    {
        ActionCheck = b => Feathers > 0,
        BuffsProvide = new[] { StatusID.ThreefoldFanDance },
    };

    /// <summary>
    /// ÉÈÎè¡¤¼±
    /// </summary>
    public static BaseAction FanDance3 { get; } = new(ActionID.FanDance3)
    {
        BuffsNeed = FanDance2.BuffsProvide,
    };

    /// <summary>
    /// ÉÈÎè¡¤ÖÕ
    /// </summary>
    public static BaseAction FanDance4 { get; } = new(ActionID.FanDance4)
    {
        BuffsNeed = new[] { StatusID.FourfoldFanDance },
    };

    /// <summary>
    /// ½£Îè
    /// </summary>
    public static BaseAction SaberDance { get; } = new(ActionID.SaberDance)
    {
        ActionCheck = b => Esprit >= 50,
    };

    /// <summary>
    /// Á÷ÐÇÎè
    /// </summary>
    public static BaseAction StarfallDance { get; } = new(ActionID.StarfallDance)
    {
        BuffsNeed = new[] { StatusID.FlourishingStarfall },
    };

    /// <summary>
    /// Ç°³å²½
    /// </summary>
    public static BaseAction EnAvant { get; } = new(ActionID.EnAvant, true, shouldEndSpecial: true);

    /// <summary>
    /// Ç¾Þ±Çú½Å²½
    /// </summary>
    public static BaseAction Emboite { get; } = new(ActionID.Emboite, true)
    {
        ActionCheck = b => (ActionID)JobGauge.NextStep == ActionID.Emboite,
    };

    /// <summary>
    /// Ð¡Äñ½»µþÌø
    /// </summary>
    public static BaseAction Entrechat { get; } = new(ActionID.Entrechat, true)
    {
        ActionCheck = b => (ActionID)JobGauge.NextStep == ActionID.Entrechat,
    };

    /// <summary>
    /// ÂÌÒ¶Ð¡ÌßÍÈ
    /// </summary>
    public static BaseAction Jete { get; } = new(ActionID.Jete, true)
    {
        ActionCheck = b => (ActionID)JobGauge.NextStep == ActionID.Jete,
    };

    /// <summary>
    /// ½ð¹ÚÖº¼â×ª
    /// </summary>
    public static BaseAction Pirouette { get; } = new(ActionID.Pirouette, true)
    {
        ActionCheck = b => (ActionID)JobGauge.NextStep == ActionID.Pirouette,
    };

    /// <summary>
    /// ±ê×¼Îè²½
    /// </summary>
    public static BaseAction StandardStep { get; } = new(ActionID.StandardStep)
    {
        BuffsProvide = new[]
         {
                StatusID.StandardStep,
                StatusID.TechnicalStep,
            },
    };

    /// <summary>
    /// ¼¼ÇÉÎè²½
    /// </summary>
    public static BaseAction TechnicalStep { get; } = new(ActionID.TechnicalStep)
    {
        BuffsNeed = new[]
         {
                StatusID.StandardFinish,
            },
        BuffsProvide = StandardStep.BuffsProvide,
    };

    /// <summary>
    /// ·ÀÊØÖ®É£°Í
    /// </summary>
    public static BaseAction ShieldSamba { get; } = new(ActionID.ShieldSamba, true)
    {
        ActionCheck = b => !Player.HasStatus(false, StatusID.Troubadour,
            StatusID.Tactician1,
            StatusID.Tactician2,
            StatusID.ShieldSamba),
    };

    /// <summary>
    /// ÖÎÁÆÖ®»ª¶û×È
    /// </summary>
    public static BaseAction CuringWaltz { get; } = new(ActionID.CuringWaltz, true);

    /// <summary>
    /// ±ÕÊ½Îè×Ë
    /// </summary>
    public static BaseAction ClosedPosition { get; } = new(ActionID.ClosedPosition, true)
    {
        ChoiceTarget = Targets =>
        {
            Targets = Targets.Where(b => b.ObjectId != Player.ObjectId && b.CurrentHp != 0 &&
            //Remove Weak
            !b.HasStatus(false, StatusID.Weakness, StatusID.BrinkofDeath)
            //Remove other partner.
            && (!b.HasStatus(false, StatusID.ClosedPosition2) | b.HasStatus(true, StatusID.ClosedPosition2)) 
            ).ToArray();

            return Targets.GetJobCategory(JobRole.Tank, JobRole.RangedMagicial, JobRole.RangedPhysical).FirstOrDefault();
        },
    };

    /// <summary>
    /// ½ø¹¥Ö®Ì½¸ê
    /// </summary>
    public static BaseAction Devilment { get; } = new(ActionID.Devilment, true);

    /// <summary>
    /// °Ù»¨ÕùÑÞ
    /// </summary>
    public static BaseAction Flourish { get; } = new(ActionID.Flourish, true)
    {
        BuffsNeed = new[] { StatusID.StandardFinish },
        BuffsProvide = new[]
        {
                 StatusID.ThreefoldFanDance,
                 StatusID.FourfoldFanDance,
            },
        ActionCheck = b => InCombat,
    };

    /// <summary>
    /// ¼´ÐË±íÑÝ
    /// </summary>
    public static BaseAction Improvisation { get; } = new(ActionID.Improvisation, true);

    /// <summary>
    /// ÌáÀ­ÄÉ
    /// </summary>
    public static BaseAction Tillana { get; } = new(ActionID.Tillana)
    {
        BuffsNeed = new[] { StatusID.FlourishingFinish },
    };

    protected bool FinishStepGCD(out IAction act)
    {
        act = null;
        if (!Player.HasStatus(true, StatusID.StandardStep, StatusID.TechnicalStep)) return false;

        if (Player.HasStatus(true, StatusID.StandardStep) && JobGauge.CompletedSteps == 2)
        {
            act = StandardStep;
            return true;
        }
        else if (Player.HasStatus(true, StatusID.TechnicalStep) && JobGauge.CompletedSteps == 4)
        {
            act = TechnicalStep;
            return true;
        }
        else
        {
            if (Emboite.ShouldUse(out act)) return true;
            if (Entrechat.ShouldUse(out act)) return true;
            if (Jete.ShouldUse(out act)) return true;
            if (Pirouette.ShouldUse(out act)) return true;
        }

        return false;
    }
}
