using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace RotationSolver.Basic.Configuration;
#pragma warning disable CS1591 // Missing XML comment for publicly visible
public class MacroInfo
{
    public int MacroIndex;
    public bool IsShared;

    public MacroInfo()
    {
        MacroIndex = -1;
        IsShared = false;
    }

    public unsafe bool AddMacro(GameObject tar = null)
    {
        if (MacroIndex < 0 || MacroIndex > 99) return false;

        var shared = RaptureMacroModule.Instance()->SharedSpan[MacroIndex];
        var individual = RaptureMacroModule.Instance()->IndividualSpan[MacroIndex];

        DataCenter.Macros.Enqueue(new MacroItem(tar, IsShared ? &shared : &individual));
        return true;
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible