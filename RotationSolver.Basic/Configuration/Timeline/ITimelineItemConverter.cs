using Newtonsoft.Json.Linq;
using RotationSolver.Basic.Configuration.Conditions;

namespace RotationSolver.Basic.Configuration.Timeline;

internal class ITimelineItemConverter : JsonCreationConverter<ITimelineItem>
{
    protected override ITimelineItem? Create(JObject jObject)
    {
        if (FieldExists(nameof(ActionTimelineItem.ID), jObject))
        {
            return new ActionTimelineItem();
        }
        else if (FieldExists(nameof(StateTimelineItem.State), jObject))
        {
            return new StateTimelineItem();
        }
        return null;
    }
}
