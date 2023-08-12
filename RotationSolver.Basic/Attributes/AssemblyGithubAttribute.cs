using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Basic.Attributes;

/// <summary>
/// An attribute to record your github link.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class AssemblyGithubAttribute : Attribute
{
    /// <summary>
    /// A link for your user name in github.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// A link for the repo in your github.
    /// </summary>
    public string Repository { get; set; }
}
