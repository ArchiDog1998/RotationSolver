namespace RotationSolver.Basic.Data;
internal class RaidLangs
{
    public Dictionary<string, Lang> langs { get; set; } = [];

    internal class Lang
    {
        public Dictionary<string, string> replaceSync { get; set; } = [];
        public Dictionary<string, string> replaceText { get; set; } = [];
    }
}