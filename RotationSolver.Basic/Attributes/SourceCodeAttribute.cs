using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Basic.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class SourceCodeAttribute : Attribute
{
    public string Url { get; }

    public SourceCodeAttribute(string url)
    {
        Url = url;
    }
}
