namespace RotationSolver.Basic.Actions;

/// <summary>
/// User config.
/// </summary>
public struct ActionConfig()
{
    public bool IsEnable { get; set; } = true;

    public uint StatusGcdCount { get; set; } = 2;
    public byte AoeCount { get; set; } = 3;
    public float TimeToKill { get; set; } = 0;

    public bool ShouldCheckStatus { get; set; } = true;
    public float AutoHealRatio { get; set; } = 0.8f;

    public bool IsEnabled { get; set; }
    public bool IsInCooldown { get; set; }
}
