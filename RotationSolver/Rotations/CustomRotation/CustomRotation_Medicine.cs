using RotationSolver.Actions;
using RotationSolver.Data;
using System.Linq;

namespace RotationSolver.Rotations.CustomRotation;
public enum MedicineType : byte
{
    Strength,
    Dexterity,
    Intelligence,
    Mind,
}
internal abstract partial class CustomRotation
{
    public abstract MedicineType MedicineType { get; }
    public static IBaseItem TinctureofStrength6 { get; } = new MedicineItem(36109,
        MedicineType.Strength, 196625);
    public static IBaseItem TinctureofDexterity6 { get; } = new MedicineItem(36110,
         MedicineType.Dexterity);
    public static IBaseItem TinctureofIntelligence6 { get; } = new MedicineItem(36112,
         MedicineType.Intelligence);
    public static IBaseItem TinctureofMind6 { get; } = new MedicineItem(36113,
        MedicineType.Mind);

    public static IBaseItem TinctureofStrength7 { get; } = new MedicineItem(37840,
         MedicineType.Strength);
    public static IBaseItem TinctureofDexterity7 { get; } = new MedicineItem(37841,
        MedicineType.Dexterity);
    public static IBaseItem TinctureofIntelligence7 { get; } = new MedicineItem(37843,
       MedicineType.Intelligence);
    public static IBaseItem TinctureofMind7 { get; } = new MedicineItem(37844,
        MedicineType.Mind);

    public static IBaseItem EchoDrops { get; } = new BaseItem(4566);
    
    static bool UseStrength(out IAction act)
    {
        if (TinctureofStrength7.CanUse(out act)) return true;
        if (TinctureofStrength6.CanUse(out act)) return true;
        return false;
    }

    static bool UseDexterity(out IAction act)
    {
        if (TinctureofDexterity7.CanUse(out act)) return true;
        if (TinctureofDexterity6.CanUse(out act)) return true;
        return false;
    }
    static bool UseIntelligence(out IAction act)
    {
        if (TinctureofIntelligence7.CanUse(out act)) return true;
        if (TinctureofIntelligence6.CanUse(out act)) return true;
        return false;
    }
    static bool UseMind(out IAction act)
    {
        if (TinctureofMind7.CanUse(out act)) return true;
        if (TinctureofMind6.CanUse(out act)) return true;
        return false;
    }
    protected bool UseBurstMedicine(out IAction act)
    {
        act = null;
        
        if (!IsFullParty || !InCombat) return false;
        if (Service.ClientState.LocalPlayer?.Level < 90) return false;

        switch (MedicineType)
        {
            case MedicineType.Strength:
                return UseStrength(out act);
            case MedicineType.Dexterity:
                return UseDexterity(out act);
            case MedicineType.Intelligence:
                return UseIntelligence(out act);
            case MedicineType.Mind:
                return UseMind(out act);
        }
        return false;
    }
}
