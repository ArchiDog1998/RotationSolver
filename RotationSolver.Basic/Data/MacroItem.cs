using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Client.UI.Shell;

namespace RotationSolver.Basic.Data;

/// <summary>
/// The class to handle the event of macro.
/// </summary>
/// <remarks>
/// Constructer
/// </remarks>
/// <param name="target"></param>
/// <param name="macro"></param>
public unsafe class MacroItem(IGameObject? target, RaptureMacroModule.Macro* macro)
{
    private IGameObject? _lastTarget;
    readonly RaptureMacroModule.Macro* _macro = macro;

    /// <summary>
    /// The target of this macro.
    /// </summary>
    public IGameObject? Target { get; } = target;

    /// <summary>
    /// Is macro running.
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// Start running the macro.
    /// </summary>
    /// <returns></returns>
    public bool StartUseMacro()
    {
        if (RaptureShellModule.Instance()->MacroCurrentLine > -1) return false;

        _lastTarget = Svc.Targets.Target;
        Svc.Targets.Target = Target;
        RaptureShellModule.Instance()->ExecuteMacro(_macro);

        IsRunning = true;
        return true;
    }

    /// <summary>
    /// End this macro.
    /// </summary>
    /// <returns></returns>
    public bool EndUseMacro()
    {
        if (RaptureShellModule.Instance()->MacroCurrentLine > -1) return false;

        Svc.Targets.Target = _lastTarget;

        IsRunning = false;
        return true;
    }
}
