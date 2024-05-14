using ECommons.DalamudServices;
using System.ComponentModel;

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

    [Description("The abilities and custom conditions that your selected job has.")]
    [TabIcon(Icon = 4)] Actions,

    [Description("All rotations that RS has loaded.")]
    [TabIcon(Icon = 47)] Rotations,

    [Description("Some things that have to be lists.")]
    [TabIcon(Icon = 21)] List,

    [Description("Basic settings")]
    [TabIcon(Icon = 14)] Basic,

    [Description("Settings about the user interface.")]
    [TabIcon(Icon = 42)] UI,

    [Description("About some general actions usage and conditions.")]
    [TabIcon(Icon = 29)] Auto,

    [Description("The way to find the targets, hostiles or friends.")]
    [TabIcon(Icon = 16)] Target,

    [Description("Some features that shouldn't be included in RS but help you.")]
    [TabIcon(Icon = 51)] Extra,

    [TabIcon(Icon = 5)] Debug,
}

public struct IncompatiblePlugin
{
    public string Name { get; set; }
    public string Icon { get; set; }
    public string Url { get; set; }
    public string Features { get; set; }

    [JsonIgnore]
    public readonly bool IsInstalled 
    { 
        get
        {
            var name = this.Name;
            return Svc.PluginInterface.InstalledPlugins.Any(x => (x.Name.Contains(name) || x.InternalName.Contains(name)) && x.IsLoaded);
        }
    }

    public CompatibleType Type { get; set; }
}

[Flags]
public enum CompatibleType : byte
{
    Skill_Usage = 1 << 0,
    Skill_Selection = 1 << 1,
    Crash = 1 << 2,
}