using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Data
{
    internal record CustomRotationGroup(ClassJobID JobId, ClassJobID[] ClassJobIds, ICustomRotation[] Rotations);
}
