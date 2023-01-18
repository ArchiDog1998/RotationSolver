using RotationSolver.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Configuration.RotationConfig
{
    internal interface IRotationConfig
    {
        string Name { get; }
        string GetValue(ClassJobID job, string rotationName);
        void SetValue(ClassJobID job, string rotationName, string value);
        void Draw(RotationConfigSet set, bool canAddButton);
    }
}
