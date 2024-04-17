namespace RotationSolver.Basic.Data;

/// <summary>
/// Special State.
/// </summary>
public enum SpecialCommandType : byte
{
    /// <summary>
    /// 
    /// </summary>
    [Description("To end this special duration before the set time.")]
    EndSpecial,

    /// <summary>
    /// 
    /// </summary>
    [Description("Open a window to use AoE heal.")]
    HealArea,

    /// <summary>
    /// 
    /// </summary>
    [Description("Open a window to use single heal.")]
    HealSingle,

    /// <summary>
    /// 
    /// </summary>
    [Description("Open a window to use AoE defense.")]
    DefenseArea,

    /// <summary>
    /// 
    /// </summary>
    [Description("Open a window to use single defense.")]
    DefenseSingle,

    /// <summary>
    /// 
    /// </summary>
    [Description("Open a window to use Esuna, tank stance actions or True North.")]
    DispelStancePositional,

    /// <summary>
    /// 
    /// </summary>
    [Description("Open a window to use Raise or Shirk.")]
    RaiseShirk,

    /// <summary>
    /// 
    /// </summary>
    [Description("Open a window to move forward.")]
    MoveForward,

    /// <summary>
    /// 
    /// </summary>
    [Description("Open a window to move back.")]
    MoveBack,

    /// <summary>
    /// 
    /// </summary>
    [Description("Open a window to use knockback immunity actions.")]
    AntiKnockback,

    /// <summary>
    /// 
    /// </summary>
    [Description("Open a window to burst.")]
    Burst,

    /// <summary>
    /// 
    /// </summary>
    [Description("Open a window to speed up.")]
    Speed,

    /// <summary>
    /// 
    /// </summary>
    [Description("Open a window to use limit break.")]
    LimitBreak,

    /// <summary>
    /// 
    /// </summary>
    [Description("Open a window to do not use the casting action.")]
    NoCasting,
}

/// <summary>
/// The state of the plugin.
/// </summary>
public enum StateCommandType : byte
{
    /// <summary>
    /// 
    /// </summary>
    [Description("Stop the addon. Always remember to turn it off when it is not in use!")]
    Off,

    /// <summary>
    /// 
    /// </summary>
    [Description("Start the addon in Auto mode. When out of combat or when combat starts, switches the target according to the set condition.")]
    Auto,

    /// <summary>
    /// 
    /// </summary>
    [Description("Start the addon in Manual mode. You need to choose the target manually. This will bypass any engage settings that you have set up and will start attacking immediately once something is targeted.")]
    Manual,
}

/// <summary>
/// Some Other Commands.
/// </summary>
public enum OtherCommandType : byte
{
    /// <summary>
    /// 
    /// </summary>
    Settings,

    /// <summary>
    /// 
    /// </summary>
    Rotations,

    /// <summary>
    /// 
    /// </summary>
    DoActions,

    /// <summary>
    /// 
    /// </summary>
    ToggleActions,

    /// <summary>
    /// 
    /// </summary>
    [Description("Do the next action")]
    NextAction,
}