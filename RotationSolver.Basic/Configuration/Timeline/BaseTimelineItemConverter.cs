using Newtonsoft.Json.Linq;
using RotationSolver.Basic.Configuration.Conditions;

namespace RotationSolver.Basic.Configuration.Timeline;

internal class BaseTimelineItemConverter : JsonCreationConverter<BaseTimelineItem>
{
    protected override BaseTimelineItem? Create(JObject jObject)
    {
        if (FieldExists(nameof(ActionTimelineItem.ID), jObject))
        {
            return new ActionTimelineItem();
        }
        else if (FieldExists(nameof(StateTimelineItem.State), jObject))
        {
            return new StateTimelineItem();
        }
        else if (FieldExists(nameof(MacroTimelineItem.Macro), jObject))
        {
            return new MacroTimelineItem();
        }
        else if (FieldExists(nameof(MoveTimelineItem.Points), jObject))
        {
            return new MoveTimelineItem();
        }
        return null;
    }
}
