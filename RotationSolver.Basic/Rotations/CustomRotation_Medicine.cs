using RotationSolver.Basic;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Helpers;

namespace RotationSolver.Rotations.CustomRotation;

public enum MedicineType : byte
{
    Strength,
    Dexterity,
    Intelligence,
    Mind,
}

public abstract partial class CustomRotation
{
    public abstract MedicineType MedicineType { get; }
    public static IBaseItem TinctureOfStrength6 { get; } 
        = new MedicineItem(36109, MedicineType.Strength, 196625);
    public static IBaseItem TinctureOfDexterity6 { get; }
        = new MedicineItem(36110, MedicineType.Dexterity);
    public static IBaseItem TinctureOfIntelligence6 { get; }
        = new MedicineItem(36112, MedicineType.Intelligence);
    public static IBaseItem TinctureOfMind6 { get; } 
        = new MedicineItem(36113, MedicineType.Mind);
    public static IBaseItem TinctureOfStrength7 { get; } 
        = new MedicineItem(37840, MedicineType.Strength);
    public static IBaseItem TinctureOfDexterity7 { get; } 
        = new MedicineItem(37841, MedicineType.Dexterity);
    public static IBaseItem TinctureOfIntelligence7 { get; } 
        = new MedicineItem(37843, MedicineType.Intelligence);
    public static IBaseItem TinctureOfMind7 { get; } 
        = new MedicineItem(37844,  MedicineType.Mind);

    public static IBaseItem EchoDrops { get; } = new BaseItem(4566);
    
    static bool UseStrength(out IAction act)
    {
        if (TinctureOfStrength7.CanUse(out act)) return true;
        if (TinctureOfStrength6.CanUse(out act)) return true;
        return false;
    }

    static bool UseDexterity(out IAction act)
    {
        if (TinctureOfDexterity7.CanUse(out act)) return true;
        if (TinctureOfDexterity6.CanUse(out act)) return true;
        return false;
    }
    static bool UseIntelligence(out IAction act)
    {
        if (TinctureOfIntelligence7.CanUse(out act)) return true;
        if (TinctureOfIntelligence6.CanUse(out act)) return true;
        return false;
    }
    static bool UseMind(out IAction act)
    {
        if (TinctureOfMind7.CanUse(out act)) return true;
        if (TinctureOfMind6.CanUse(out act)) return true;
        return false;
    }
    protected bool UseBurstMedicine(out IAction act)
    {
        act = null;
        
        if (!InCombat) return false;
        if (!(Target?.IsDummy() ?? false) && !DataCenter.InHighEndDuty) return false;

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
