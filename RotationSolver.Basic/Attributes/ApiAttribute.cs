using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Basic.Attributes
{
    /// <summary>
    /// Attribute to specify the version of the API that this rotation targets.
    /// </summary>
    public class ApiAttribute(int apiVersion) : Attribute
    {
        /// <summary>
        /// The version of the API that this rotation targets.
        /// </summary>
        public int ApiVersion => apiVersion;
    }
}
