using RotationSolver.Actions;
using RotationSolver.Data;
using System.Linq;

namespace RotationSolver.Rotations.CustomRotation;
internal abstract partial class CustomRotation
{
    public static IBaseItem TinctureofStrength6 { get; } = new RoleItem(36109,
        new JobRole[] { JobRole.Tank, JobRole.Melee }, 196625);
    public static IBaseItem TinctureofDexterity6 { get; } = new RoleItem(36110,
        new JobRole[] { JobRole.RangedPhysical });
    public static IBaseItem TinctureofIntelligence6 { get; } = new RoleItem(36112,
        new JobRole[] { JobRole.RangedMagicial });
    public static IBaseItem TinctureofMind6 { get; } = new RoleItem(36113,
        new JobRole[] { JobRole.Healer });

    public static IBaseItem TinctureofStrength7 { get; } = new RoleItem(37840,
        new JobRole[] { JobRole.Tank, JobRole.Melee });
    public static IBaseItem TinctureofDexterity7 { get; } = new RoleItem(37841,
        new JobRole[] { JobRole.RangedPhysical });
    public static IBaseItem TinctureofIntelligence7 { get; } = new RoleItem(37843,
        new JobRole[] { JobRole.RangedMagicial });
    public static IBaseItem TinctureofMind7 { get; } = new RoleItem(37844,
        new JobRole[] { JobRole.Healer });

    public static IBaseItem EchoDrops { get; } = new BaseItem(4566);


    protected bool UseBurstMedicine(out IAction act)
    {
        act = null;
        
        if (!IsFullParty || !InCombat) return false;
        if (Service.ClientState.LocalPlayer?.Level < 90) return false;

        var role = Job.GetJobRole();
        switch (role)
        {
            case JobRole.Tank:
            case JobRole.Melee:
                if (TinctureofStrength7.CanUse(out act)) return true;
                if (TinctureofStrength6.CanUse(out act)) return true;
                break;

            case JobRole.RangedPhysical:
                if (TinctureofDexterity7.CanUse(out act)) return true;
                if (TinctureofDexterity6.CanUse(out act)) return true;
                break;

            case JobRole.RangedMagicial:
                if (TinctureofIntelligence7.CanUse(out act)) return true;
                if (TinctureofIntelligence6.CanUse(out act)) return true;
                break;

            case JobRole.Healer:
                if (TinctureofMind7.CanUse(out act)) return true;
                if (TinctureofMind6.CanUse(out act)) return true;
                break;
        }
        return false;
    }
}
