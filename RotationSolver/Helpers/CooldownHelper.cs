using FFXIVClientStructs.FFXIV.Client.Game.UI;
using RotationSolver;
using RotationSolver.Data;
using RotationSolver.Updaters;
using System;

namespace RotationSolver.Helpers;

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

        return gcdelapsed.IsLessThan(elapsed);
    }

    /// <summary>
    /// 这个技能已经进入冷却多少秒了
    /// </summary>
    /// <param name="gcdelapsed">已经进行了多少秒了</param>
    /// <returns>是否已经冷却了这么久了</returns>
    internal static bool ElapsedAfter(float elapsed, float gcdelapsed)
    {
        gcdelapsed += ActionUpdater.WeaponElapsed;
        return gcdelapsed.IsLessThan(elapsed);
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

        return recast.IsLessThan(remain);
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

        return recast.IsLessThan(remain);
    }

    internal static bool IsLessThan(this float a, float b)
    {
        if (a <= b) return true;

        if (Math.Abs(a - b) < 0.05) return true;

        return false;
    }
}
