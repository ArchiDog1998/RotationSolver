namespace RotationSolver.Basic.Configuration.Timeline.TimelineCondition;

[Description("Map Effect")]
internal class TimelineConditionMapEffect : ITimelineCondition
{
    public Vector2 TimeDuration { get; set; } = new(0, 2);
    public int Position { get; set; }
    public int Param1 { get; set; }
    public int Param2 { get; set; }
    public bool IsTrue(TimelineItem item)
    {
        return DataCenter.MapEffects.Reverse().Any(effect =>
        {
            var time = effect.TimeDuration.TotalSeconds;

            if (time < TimeDuration.X) return false;
            if (time > TimeDuration.Y) return false;

            if (effect.Position != Position) return false;
            if (effect.Param1 != Param1) return false;
            if (effect.Param2 != Param2) return false;

            return true;
        });
    }
}
