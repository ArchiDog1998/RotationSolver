using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.ImGuiMethods;
using RotationSolver.UI.SearchableSettings;

namespace RotationSolver.UI.SearchableConfigs;

internal abstract class Searchable : ISearchable
{
    public CheckBoxSearch Parent { get; set; }

    public string SearchingKey => Name + " : " + Description;
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract string Command { get; }
    public abstract Action DrawTooltip { get; }
    public abstract string ID { get; }

    public abstract void Draw(Job job);

    public abstract void ResetToDefault();


    private const string Popup_Key = "Rotation Solver RightClicking Menu";
    protected void ShowTooltip()
    {
        ImguiTooltips.ShowTooltip(() =>
        {
            var showDesc = !string.IsNullOrEmpty(Description);
            if (showDesc)
            {
                ImGui.TextWrapped(Description);
            }
            if(showDesc && DrawTooltip != null)
            {
                ImGui.Separator();
            }
            DrawTooltip?.Invoke();
        });

        ImGui.SetNextWindowSizeConstraints(new Vector2(150, 0) * ImGuiHelpers.GlobalScale, Vector2.Zero);
        if(ImGui.BeginPopup(Popup_Key))
        {
            DrawHotKeys("Reset to Default Value.", ResetToDefault, ImGuiKey.Backspace);

            if (!string.IsNullOrEmpty(Command))
            {
                DrawHotKeys(Command, ExecuteCommand, ImGuiKey.LeftAlt, ImGuiKey.C);

                DrawHotKeys("Copy Command Data", CopyCommand, ImGuiKey.LeftCtrl, ImGuiKey.C);
            }

            ImGui.EndPopup();
        }

        if (ImGui.IsMouseClicked(ImGuiMouseButton.Right)) 
        {
            ImGui.OpenPopup(Popup_Key);
        }

        ExecuteHotKeys(ResetToDefault, ImGuiKey.Backspace);
        ExecuteHotKeys(ExecuteCommand, ImGuiKey.LeftAlt, ImGuiKey.C);
        ExecuteHotKeys(CopyCommand, ImGuiKey.LeftCtrl, ImGuiKey.C);
    }

    private void ExecuteCommand()
    {
        Svc.Commands.ProcessCommand(Command);
    }

    private void CopyCommand()
    {
        ImGui.SetClipboardText(Command);
        Notify.Success($"\"{Command}\" copied to clipboard.");
    }

    private static void DrawHotKeys(string name, Action action, params ImGuiKey[] keys)
    {
        if (action == null) return;

        if (ImGui.Selectable(name))
        {
            action();
            ImGui.CloseCurrentPopup();
        }

        ImGui.SameLine();
        var str = string.Join(' ', keys);
        ImGui.SetCursorPosY(ImGui.GetWindowSize().Y - ImGui.CalcTextSize(str).Y);
        ImGui.TextDisabled(str);
    }

    private static void ExecuteHotKeys(Action action, params ImGuiKey[] keys)
    {
        if (action == null) return;
        if(keys.All(ImGui.IsKeyDown)) action();
    }
}
