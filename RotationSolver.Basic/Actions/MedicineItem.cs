namespace RotationSolver.Basic.Actions;

/// <summary>
/// The type of the medicine
/// </summary>
public enum MedicineType : byte
{
    /// <summary>
    /// 
    /// </summary>
    Strength,

    /// <summary>
    /// 
    /// </summary>
    Dexterity,

    /// <summary>
    /// 
    /// </summary>
    Intelligence,

    /// <summary>
    /// 
    /// </summary>
    Mind,
}

internal class MedicineItem : BaseItem
{
    private readonly MedicineType _type;

    protected override bool CanUseThis => Service.Config.UseTinctures;

    public MedicineItem(uint row, MedicineType type, uint a4 = 65535) : base(row, a4)
    {
        _type = type;
    }

    internal bool InType(ICustomRotation rotation) => rotation.MedicineType == _type;
}
