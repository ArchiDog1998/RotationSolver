using Dalamud.Game.ClientState.Objects.Types;
using RotationSolver.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Actions
{
    internal interface IBaseAction : IAction, IEnable
    {
        /// <summary>
        /// If combo id is on this list, this aciton will not used.
        /// </summary>
        ActionID[] ComboIdsNot {set; }

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
        /// Should I use this action at this time. It will check a lot of things.
        /// Level, Enabled, Action Status, MP, Player Status, Coll down, Combo, Moving (for casting), Charges, Target, etc.
        /// </summary>
        /// <param name="act"></param>
        /// <param name="mustUse">AOE only need one target to use.
        /// Moving action don't need to have enough distance to use. 
        /// Skip for <seealso cref="StatusProvide"/> and <seealso cref="TargetStatus"> cheking.</param>
        /// <param name="emptyOrSkipCombo">Use all charges, no keeping.
        /// Do not need to check the combo.</param>
        /// <param name="skipDisable">skip the diable for emergency use.</param>
        /// <returns>should I use.</returns>
        bool CanUse(out IAction act, bool mustUse = false, bool emptyOrSkipCombo = false, bool skipDisable = false);

        #region CoolDown

        /// <summary>
        /// Is cooling down.
        /// </summary>
        bool IsCoolDown { get; }

        /// <summary>
        /// The charges count.
        /// </summary>
        ushort CurrentCharges { get; }
        ushort MaxCharges { get; }
        bool ElapsedAfterGCD(uint gcdCount = 0, uint abilityCount = 0);

        bool ElapsedAfter(float gcdelapsed);

        bool WillHaveOneChargeGCD(uint gcdCount = 0, uint abilityCount = 0);

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
