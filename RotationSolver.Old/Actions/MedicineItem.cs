using RotationSolver.Data;
using RotationSolver.Rotations.CustomRotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Actions
{
    internal class MedicineItem : BaseItem
    {
        private MedicineType _type;
        public MedicineItem(uint row, MedicineType type, uint a4 = 65535) : base(row, a4)
        {
            _type = type;
        }

        internal bool InType(ICustomRotation rotation) => rotation.MedicineType == _type;
    }
}
