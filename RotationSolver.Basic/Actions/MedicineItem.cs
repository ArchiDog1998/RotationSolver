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

internal class MedicineItem(uint row, MedicineType type, uint a4 = 65535) : BaseItem(row, a4)
{
    private readonly MedicineType _type = type;

    protected override bool CanUseThis => Service.Config.UseTinctures;

    internal bool InType(ICustomRotation rotation) => rotation.MedicineType == _type;
}
