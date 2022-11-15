using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;


namespace XIVAutoAttack.Actions.BaseAction
{
    internal partial class BaseAction
    {
        /// <summary>
        /// 这个技能已经运转了几个完整的GCD
        /// </summary>
        /// <param name="gcdCount">已经运转了多少个完整的GCD</param>
        /// <param name="abilityCount">再多少个能力技之后</param>
        /// <returns>是否已经冷却了这么久了(不在冷却会返回false)</returns>
        internal bool ElapsedAfterGCD(uint gcdCount = 0, uint abilityCount = 0)
        {
            if (!IsCoolDown) return false;
            var elapsed = RecastTimeElapsedOneCharge;
            return CooldownHelper.ElapsedAfterGCD(elapsed, gcdCount, abilityCount);
        }

        /// <summary>
        /// 这个技能已经进入冷却多少秒了
        /// </summary>
        /// <param name="gcdelapsed">已经进行了多少秒了</param>
        /// <returns>是否已经冷却了这么久了(不在冷却会返回false)</returns>
        internal bool ElapsedAfter(float gcdelapsed)
        {
            if (!IsCoolDown) return false;
            var elapsed = RecastTimeElapsedOneCharge;
            return CooldownHelper.ElapsedAfter(elapsed, gcdelapsed);
        }

        /// <summary>
        /// 距离下几个GCD转好这个技能能用吗。
        /// </summary>
        /// <param name="gcdCount">要隔着多少个完整的GCD</param>
        /// <param name="abilityCount">再多少个能力技之后</param>
        /// <returns>这个时间点是否起码有一层可以用</returns>
        internal bool WillHaveOneChargeGCD(uint gcdCount = 0, uint abilityCount = 0)
        {
            if (HaveOneCharge) return true;
            var recast = RecastTimeRemainOneCharge;
            return CooldownHelper.RecastAfterGCD(recast, gcdCount, abilityCount);
        }

        /// <summary>
        /// 几秒钟以后能转好嘛
        /// </summary>
        /// <param name="remain">要多少秒呢</param>
        /// <returns>这个时间点是否起码有一层可以用</returns>
        internal bool WillHaveOneCharge(float remain)
        {
            return WillHaveOneCharge(remain, true);
        }

        private bool WillHaveOneCharge(float remain, bool addWeaponRemain)
        {
            if (HaveOneCharge) return true;
            var recast = RecastTimeRemainOneCharge;
            return CooldownHelper.RecastAfter(recast, remain, addWeaponRemain);
        }


        private unsafe RecastDetail* CoolDownDetail => ActionManager.Instance()->GetRecastGroupDetail(CoolDownGroup - 1);
        /// <summary>
        /// 复唱时间
        /// </summary>
        private unsafe float RecastTime => CoolDownDetail->Total;

        /// <summary>
        /// 复唱经过时间
        /// </summary>
        private unsafe float RecastTimeElapsed => CoolDownDetail->Elapsed;

        /// <summary>
        /// 是否正在冷却中
        /// </summary>
        internal unsafe bool IsCoolDown => CoolDownDetail->IsActive != 0;

        /// <summary>
        /// 复唱剩余时间
        /// </summary>
        private float RecastTimeRemain => RecastTime - RecastTimeElapsed;

        /// <summary>
        /// 技能的最大层数
        /// </summary>
        internal unsafe ushort MaxCharges => Math.Max(ActionManager.GetMaxCharges(AdjustedID, Service.ClientState.LocalPlayer.Level), (ushort)1);
        /// <summary>
        /// 是否起码有一层技能
        /// </summary>
        private bool HaveOneCharge => IsCoolDown ? RecastTimeElapsed >= RecastTimeOneCharge : true;
        /// <summary>
        /// 当前技能层数
        /// </summary>
        internal ushort CurrentCharges => IsCoolDown ? (ushort)(RecastTimeElapsed / RecastTimeOneCharge) : MaxCharges;

        private float RecastTimeOneCharge => ActionManager.GetAdjustedRecastTime(ActionType.Spell, AdjustedID) / 1000f;

        /// <summary>
        /// 下一层转好的时间
        /// </summary>
        private float RecastTimeRemainOneCharge => RecastTimeRemain % RecastTimeOneCharge;
        private float RecastTimeElapsedOneCharge => RecastTimeElapsed % RecastTimeOneCharge;

#if DEBUG
        internal bool HaveOneChargeDEBUG => HaveOneCharge;
        internal float RecastTimeOneChargeDEBUG => RecastTimeOneCharge;
        internal float RecastTimeElapsedDEBUG => RecastTimeElapsed;
        internal float RecastTimeRemainDEBUG => RecastTimeRemain;
#endif
    }
}
