using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using XIVAutoAttack.Combos;

namespace XIVAutoAttack.Actions.BaseAction
{
    internal partial class BaseAction
    {
        /// <summary>
        /// 距离下一个GCD转好还需要多少时间
        /// </summary>
        /// <param name="gcdCount">要隔着多少个GCD</param>
        /// <param name="abilityCount">再多少个能力技之后</param>
        /// <returns>还剩几秒</returns>
        internal static float WeaponRemain(uint gcdCount = 0, uint abilityCount = 0)
            => WeaponTime(gcdCount, abilityCount) + TargetHelper.WeaponRemain;

        private static float WeaponTime(uint gcdCount = 0, uint abilityCount = 0)
            => TargetHelper.WeaponTotal * gcdCount
            + Service.Configuration.WeaponInterval * abilityCount;

        /// <summary>
        /// 这个技能已经运转了几个完整的GCD
        /// </summary>
        /// <param name="gcdCount">已经运转了多少个完整的GCD</param>
        /// <param name="abilityCount">已经运转了多少个能力技之后</param>
        /// <returns>是否已经转了这么久了</returns>
        internal bool ElapsedAfter(uint gcdCount = 0, uint abilityCount = 0)
        {
            if (!IsCoolDown) return false;
            var elapsed = RecastTimeElapsed;
            var gcdelapsed = WeaponTime(gcdCount, abilityCount) + TargetHelper.Weaponelapsed;

            return IsLessThan(gcdelapsed , elapsed);
        }

        /// <summary>
        /// 距离下几个GCD转好这个技能能用吗。
        /// </summary>
        /// <param name="gcdCount">要隔着多少个完整的GCD</param>
        /// <param name="abilityCount">再多少个能力技之后</param>
        /// <returns>这个时间点是否起码有一层可以用</returns>
        internal bool WillHaveOneCharge(uint gcdCount = 0, uint abilityCount = 0)
        {
            if (HaveOneCharge) return true;
            var recast = RecastTimeOneCharge;
            var remain = WeaponRemain(gcdCount, abilityCount);

            return IsLessThan(recast, remain);
        }

        private static bool IsLessThan(float a, float b)
        {
            if (a <= b) return true;

            if (Math.Abs(a - b) < 0.05) return true;

            return false;
        }

        /// <summary>
        /// 复唱时间
        /// </summary>
        internal unsafe float RecastTime => ActionManager.Instance()->GetRecastTime(ActionType.Spell, ID);
        /// <summary>
        /// 咏唱时间
        /// </summary>
        internal virtual int Cast100 => Action.Cast100ms - (Service.ClientState.LocalPlayer.HaveStatus(ObjectStatus.LightSpeed, ObjectStatus.Requiescat) ? 25 : 0);
        
        [Obsolete("能否尽量不用，然后用WillHaveOneCharge")]
        internal float RecastTimeRemain => RecastTime - RecastTimeElapsed;

        [Obsolete("能否尽量不用，然后用ElapsedAfter")]
        internal unsafe float RecastTimeElapsed => ActionManager.Instance()->GetRecastTimeElapsed(ActionType.Spell, ID);
        internal unsafe ushort MaxCharges => Math.Max(ActionManager.GetMaxCharges(ID, Service.ClientState.LocalPlayer.Level), (ushort)1);
        internal unsafe bool IsCoolDown => ActionManager.Instance()->IsRecastTimerActive(ActionType.Spell, ID);
        /// <summary>
        /// 是否起码有一层技能
        /// </summary>
        internal bool HaveOneCharge => IsCoolDown ? RecastTimeElapsed >= RecastTimeOneCharge : true;

        private float RecastTimeOneCharge => RecastTime / MaxCharges;

        /// <summary>
        /// 下一层转好的时间
        /// </summary>
        internal float RecastTimeRemainOneCharge => RecastTimeRemain % RecastTimeOneCharge;
    }
}
