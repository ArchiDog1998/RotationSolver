using Newtonsoft.Json.Linq;
using RotationSolver.Basic.Configuration.Conditions;

namespace RotationSolver.Basic.Configuration.Timeline.TimelineCondition;
internal class ITimelineConditionConverter : JsonCreationConverter<ITimelineCondition>
{
    protected override ITimelineCondition? Create(JObject jObject)
    {
        if (FieldExists(nameof(TimelineConditionSet.Conditions), jObject))
        {
            return new TimelineConditionSet();
        }
        else if (FieldExists(nameof(TimelineConditionAction.ActionID), jObject))
        {
            return new TimelineConditionAction();
        }
        else if (FieldExists(nameof(TimelineConditionTargetCount.Getter), jObject))
        {
            return new TimelineConditionTargetCount();
        }
        else if (FieldExists(nameof(TimelineConditionMapEffect.Position), jObject))
        {
            return new TimelineConditionMapEffect();
        }

        return null;
    }
}
