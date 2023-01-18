using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Configuration.RotationConfig
{
    internal class RotationConfigComparer : IEqualityComparer<IRotationConfig>
    {
        public bool Equals(IRotationConfig x, IRotationConfig y) => x.Name.Equals(y.Name);

        public int GetHashCode([DisallowNull] IRotationConfig obj) => obj.Name.GetHashCode();
    }
}
