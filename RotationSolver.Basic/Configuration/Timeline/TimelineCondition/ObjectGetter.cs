using Dalamud.Game.ClientState.Objects.SubKinds;
using System.Text.RegularExpressions;

namespace RotationSolver.Basic.Configuration.Timeline.TimelineCondition;

internal class ObjectGetter
{
    public bool IsPlayer { get; set; } = false;
    public string DataID { get; set; } = "";
    public JobRole Role { get; set; } = JobRole.None;
    public bool CanGet(GameObject obj)
    {
        if (IsPlayer && obj is not PlayerCharacter) return false;

        if (!string.IsNullOrEmpty(DataID) && !new Regex(DataID).IsMatch(obj.DataId.ToString("X"))) return false;

        if (Role != JobRole.None && !obj.IsJobCategory(Role)) return false;
        return true;
    }
}