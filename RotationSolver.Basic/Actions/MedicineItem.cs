namespace RotationSolver.Basic.Actions;

public enum MedicineType : byte
{
    Strength,
    Dexterity,
    Intelligence,
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
