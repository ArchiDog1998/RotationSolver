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
    [TabIcon(Icon = 21)] IDs,
    [TabIcon(Icon = 14)] Basic,
    [TabIcon(Icon = 42)] UI,
    [TabIcon(Icon = 29)] Auto,
    [TabIcon(Icon = 16)] Target,
    [TabIcon(Icon = 51)] Extra,
    [TabIcon(Icon = 5)] Debug,
}
