using RotationSolver.Basic;

namespace RotationSolver.Basic.Helpers;

public static class CooldownHelper
{
    private static float WeaponRemain(uint gcdCount = 0, uint abilityCount = 0)
        => WeaponTime(gcdCount, abilityCount) + DataCenter.WeaponRemain;

    private static float WeaponElapsed(uint gcdCount = 0, uint abilityCount = 0)
        => WeaponTime(gcdCount, abilityCount) + DataCenter.WeaponElapsed;


    private static float WeaponTime(uint gcdCount = 0, uint abilityCount = 0)
        => DataCenter.WeaponTotal * gcdCount
        + Service.Config.AbilitiesInterval * abilityCount;

    internal static bool ElapsedAfterGCD(float elapsed, uint gcdCount = 0, uint abilityCount = 0)
    {
        var gcdElapsed = WeaponElapsed(gcdCount, abilityCount);

        return gcdElapsed.IsLessThan(elapsed);
    }

    internal static bool ElapsedAfter(float elapsed, float gcdElapsed)
    {
        gcdElapsed += DataCenter.WeaponElapsed;
        return gcdElapsed.IsLessThan(elapsed);
    }

    public static bool RecastAfterGCD(float recast, uint gcdCount = 0, uint abilityCount = 0)
    {
        var remain = WeaponRemain(gcdCount, abilityCount);

        return recast.IsLessThan(remain);
    }

    internal static bool RecastAfter(float recast, float remain, bool addWeaponRemain = true)
    {
        if (addWeaponRemain) remain += DataCenter.WeaponRemain;

        return recast.IsLessThan(remain);
    }

    internal static bool IsLessThan(this float a, float b)
    {
        if (a <= b) return true;

        if (Math.Abs(a - b) < 0.05) return true;

        return false;
    }
}
