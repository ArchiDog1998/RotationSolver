namespace RotationSolver.Basic.Actions
{
    public interface IBaseAction : IAction
    {
        /// <summary>
        /// MP for casting.
        /// </summary>
        uint MPNeed { get; }

        /// <summary>
        /// Casting time
        /// </summary>
        float CastTime { get; }

        float Range { get; }

        bool IsFriendly { get; }
        bool IsTimeline { get; }
        bool IsEot { get; }
        EnemyPositional EnemyPositional { get; }

        /// <summary>
        /// If combo id is on this list, this action will not used.
        /// </summary>
        ActionID[] ComboIdsNot { set; }

        /// <summary>
        /// The combos that are not written on the action list.
        /// </summary>
        ActionID[] ComboIds { set; }

        /// <summary>
        /// If player has these statuses from player self, this action will not used.
        /// </summary>
        StatusID[] StatusProvide { get; set; }

        /// <summary>
        /// If player doesn't have these statuses from player self, this action will not used.
        /// </summary>
        StatusID[] StatusNeed { get; set; }

        /// <summary>
        /// Check for this action, but not for the rotation. It is some additional conditions for this action.
        /// Input data is the target for this action.
        /// </summary>
        Func<BattleChara, bool> ActionCheck { get; set; }

        /// <summary>
        /// Is a GCD action.
        /// </summary>
        bool IsRealGCD { get; }

        /// <summary>
        /// Is a simple gcd action, without other cooldown.
        /// </summary>
        bool IsGeneralGCD { get; }

        /// <summary>
        /// Can I use this action at this time. It will check a lot of things.
        /// Level, Enabled, Action Status, MP, Player Status, Coll down, Combo, Moving (for casting), Charges, Target, etc.
        /// </summary>
        /// <param name="act"></param>
        /// <param name="option">Options about using this method.</param>
        /// <param name="gcdCountForAbility">The count of gcd for ability to delay. Only used in BLM right now</paramref>
        /// <returns>Should I use.</returns>
        bool CanUse(out IAction act, CanUseOption option = CanUseOption.None, byte gcdCountForAbility = 0);

        #region CoolDown
        /// <summary>
        /// Current charges count.
        /// </summary>
        ushort CurrentCharges { get; }

        /// <summary>
        /// Max charges count.
        /// </summary>
        ushort MaxCharges { get; }

        /// <summary>
        /// At least has one Charge
        /// </summary>
        bool HasOneCharge { get; }

        /// <summary>
        /// recast time remain total.
        /// </summary>
        float RecastTimeRemain { get; }
        /// <summary>
        /// Has it been in cooldown for <paramref name="gcdCount"/> gcds and <paramref name="abilityCount"/> abilities?
        /// </summary>
        /// <param name="gcdCount"></param>
        /// <param name="abilityCount"></param>
        /// <returns></returns>
        bool ElapsedOneChargeAfterGCD(uint gcdCount = 0, uint abilityCount = 0);

        /// <summary>
        /// Has it been in cooldown for <paramref name="time"/> seconds?
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        bool ElapsedOneChargeAfter(float time);

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
