using Lumina.Excel.GeneratedSheets;

namespace RotationSolver.Basic.Rotations;

partial class CustomRotation
{
    #region Tincture
    /// <summary>
    /// The type of medicine.
    /// </summary>
    public abstract MedicineType MedicineType { get; }

    /// <summary>
    /// 
    /// </summary>
    public static IBaseItem TinctureOfStrength6 { get; }
        = new MedicineItem(36109, MedicineType.Strength, 196625);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseItem TinctureOfDexterity6 { get; }
        = new MedicineItem(36110, MedicineType.Dexterity);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseItem TinctureOfIntelligence6 { get; }
        = new MedicineItem(36112, MedicineType.Intelligence);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseItem TinctureOfMind6 { get; }
        = new MedicineItem(36113, MedicineType.Mind);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseItem TinctureOfStrength7 { get; }
        = new MedicineItem(37840, MedicineType.Strength);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseItem TinctureOfDexterity7 { get; }
        = new MedicineItem(37841, MedicineType.Dexterity);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseItem TinctureOfIntelligence7 { get; }
        = new MedicineItem(37843, MedicineType.Intelligence);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseItem TinctureOfMind7 { get; }
        = new MedicineItem(37844, MedicineType.Mind);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseItem TinctureOfStrength8 { get; }
    = new MedicineItem(39727, MedicineType.Strength);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseItem TinctureOfDexterity8 { get; }
        = new MedicineItem(39728, MedicineType.Dexterity);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseItem TinctureOfIntelligence8 { get; }
        = new MedicineItem(39730, MedicineType.Intelligence);

    /// <summary>
    /// 
    /// </summary>
    public static IBaseItem TinctureOfMind8 { get; }
        = new MedicineItem(39731, MedicineType.Mind);

    static bool UseStrength(out IAction act, bool clippingCheck)
    {
        if (TinctureOfStrength8.CanUse(out act, clippingCheck)) return true;
        if (TinctureOfStrength7.CanUse(out act, clippingCheck)) return true;
        if (TinctureOfStrength6.CanUse(out act, clippingCheck)) return true;
        return false;
    }

    static bool UseDexterity(out IAction act, bool clippingCheck)
    {
        if (TinctureOfDexterity8.CanUse(out act, clippingCheck)) return true;
        if (TinctureOfDexterity7.CanUse(out act, clippingCheck)) return true;
        if (TinctureOfDexterity6.CanUse(out act, clippingCheck)) return true;
        return false;
    }
    static bool UseIntelligence(out IAction act, bool clippingCheck)
    {
        if (TinctureOfIntelligence8.CanUse(out act, clippingCheck)) return true;
        if (TinctureOfIntelligence7.CanUse(out act, clippingCheck)) return true;
        if (TinctureOfIntelligence6.CanUse(out act, clippingCheck)) return true;
        return false;
    }
    static bool UseMind(out IAction act, bool clippingCheck)
    {
        if (TinctureOfMind8.CanUse(out act, clippingCheck)) return true;
        if (TinctureOfMind7.CanUse(out act, clippingCheck)) return true;
        if (TinctureOfMind6.CanUse(out act, clippingCheck)) return true;
        return false;
    }

    /// <summary>
    /// Use the burst medicines.
    /// </summary>
    /// <param name="act"></param>
    /// <param name="clippingCheck"></param>
    /// <returns></returns>
    protected bool UseBurstMedicine(out IAction act, bool clippingCheck = true)
    {
        act = null;

#pragma warning disable CS0618 // Type or member is obsolete
        if (!(Target?.IsDummy() ?? false) && !DataCenter.IsInHighEndDuty) return false;
#pragma warning restore CS0618 // Type or member is obsolete

        return MedicineType switch
        {
            MedicineType.Strength => UseStrength(out act, clippingCheck),
            MedicineType.Dexterity => UseDexterity(out act, clippingCheck),
            MedicineType.Intelligence => UseIntelligence(out act, clippingCheck),
            MedicineType.Mind => UseMind(out act, clippingCheck),
            _ => false,
        };
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    public static IBaseItem EchoDrops { get; } = new BaseItem(4566);

    #region Heal Potion
    internal static HealPotionItem[] Potions { get; } = Service.GetSheet<Item>()
        .Where(i => i.FilterGroup == 8 && i.ItemSearchCategory.Row == 43)
        .Select(i => new HealPotionItem(i)).ToArray();

    private static bool UseHealPotion(out IAction? act)
    {
        var acts = from a in Potions
                   where a.CanUse(out _, true)
                   orderby a.MaxHealHp
                   select a;

        act = acts.LastOrDefault();
        return act != null;
    }

    #endregion
}
