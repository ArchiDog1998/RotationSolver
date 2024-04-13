using Lumina.Excel.GeneratedSheets;

namespace RotationSolver.Basic.Rotations;

partial class CustomRotation
{
    #region Burst Medicine
    /// <summary>
    /// The type of medicine.
    /// </summary>
    public abstract MedicineType MedicineType { get; }

    internal static MedicineItem[] Medicines { get; } = Service.GetSheet<Item>()
        .Where(i => i.FilterGroup == 6 && i.ItemSearchCategory.Row == 43)
        .Select(i => new MedicineItem(i))
        .Where(i => i.Type != MedicineType.None).Reverse().ToArray();

    /// <summary>
    /// Use the burst medicines.
    /// </summary>
    /// <param name="act"></param>
    /// <param name="clippingCheck"></param>
    /// <returns></returns>
    protected bool UseBurstMedicine(out IAction? act, bool clippingCheck = true)
    {
        act = null;

        if (!(HostileTarget?.IsDummy() ?? false) && !DataCenter.IsInHighEndDuty) return false;

        foreach (var medicine in Medicines)
        {
            if (medicine.Type != MedicineType) continue;

            if (medicine.CanUse(out act, clippingCheck)) return true;
        }

        return false;
    }
    #endregion

    #region MP Potions
    internal static MpPotionItem[] MpPotions { get; } = Service.GetSheet<Item>()
        .Where(i => i.FilterGroup == 9 && i.ItemSearchCategory.Row == 43)
        .Select(i => new MpPotionItem(i)).Reverse().ToArray();

    private static bool UseMpPotion(IAction nextGCD, out IAction? act)
    {
        var acts = from a in MpPotions
                   where a.CanUse(out _, true)
                   orderby a.MaxMp
                   select a;

        act = acts.LastOrDefault();
        return act != null;
    }
    #endregion

    #region HP Potions
    internal static HpPotionItem[] HpPotions { get; } = Service.GetSheet<Item>()
        .Where(i => i.FilterGroup == 8 && i.ItemSearchCategory.Row == 43)
        .Select(i => new HpPotionItem(i)).Reverse().ToArray();

    private static bool UseHpPotion(IAction nextGCD, out IAction? act)
    {
        var acts = from a in HpPotions
                   where a.CanUse(out _, true)
                   orderby a.MaxHp
                   select a;

        act = acts.LastOrDefault();
        return act != null;
    }
    #endregion
}
