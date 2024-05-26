using System.ComponentModel;
using XIVConfigUI;

namespace RotationSolver.UI.ConfigWindows;

[Description("Timeline")]
public class TimelineItem : ConfigWindowItemRS
{
    public override uint Icon => 73;
    public override string Description => UiString.Item_Timeline.Local();

    public override void Draw(ConfigWindow window)
    {
        TimelineDrawer.DrawTimeline();
    }
}

