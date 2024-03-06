using Newtonsoft.Json.Linq;
using RotationSolver.Basic.Configuration.Conditions;

namespace RotationSolver.Basic.Configuration.Timeline.TimelineDrawing;

internal class IDrawingGetterConverter : JsonCreationConverter<IDrawingGetter>
{
    protected override IDrawingGetter? Create(JObject jObject)
    {
        if (FieldExists(nameof(ActionDrawingGetter.ActionID), jObject))
        {
            return new ActionDrawingGetter();
        }
        else if (FieldExists(nameof(ObjectDrawingGetter.IsActorEffect), jObject))
        {
            return new ObjectDrawingGetter();
        }
        else if (FieldExists(nameof(StaticDrawingGetter.Text), jObject))
        {
            return new StaticDrawingGetter();
        }

        return null;
    }
}
