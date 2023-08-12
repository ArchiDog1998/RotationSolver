using Dalamud.Game.ClientState.Keys;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.ImGuiMethods;
using RotationSolver.Localization;
using RotationSolver.UI.SearchableSettings;
using RotationSolver.Updaters;

namespace RotationSolver.UI.SearchableConfigs;

internal abstract class Searchable : ISearchable
{
    protected const float DRAG_WIDTH = 150;
    protected static float Scale => ImGuiHelpers.GlobalScale;
    public CheckBoxSearch Parent { get; set; }

    public string SearchingKey => Name + " : " + Description;
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract string Command { get; }
    public abstract LinkDescription[] Tooltips { get; }
    public abstract string ID { get; }
    private string Popup_Key => "Rotation Solver RightClicking: " + ID;

    public void Draw(Job job)
    {
        if (string.IsNullOrEmpty(Name)) return;

        DrawMain(job);

        if (ImGui.BeginPopup(Popup_Key))
        {
            if(ImGui.BeginTable(Popup_Key, 2, ImGuiTableFlags.BordersOuter))
            {
                DrawHotKeys("Reset to Default Value.", () => ResetToDefault(job), "Backspace");

                if (!string.IsNullOrEmpty(Command))
                {
                    DrawHotKeys($"Execute \"{Command}\"", ExecuteCommand, "Alt");

                    DrawHotKeys($"Copy \"{Command}\"", CopyCommand, "Ctrl");
                }
                ImGui.EndTable();
            }

            ImGui.EndPopup();
        }
    }

    protected abstract void DrawMain(Job job);

    public abstract void ResetToDefault(Job job);

    protected void ShowTooltip(Job job, bool showHand = true)
    {
        var showDesc = !string.IsNullOrEmpty(Description);
        if (showDesc || Tooltips != null)
        {
            ImguiTooltips.ShowTooltip(() =>
            {
                if (showDesc)
                {
                    ImGui.TextWrapped(Description);
                }
                if (showDesc && Tooltips != null && Tooltips.Length > 0)
                {
                    ImGui.Separator();
                }
                var wholeWidth = ImGui.GetWindowWidth();

                foreach (var tooltip in Tooltips)
                {
                    RotationConfigWindowNew.DrawLinkDescription(tooltip, wholeWidth, false);
                }
            });
        }

        if(showHand) ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);

        if (ImGui.IsMouseClicked(ImGuiMouseButton.Right)) 
        {
            if(!ImGui.IsPopupOpen(Popup_Key))
            {
                ImGui.OpenPopup(Popup_Key);
            }
        }

        ExecuteHotKeys(() => ResetToDefault(job), VirtualKey.BACK);
        ExecuteHotKeys(ExecuteCommand, VirtualKey.MENU);
        ExecuteHotKeys(CopyCommand, VirtualKey.CONTROL);
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

    private static void DrawHotKeys(string name, Action action, params string[] keys)
    {
        if (action == null) return;

        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        if (ImGui.Selectable(name))
        {
            action();
            ImGui.CloseCurrentPopup();
        }

        ImGui.TableNextColumn();
        ImGui.TextDisabled(string.Join(' ', keys));
    }

    private static readonly SortedList<string, bool> _lastChecked = new();
    private static void ExecuteHotKeys(Action action, params VirtualKey[] keys)
    {
        if (action == null) return;
        var name = string.Join(' ', keys);

        if (!_lastChecked.TryGetValue(name, out var last)) last = false;
        var now = keys.All(k => Svc.KeyState[k]);
        _lastChecked[name] = now;

        if (!last && now) action();
    }

    protected static void DrawJobIcon()
    {
        ImGui.SameLine();

        if (IconSet.GetTexture(IconSet.GetJobIcon(RotationUpdater.Job, IconType.Framed), out var texture))
        {
            ImGui.Image(texture.ImGuiHandle, Vector2.One * 24 * ImGuiHelpers.GlobalScale);
            ImguiTooltips.HoveredTooltip(LocalizationManager.RightLang.ConfigWindow_Configs_JobConfigTip);
        }
    }
}
