using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVAutoAttack.Data;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Helpers
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
        internal static float WeaponRemain(uint gcdCount = 0, uint abilityCount = 0, bool addWeaponRemain = true)
            => addWeaponRemain ? WeaponTime(gcdCount, abilityCount) + ActionUpdater.WeaponRemain : WeaponTime(gcdCount, abilityCount);

        /// <summary>
        /// 距离上一次搞好GCD已经过去多少时间里
        /// </summary>
        /// <param name="gcdCount">已经过去的GCD数量</param>
        /// <param name="abilityCount">已经过去多少个能力技了</param>
        /// <param name="addWeaponElapsed">是否要把<see cref="ActionUpdater.WeaponElapsed"/>加进去</param>
        /// <returns>算出总时间</returns>
        internal static float WeaponElapsed(uint gcdCount = 0, uint abilityCount = 0, bool addWeaponElapsed = true)
            => addWeaponElapsed? WeaponTime(gcdCount, abilityCount) + ActionUpdater.WeaponElapsed : WeaponTime(gcdCount, abilityCount);


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
        internal static bool ElapsedAfterGCD(float elapsed, uint gcdCount = 0, uint abilityCount = 0, bool addWeaponElapsed = true)
        {
            var gcdelapsed = WeaponElapsed(gcdCount, abilityCount, addWeaponElapsed);

            return IsLessThan(gcdelapsed, elapsed);
        }

        /// <summary>
        /// 这个技能已经进入冷却多少秒了
        /// </summary>
        /// <param name="gcdelapsed">已经进行了多少秒了</param>
        /// <param name="addWeaponElapsed">是否要把<see cref="ActionUpdater.WeaponElapsed"/>加进去</param>
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
        internal static bool RecastAfterGCD(float recast, uint gcdCount = 0, uint abilityCount = 0, bool addWeaponRemain = true)
        {
            var remain = WeaponRemain(gcdCount, abilityCount, addWeaponRemain);

            return IsLessThan(recast, remain);
        }

        /// <summary>
        /// 距离下几个GCD转好这个技能能用吗。
        /// </summary>
        /// <param name="gcdCount">要隔着多少个完整的GCD</param>
        /// <param name="abilityCount">再多少个能力技之后</param>
        /// <returns>这个时间点是否起码有一层可以用</returns>
        internal static bool RecastAfter(float recast, float remain, bool addWeaponRemain = true)
        {
            if (addWeaponRemain) remain += ActionUpdater.WeaponRemain;

            return IsLessThan(recast, remain);
        }

        private static bool IsLessThan(float a, float b)
        {
            if (a <= b) return true;

            if (Math.Abs(a - b) < 0.05) return true;

            return false;
        }

        /// <summary>
        /// 计算魔法的咏唱时间(黑魔专供)
        /// </summary>
        /// <param name="GCDTime">原始咏唱时间(毫秒)</param>
        /// <returns>真实的咏唱时间+读条税(毫秒)</returns>
        public static unsafe double CalcSpellTime(double GCDTime)
        {
            var uiState = UIState.Instance();
            //获得当前等级
            var lvl = uiState->PlayerState.CurrentLevel;
            //获得咏速
            var speed = uiState->PlayerState.Attributes[46];
            //等级对照表
            var levelModifier = HealHelper.LevelTable[lvl];

            //有黑魔纹Buff时
            if (Service.ClientState.LocalPlayer.HaveStatus(ObjectStatus.LeyLines))
            {
                return (Math.Floor(GCDTime * 0.85 * (1000d + Math.Ceiling(130d * (levelModifier.Sub - speed) / levelModifier.Div)) / 10000d) / 100d + 0.1) * 1000;
            }
            else
            {
                return (Math.Floor(GCDTime * (1000d + Math.Ceiling(130d * (levelModifier.Sub - speed) / levelModifier.Div)) / 10000d) / 100d + 0.1) * 1000;
            }
        }
    }
}
