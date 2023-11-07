namespace RotationSolver.Basic.Configuration;

/// <summary>
/// Record something about your using of Rotation Solver.
/// </summary>
public class RotationSolverRecord
{
    /// <summary>
    /// How many times have rs clicked for you.
    /// </summary>
    public uint ClickingCount { get; set; } = 0;

    /// <summary>
    /// How many times have you greeted the other users.
    /// </summary>
    public uint SayingHelloCount { get; set; } = 0;

    /// <summary>
    /// The users that already said hello.
    /// </summary>
    public HashSet<string> SaidUsers { get; set; } = new HashSet<string>();
}
