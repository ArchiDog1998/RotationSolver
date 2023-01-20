using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Actions
{
    internal interface IBaseItem : IAction
    {
        bool ShoudUseItem(out IAction item);
    }
}
