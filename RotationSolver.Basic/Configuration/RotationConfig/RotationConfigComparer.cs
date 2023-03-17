using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RotationSolver.Basic.Configuration.RotationConfig
{
    internal class RotationConfigComparer : IEqualityComparer<IRotationConfig>
    {
        public bool Equals(IRotationConfig x, IRotationConfig y) => x.Name.Equals(y.Name);

        public int GetHashCode([DisallowNull] IRotationConfig obj) => obj.Name.GetHashCode();
    }
}
