using Newtonsoft.Json.Linq;
using RotationSolver.Basic.Configuration.Conditions;

namespace RotationSolver.Basic.Configuration.Timeline;

internal class ITimelineItemConverter : JsonCreationConverter<BaseTimelineItem>
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
        else if (FieldExists(nameof(DrawingTimeline.Condition), jObject))
        {
            return new DrawingTimeline();
        }

        return null;
    }
}
