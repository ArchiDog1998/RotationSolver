namespace RotationSolver.Basic.Data;
internal class RaidLangs
{
#pragma warning disable IDE1006 // Naming Styles
    public Dictionary<string, Lang> langs { get; set; } = [];

    internal class Lang
    {
        public Dictionary<string, string> replaceSync { get; set; } = [];
        public Dictionary<string, string> replaceText { get; set; } = [];
    }
#pragma warning restore IDE1006 // Naming Styles
}