namespace RotationSolver.Basic.Actions
{
    internal class MedicineItem : BaseItem
    {
        private readonly MedicineType _type;
        public MedicineItem(uint row, MedicineType type, uint a4 = 65535) : base(row, a4)
        {
            _type = type;
        }

        internal bool InType(ICustomRotation rotation) => rotation.MedicineType == _type;
    }
}
