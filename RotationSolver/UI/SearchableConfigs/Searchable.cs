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
    public const float DRAG_WIDTH = 150;
    protected static float Scale => ImGuiHelpers.GlobalScale;
    public CheckBoxSearch Parent { get; set; }

    public string SearchingKeys =>Name + " " + Description;
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract string Command { get; }
    public abstract LinkDescription[] Tooltips { get; }
    public abstract string ID { get; }
    private string Popup_Key => "Rotation Solver RightClicking: " + ID;
    protected virtual bool IsJob => false;

    /// <summary>
    /// Only these job roles can get this setting.
    /// </summary>
    public JobRole[] JobRoles { get; set; }
    public Job[] Jobs { get; set; }

    public void Draw(Job job, bool mustDraw = false)
    {
        if (!mustDraw)
        {
            var canDraw = true;
            if (JobRoles != null)
            {
                var role = RotationUpdater.RightNowRotation?.ClassJob?.GetJobRole();
                if (role.HasValue)
                {
                    canDraw = JobRoles.Contains(role.Value);
                }
            }

            if (Jobs != null)
            {
                canDraw |= Jobs.Contains(RotationUpdater.Job);
            }

            if (!canDraw) return;
        }

        DrawMain(job);

        PrepareGroup(Popup_Key, Command, () => ResetToDefault(job));
    }

    public static void PrepareGroup(string key, string command, Action reset)
    {
        if (ImGui.BeginPopup(key))
        {
            if (ImGui.BeginTable(key, 2, ImGuiTableFlags.BordersOuter))
            {
                if (reset != null) DrawHotKeys("Reset to Default Value.", reset, "Backspace");

                if (!string.IsNullOrEmpty(command))
                {
                    DrawHotKeys($"Execute \"{command}\"", () => ExecuteCommand(command), "Alt");

                    DrawHotKeys($"Copy \"{command}\"", () => CopyCommand(command), "Ctrl");
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

                if(Tooltips != null) foreach (var tooltip in Tooltips)
                {
                    RotationConfigWindowNew.DrawLinkDescription(tooltip, wholeWidth, false);
                }
            });
        }

        ReactPopup(Popup_Key, Command, () => ResetToDefault(job));
    }

    public static void ReactPopup(string key, string command, Action reset, bool showHand = true)
    {
        if (showHand) ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);

        if (ImGui.IsMouseClicked(ImGuiMouseButton.Right))
        {
            if (!ImGui.IsPopupOpen(key))
            {
                ImGui.OpenPopup(key);
            }
        }
        if (reset != null) ExecuteHotKeys(reset, VirtualKey.BACK);
        ExecuteHotKeys(() => ExecuteCommand(command), VirtualKey.MENU);
        ExecuteHotKeys(() => CopyCommand(command), VirtualKey.CONTROL);
    }

    private static void ExecuteCommand(string command)
    {
        Svc.Commands.ProcessCommand(command);
    }

    private static void CopyCommand(string command)
    {
        ImGui.SetClipboardText(command);
        Notify.Success($"\"{command}\" copied to clipboard.");
    }

    public static void DrawHotKeys(string name, Action action, params string[] keys)
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
    public static void ExecuteHotKeys(Action action, params VirtualKey[] keys)
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
