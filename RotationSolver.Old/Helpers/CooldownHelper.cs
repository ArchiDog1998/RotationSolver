using RotationSolver.Updaters;
using System;

namespace RotationSolver.Helpers;

internal static class CooldownHelper
{
    private static float WeaponRemain(uint gcdCount = 0, uint abilityCount = 0)
        => WeaponTime(gcdCount, abilityCount) + ActionUpdater.WeaponRemain;

    private static float WeaponElapsed(uint gcdCount = 0, uint abilityCount = 0)
        => WeaponTime(gcdCount, abilityCount) + ActionUpdater.WeaponElapsed;


    private static float WeaponTime(uint gcdCount = 0, uint abilityCount = 0)
        => ActionUpdater.WeaponTotal * gcdCount
        + Service.Configuration.AbilitiesInterval * abilityCount;

    internal static bool ElapsedAfterGCD(float elapsed, uint gcdCount = 0, uint abilityCount = 0)
    {
        var gcdelapsed = WeaponElapsed(gcdCount, abilityCount);

        return gcdelapsed.IsLessThan(elapsed);
    }

    internal static bool ElapsedAfter(float elapsed, float gcdelapsed)
    {
        gcdelapsed += ActionUpdater.WeaponElapsed;
        return gcdelapsed.IsLessThan(elapsed);
    }

    internal static bool RecastAfterGCD(float recast, uint gcdCount = 0, uint abilityCount = 0)
    {
        var remain = WeaponRemain(gcdCount, abilityCount);

        return recast.IsLessThan(remain);
    }

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
