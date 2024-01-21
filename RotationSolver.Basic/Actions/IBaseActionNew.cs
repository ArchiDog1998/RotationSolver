using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.Basic.Actions;
public interface IBaseActionNew
{
    Action Action { get; }

    ActionTargetInfo TargetInfo { get; }
    ActionBasicInfo BasicInfo { get; }
}
