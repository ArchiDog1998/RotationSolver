namespace RotationSolver.Basic.Configuration.Timeline.TimelineCondition;
public class ObjectGetter
{
    public bool CanGet(GameObject obj)
    {
        return true;
    }

    public GameObject[] TargetGetter(GameObject obj)
    {
        return [];
    }
}