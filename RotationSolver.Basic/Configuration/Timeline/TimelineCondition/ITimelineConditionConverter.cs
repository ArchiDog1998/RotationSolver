using Newtonsoft.Json.Linq;
using RotationSolver.Basic.Configuration.Conditions;
using RotationSolver.Basic.Configuration.Timeline.TimelineDrawing;

namespace RotationSolver.Basic.Configuration.Timeline.TimelineCondition;
internal class ITimelineConditionConverter : JsonCreationConverter<ITimelineCondition>
{
    protected override ITimelineCondition? Create(JObject jObject)
    {
        if (FieldExists(nameof(ActionDrawingGetter.ActionID), jObject))
        {
            //return new ActionDrawingGetter();
        }

        return new TrueTimelineCondition();
    }
}
