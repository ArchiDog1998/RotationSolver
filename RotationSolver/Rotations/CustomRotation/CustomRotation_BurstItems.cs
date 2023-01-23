using RotationSolver.Actions;
using RotationSolver.Data;
using System.Linq;

namespace RotationSolver.Rotations.CustomRotation;
internal abstract partial class CustomRotation
{
    private static readonly IBaseItem
        //刚力
        TinctureofStrength6 = new BaseItem(36109, 196625),
        //巧力
        TinctureofDexterity6 = new BaseItem(36110, 65535),
        //意力
        TinctureofMind6 = new BaseItem(36113, 65535),
        //意力
        TinctureofIntelligence6 = new BaseItem(36112, 65535);

    protected bool UseBurstItem(out IAction act)
    {
        act = null;
        
        if (!IsFullParty) return false;
        if (Service.ClientState.LocalPlayer.Level < 90) return false;

        var role = Job.GetJobRole();
        switch (role)
        {
            case JobRole.Tank:
            case JobRole.Melee:
                if (TinctureofStrength6.CanUse(out act)) return true;
                break;

            case JobRole.RangedPhysical:
                if (TinctureofDexterity6.CanUse(out act)) return true;
                break;

            case JobRole.RangedMagicial:
                if (TinctureofIntelligence6.CanUse(out act)) return true;
                break;

            case JobRole.Healer:
                if (TinctureofMind6.CanUse(out act)) return true;
                break;
        }
        return false;
    }
}
