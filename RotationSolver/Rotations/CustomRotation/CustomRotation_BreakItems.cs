using RotationSolver;
using RotationSolver.Actions;
using RotationSolver.Data;
using System;
using System.Linq;

namespace RotationSolver.Combos.CustomCombo;

internal abstract partial class CustomRotation
{
    private static readonly BaseItem
        //刚力
        TinctureofStrength6 = new BaseItem(36109, 196625),
        //巧力
        TinctureofDexterity6 = new BaseItem(36110, 65535),
        //意力
        TinctureofMind6 = new BaseItem(36113, 65535),
        //意力
        TinctureofIntelligence6 = new BaseItem(36112, 65535);

    /// <summary>
    /// 是否使用爆发药
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    protected bool UseBreakItem(out IAction act)
    {
        act = null;
        if (Service.PartyList.Count() < 8) return false;
        if (Service.ClientState.LocalPlayer.Level < 90) return false;

        var role = Job.GetJobRole();
        switch (role)
        {
            case JobRole.Tank:
            case JobRole.Melee:
                if (TinctureofStrength6.ShoudUseItem(out act)) return true;
                break;

            case JobRole.RangedPhysical:
                if (TinctureofDexterity6.ShoudUseItem(out act)) return true;
                break;

            case JobRole.RangedMagicial:
                if (TinctureofIntelligence6.ShoudUseItem(out act)) return true;
                break;

            case JobRole.Healer:
                if (TinctureofMind6.ShoudUseItem(out act)) return true;
                break;
        }
        return false;
    }
}
