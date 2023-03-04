using Dalamud.Game.ClientState.Objects.Types;
using RotationSolver.Data;
using System;

namespace RotationSolver.Actions
{
    internal interface IBaseAction : IAction
    {
        /// <summary>
        /// MP for casting.
        /// </summary>
        uint MPNeed { get; }

        /// <summary>
        /// Casting time
        /// </summary>
        float CastTime { get; }

        /// <summary>
        /// If combo id is on this list, this aciton will not used.
        /// </summary>
        ActionID[] ComboIdsNot { set; }

        /// <summary>
        /// The combos that are not written on the action list.
        /// </summary>
        ActionID[] ComboIds { set; }

        /// <summary>
        /// If player has these statuses from player self, this aciton will not used.
        /// </summary>
        StatusID[] StatusProvide { get; set; }

        /// <summary>
        /// If player doesn't have these statuses from player self, this aciton will not used.
        /// </summary>
        StatusID[] StatusNeed { get; set; }

        /// <summary>
        /// Check for this action, but not for the rotation. It is some additional conditions for this action.
        /// Input data is the target for this action.
        /// </summary>
        Func<BattleChara, bool> ActionCheck { get; set; }

        /// <summary>
        /// Check for rotation, you can add it for simplify the rotation file. 
        /// Input data is the target for this action.
        Func<BattleChara, bool> RotationCheck { get; set; }

        /// <summary>
        /// Player's level is enough for this action's usage.
        /// </summary>
        bool EnoughLevel { get; }

        /// <summary>
        /// Is a GCD action.
        /// </summary>
        bool IsRealGCD { get; }

        /// <summary>
        /// Can I use this action at this time. It will check a lot of things.
        /// Level, Enabled, Action Status, MP, Player Status, Coll down, Combo, Moving (for casting), Charges, Target, etc.
        /// </summary>
        /// <param name="act"></param>
        /// <param name="mustUse">AOE only need one target to use.
        /// Moving action don't need to have enough distance to use. 
        /// Skip for <seealso cref="StatusProvide"/> and <seealso cref="TargetStatus"> checking.</param>
        /// <param name="emptyOrSkipCombo">Use all charges, no keeping one.
        /// Do not need to check the combo.</param>
        /// <param name="skipDisable">Skip the diable for emergency use. Please always set this to false.</param>
        /// <param name="gcdCountForAbility">The count of gcd for ability to delay. Only used in BLM right now</paramref>
        /// <returns>Should I use.</returns>
        bool CanUse(out IAction act, bool mustUse = false, bool emptyOrSkipCombo = false, bool skipDisable = false, uint gcdCountForAbility = 0, bool recordTarget = true);

        #region CoolDown
        /// <summary>
        /// Is action cooling down.
        /// </summary>
        bool IsCoolingDown { get; }

        /// <summary>
        /// Current charges count.
        /// </summary>
        ushort CurrentCharges { get; }

        /// <summary>
        /// Max charges count.
        /// </summary>
        ushort MaxCharges { get; }

        /// <summary>
        /// Has it been in cooldown for <paramref name="gcdCount"/> gcds and <paramref name="abilityCount"/> abilities?
        /// </summary>
        /// <param name="gcdCount"></param>
        /// <param name="abilityCount"></param>
        /// <returns></returns>
        bool ElapsedAfterGCD(uint gcdCount = 0, uint abilityCount = 0);

        /// <summary>
        /// Has it been in cooldown for <paramref name="time"/> seconds?
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        bool ElapsedAfter(float time);

        /// <summary>
        /// Will have at least one charge after <paramref name="gcdCount"/> gcds and <paramref name="abilityCount"/> abilities?
        /// </summary>
        /// <param name="gcdCount"></param>
        /// <param name="abilityCount"></param>
        /// <returns></returns>
        bool WillHaveOneChargeGCD(uint gcdCount = 0, uint abilityCount = 0);

        /// <summary>
        /// Will have at least one charge after <paramref name="time"/> seconds?
        /// </summary>
        /// <param name="remain"></param>
        /// <returns></returns>
        bool WillHaveOneCharge(float remain);
        #endregion

        #region Target
        /// <summary>
        /// If target has these statuses from player self, this aciton will not used.
        /// </summary>
        StatusID[] TargetStatus { get; set; }

        BattleChara Target { get; }

        /// <summary>
        /// Is target a boss.
        /// </summary>
        bool IsTargetBoss { get; }

        /// <summary>
        /// Is target will die immediately.
        /// </summary>
        bool IsTargetDying { get; }

        /// <summary>
        /// If this is an aoe action, how many hostile target would want to attack on, when you use this action.
        /// </summary>
        byte AOECount { set; }
        #endregion
    }
}
