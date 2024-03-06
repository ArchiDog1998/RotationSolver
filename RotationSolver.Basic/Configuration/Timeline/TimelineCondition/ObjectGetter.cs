using Dalamud.Game.ClientState.Objects.SubKinds;

namespace RotationSolver.Basic.Configuration.Timeline.TimelineCondition;

internal class ObjectGetter
{
    public bool IsPlayer { get; set; } = false;
    public uint DataID { get; set; } = 0;
    public JobRole Role { get; set; } = JobRole.None;
    public bool CanGet(GameObject obj)
    {
        if (IsPlayer && obj is not PlayerCharacter) return false;

        if (DataID != 0 && obj.DataId != DataID) return false;

        if (Role != JobRole.None && !obj.IsJobCategory(Role)) return false;
        return true;
    }
}