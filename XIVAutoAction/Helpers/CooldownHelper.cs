using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System;
using AutoAction.Data;
using AutoAction.Updaters;

namespace AutoAction.Helpers
{
    internal static class CooldownHelper
    {
        /// <summary>
        /// 距离下一个GCD转好还需要多少时间
        /// </summary>
        /// <param name="gcdCount">要隔着多少个GCD</param>
        /// <param name="abilityCount">再多少个能力技之后</param>
        /// <param name="addWeaponRemain">是否要把<see cref="ActionUpdater.WeaponRemain"/>加进去</param>
        /// <returns>还剩几秒</returns>
        private static float WeaponRemain(uint gcdCount = 0, uint abilityCount = 0)
            => WeaponTime(gcdCount, abilityCount) + ActionUpdater.WeaponRemain;

        /// <summary>
        /// 距离上一次搞好GCD已经过去多少时间里
        /// </summary>
        /// <param name="gcdCount">已经过去的GCD数量</param>
        /// <param name="abilityCount">已经过去多少个能力技了</param>
        /// <param name="addWeaponElapsed">是否要把<see cref="ActionUpdater.WeaponElapsed"/>加进去</param>
        /// <returns>算出总时间</returns>
        private static float WeaponElapsed(uint gcdCount = 0, uint abilityCount = 0)
            => WeaponTime(gcdCount, abilityCount) + ActionUpdater.WeaponElapsed;


        private static float WeaponTime(uint gcdCount = 0, uint abilityCount = 0)
            => ActionUpdater.WeaponTotal * gcdCount
            + Service.Configuration.WeaponInterval * abilityCount;


        /// <summary>
        /// 这个技能已经运转了几个完整的GCD
        /// </summary>
        /// <param name="gcdCount">已经运转了多少个完整的GCD</param>
        /// <param name="abilityCount">已经运转了多少个能力技之后</param>
        /// <param name="addWeaponElapsed">是否要把<see cref="ActionUpdater.WeaponElapsed"/>加进去</param>
        /// <returns>是否已经冷却了这么久了</returns>
        internal static bool ElapsedAfterGCD(float elapsed, uint gcdCount = 0, uint abilityCount = 0)
        {
            var gcdelapsed = WeaponElapsed(gcdCount, abilityCount);

            return IsLessThan(gcdelapsed, elapsed);
        }

        /// <summary>
        /// 这个技能已经进入冷却多少秒了
        /// </summary>
        /// <param name="gcdelapsed">已经进行了多少秒了</param>
        /// <returns>是否已经冷却了这么久了</returns>
        internal static bool ElapsedAfter(float elapsed, float gcdelapsed, bool addWeaponElapsed = true)
        {
            if (addWeaponElapsed) gcdelapsed += ActionUpdater.WeaponElapsed;
            return IsLessThan(gcdelapsed, elapsed);
        }

        /// <summary>
        /// 距离下几个GCD转好这个技能能用吗。
        /// </summary>
        /// <param name="gcdCount">要隔着多少个完整的GCD</param>
        /// <param name="abilityCount">再多少个能力技之后</param>
        /// <param name="addWeaponRemain">是否要把<see cref="ActionUpdater.WeaponRemain"/>加进去</param>
        /// <returns>这个时间点是否起码有一层可以用</returns>
        internal static bool RecastAfterGCD(float recast, uint gcdCount = 0, uint abilityCount = 0)
        {
            var remain = WeaponRemain(gcdCount, abilityCount);

            return IsLessThan(recast, remain);
        }

        /// <summary>
        /// 几秒钟以后是否已经结束
        /// </summary>
        /// <param name="remain">要多少秒呢</param>
        /// <param name="addWeaponRemain">是否要把<see cref="ActionUpdater.WeaponRemain"/>加进去</param>
        /// <returns>这个时间点是否已经结束</returns>
        internal static bool RecastAfter(float recast, float remain, bool addWeaponRemain = true)
        {
            if (addWeaponRemain) remain += ActionUpdater.WeaponRemain;

            return IsLessThan(recast, remain);
        }

        internal static bool IsLessThan(this float a, float b)
        {
            if (a <= b) return true;

            if (Math.Abs(a - b) < 0.05) return true;

            return false;
        }



        /// <summary>
        /// 计算魔法的咏唱或GCD时间(法系专供)
        /// </summary>
        /// <param name="GCDTime">原始咏唱时间或GCD时间(毫秒)</param>
        /// <param name="isSpell">是否是需要咏唱的魔法</param>
        /// <param name="isSpellTime">是否是计算咏唱的真实消耗时间,否则计算到判定点的时间</param>
        /// <returns>返回真实消耗的时间(毫秒)</returns>
        internal static unsafe double CalcSpellTime(double GCDTime, bool isSpell = true, bool isSpellTime = true)
        {
            var uiState = UIState.Instance();
            //获得当前等级
            var lvl = uiState->PlayerState.CurrentLevel;
            //获得咏速
            var speed = uiState->PlayerState.Attributes[46];
            //等级对照表
            var levelModifier = HealHelper.LevelTable[lvl];

            //读条税
            var spellTax = 100d;
            //判定点
            var spellPoint = 500d;
            //有否黑魔纹Buff
            var multiSpeed = Service.ClientState.LocalPlayer.HasStatus(true, StatusID.LeyLines) ? 0.85 : 1;

            //计算值
            var val = Math.Floor(GCDTime * multiSpeed * (1000d + Math.Ceiling(130d * (levelModifier.Sub - speed) / levelModifier.Div)) / 10000d) / 100d * 1000;

            //返回咏唱时间大于等于2.5秒的魔法的真实消耗时间
            if (isSpell && isSpellTime && GCDTime >= 2500) return val + spellTax;
            //返回魔法咏唱到判定点的时间
            else if (isSpell && !isSpellTime) return val - spellPoint;
            //返回咏唱时间低于2.5秒的魔法的真实消耗时间或GCD时间
            else return val;
        }
    }
}
