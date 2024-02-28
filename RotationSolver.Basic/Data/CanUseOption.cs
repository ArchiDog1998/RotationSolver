using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Basic.Data;

/// <summary>
/// the option for the <see cref="IBaseAction.CanUse(out IAction, CanUseOption, byte)"/>
/// </summary>
[Flags]
public enum CanUseOption : byte
{
    /// <summary>
    /// None.
    /// </summary>
    None,

    /// <summary>
    /// Skip Status Provide Check
    /// </summary>
    [Description("Skip Status Provide Check")]
    SkipStatusProvideCheck = 1 << 0,

    /// <summary>
    /// Skip Combo Check
    /// </summary>
    [Description("Skip Combo Check")]
    SkipComboCheck = 1 << 1,

    /// <summary>
    /// Skip Casting and Moving Check
    /// </summary>
    [Description("Skip Casting and Moving Check")]
    SkipCastingCheck = 1 << 2,

    /// <summary>
    /// Is it used up all stacks
    /// </summary>
    [Description("Is it used up all stacks")]
    UsedUp = 1 << 3,

    /// <summary>
    /// Is it on the last ability
    /// </summary>
    [Description("Is it on the last ability")]
    OnLastAbility = 1 << 4,

    /// <summary>
    /// Skip clipping Check
    /// </summary>
    [Description("Skip clipping Check")]
    SkipClippingCheck = 1 << 5,

    /// <summary>
    /// Skip aoe Check
    /// </summary>
    [Description("Skip aoe Check")]
    SkipAoeCheck = 1 << 6,
}
