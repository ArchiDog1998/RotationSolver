using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Basic.Configuration
{
    /// <summary>
    /// Various types used in the configuration.
    /// </summary>
    public class ConfigTypes
    {
        /// <summary>
        /// The type of AoE actions to use.
        /// </summary>
        public enum AoEType
        {
            /// <summary>
            /// No AoE.
            /// </summary>
            Off = 0,

            /// <summary>
            /// Only single-target AoE.
            /// </summary>
            Limited = 1,

            /// <summary>
            /// Full AoE.
            /// </summary>
            On = 2,
        }
    }
}
