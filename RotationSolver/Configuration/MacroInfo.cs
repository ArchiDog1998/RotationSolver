using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Configuration;

public class MacroInfo
{
    public int MacroIndex { get; set; }
    public bool IsShared { get; set; }

    public MacroInfo()
    {
        MacroIndex = -1;
        IsShared = false;
    }
}
