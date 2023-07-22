namespace RotationSolver.Basic.Data;

/// <summary>
/// The Command of the next action.
/// </summary>
/// <param name="Act">the actio it self.</param>
/// <param name="DeadTime">When to stop</param>
public record NextAct(IAction Act, DateTime DeadTime);
