namespace RotationSolver.UI;

[AttributeUsage(AttributeTargets.Field)]
internal class TabSkipAttribute : Attribute
{

}

[AttributeUsage(AttributeTargets.Field)]
internal class TabIconAttribute : Attribute
{
    public uint Icon { get; set; }
}

internal enum RotationConfigWindowTab : byte
{
    [TabSkip] About,
    [TabSkip] Rotation,

    [TabIcon(Icon = 4)] Actions,
    [TabIcon(Icon = 47)] Rotations,
    [TabIcon(Icon = 21)] List,
    [TabIcon(Icon = 14)] Basic,
    [TabIcon(Icon = 42)] UI,
    [TabIcon(Icon = 29)] Auto,
    [TabIcon(Icon = 16)] Target,
    [TabIcon(Icon = 51)] Extra,
    [TabIcon(Icon = 5)] Debug,
}

public struct IncompatiblePlugin
{
    public string Name { get; set; }
    public string Icon { get; set; }
    public string Url { get; set; }
    public string Features { get; set; }

    public CompatibleType Type { get; set; }
}

[Flags]
public enum CompatibleType : byte
{
    Skill_Usage = 1 << 0,
    Skill_Selection = 1 << 1,
    Crash = 1 << 2,
}