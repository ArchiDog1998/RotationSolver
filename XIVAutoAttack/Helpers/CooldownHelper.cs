using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVAutoAttack.Helpers
{
    internal static class CooldownHelper
    {
        /// <summary>
        /// 距离下一个GCD转好还需要多少时间
        /// </summary>
        /// <param name="gcdCount">要隔着多少个GCD</param>
        /// <param name="abilityCount">再多少个能力技之后</param>
        /// <returns>还剩几秒</returns>
        internal static float WeaponRemain(uint gcdCount = 0, uint abilityCount = 0)
            => WeaponTime(gcdCount, abilityCount) + TargetHelper.WeaponRemain;

        /// <summary>
        /// 距离上一次搞好GCD已经过去多少时间里
        /// </summary>
        /// <param name="gcdCount">已经过去的GCD数量</param>
        /// <param name="abilityCount">已经过去多少个能力技了</param>
        /// <returns>算出总时间</returns>
        internal static float WeaponElapsed(uint gcdCount = 0, uint abilityCount = 0)
            => WeaponTime(gcdCount, abilityCount) + TargetHelper.Weaponelapsed;


        private static float WeaponTime(uint gcdCount = 0, uint abilityCount = 0)
            => TargetHelper.WeaponTotal * gcdCount
            + Service.Configuration.WeaponInterval * abilityCount;

        /// <summary>
        /// 这个技能已经运转了几个完整的GCD
        /// </summary>
        /// <param name="gcdCount">已经运转了多少个完整的GCD</param>
        /// <param name="abilityCount">已经运转了多少个能力技之后</param>
        /// <returns>是否已经转了这么久了</returns>
        internal static bool ElapsedAfter(float elapsed, uint gcdCount = 0, uint abilityCount = 0)
        {
            var gcdelapsed = WeaponElapsed(gcdCount, abilityCount);

            return IsLessThan(gcdelapsed, elapsed);
        }

        /// <summary>
        /// 距离下几个GCD转好这个技能能用吗。
        /// </summary>
        /// <param name="gcdCount">要隔着多少个完整的GCD</param>
        /// <param name="abilityCount">再多少个能力技之后</param>
        /// <returns>这个时间点是否起码有一层可以用</returns>
        internal static bool RecastAfter(float recast, uint gcdCount = 0, uint abilityCount = 0)
        {
            var remain = WeaponRemain(gcdCount, abilityCount);

            return IsLessThan(recast, remain);
        }

        private static bool IsLessThan(float a, float b)
        {
            if (a <= b) return true;

            if (Math.Abs(a - b) < 0.05) return true;

            return false;
        }
    }
}
