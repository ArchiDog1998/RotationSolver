namespace RotationSolver.Basic.Rotations;

public abstract partial class CustomRotation
{
    #region Tincture
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
        = new MedicineItem(37844, MedicineType.Mind);

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
    #endregion

    public static IBaseItem EchoDrops { get; } = new BaseItem(4566);

    #region Heal Potion
    public static IBaseItem HyperPotion { get; } = new HealPotionItem(38956, 0.25f, 11000);

    private bool UseHealPotion(out IAction act)
    {
        var acts = from prop in GetType().GetProperties()
                   where typeof(HealPotionItem).IsAssignableFrom(prop.PropertyType) && !(prop.GetMethod?.IsPrivate ?? true)
                   select (HealPotionItem)prop.GetValue(this) into a
                   where a != null && a.CanUse(out _)
                   orderby a.MaxHealHp
                   select a;

        act = acts.LastOrDefault();
        return act != null;
    }

    #endregion
}
