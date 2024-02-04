using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Basic.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class RotationAttribute(string name) : Attribute
{
    public string Name => name;
    public string? Description {  get; set; }
    public CombatType Type { get; set; } = CombatType.None;

    public string? GameVersion { get; set; }
}
