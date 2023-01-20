using RotationSolver.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Timeline;

internal class MajorConditionSet
{
    public Dictionary<ActionID, ConditionSet> Conditions { get; } = new Dictionary<ActionID, ConditionSet>();

    public string Name { get; set; }

    public MajorConditionSet(string name)
    {
        Name = name;
    }
}
