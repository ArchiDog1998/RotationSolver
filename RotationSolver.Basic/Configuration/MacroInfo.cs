using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using ImGuiNET;
using RotationSolver.Basic;
using RotationSolver.Data;
using RotationSolver.Helpers;

namespace RotationSolver.Configuration;

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

        DataCenter.Macros.Enqueue(new MacroItem(tar, IsShared ?
            RaptureMacroModule.Instance->Shared[MacroIndex] :
            RaptureMacroModule.Instance->Individual[MacroIndex]));

        return true;
    }
}
