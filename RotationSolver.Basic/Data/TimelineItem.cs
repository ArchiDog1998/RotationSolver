using System.Text.RegularExpressions;

namespace RotationSolver.Basic.Data;
internal readonly struct TimelineItem(float time, string name, params string[] ids)
{
    public float Time => time;

    public bool IsShown => !name.StartsWith("--") && !name.EndsWith("--");

    public bool IsIdMatched(uint id)
    {
        return ids.Any(i => new Regex(i).IsMatch(id.ToString("X")));
    }

    public override string ToString()
    {
        return $"""
            IsShown: {IsShown},
            Time: {time},
            Name: {name},
            Ids: {string.Join(", ", ids)}
            """;
    }
}
