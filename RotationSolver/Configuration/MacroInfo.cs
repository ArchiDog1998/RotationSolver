using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using ImGuiNET;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using RotationSolver.Updaters;

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

        MacroUpdater.Macros.Enqueue(new MacroItem(tar, IsShared ?
            RaptureMacroModule.Instance->Shared[MacroIndex] :
            RaptureMacroModule.Instance->Individual[MacroIndex]));

        return true;
    }

    public virtual void DisplayMacro()
    {
        if (ImGui.DragInt($"{LocalizationManager.RightLang.Configwindow_Events_MacroIndex}##MacroIndex{GetHashCode()}",
            ref MacroIndex, 1, 0, 99))
        {
            Service.Configuration.Save();
        }

        ImGui.SameLine();
        ImGuiHelper.Spacing();

        if (ImGui.Checkbox($"{LocalizationManager.RightLang.Configwindow_Events_ShareMacro}##ShareMacro{GetHashCode()}",
            ref IsShared))
        {
            Service.Configuration.Save();
        }
    }
}
