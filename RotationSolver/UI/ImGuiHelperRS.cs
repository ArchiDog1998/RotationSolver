using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Configuration.Timeline.TimelineCondition;
using RotationSolver.Commands;
using RotationSolver.Updaters;
using XIVConfigUI;
using XIVConfigUI.ConditionConfigs;
using XIVConfigUI.SearchableConfigs;
using XIVDrawer;

namespace RotationSolver.UI;

internal static class ImGuiHelperRS
{
    private static float Scale => ImGuiHelpers.GlobalScale;

    static string _statusSearching = string.Empty;
    private static readonly CollapsingHeaderGroup _territoryActionsList = new()
    {
        HeaderSize = FontSize.Fifth,
    };

    public static void Init()
    {
        ConditionDrawer.CustomDrawings[typeof(StatusID)] = (obj, property) =>
        {
            var value = (uint)(property.GetValue(obj) as StatusID?)!.Value;

            var key = "Status PopUp " + property.Name + obj.GetHashCode();
            var status = Svc.Data.GetExcelSheet<Status>()?.GetRow(value);
            if (ImageLoader.GetTexture(value == 0 ? 16220 : status?.Icon ?? 10100, out var texture, 10100))
            {
                if (ImGuiHelper.NoPaddingNoColorImageButton(texture.ImGuiHandle, new Vector2(ConditionDrawer.IconSize * 3 / 4, ConditionDrawer.IconSize), "Status" + value.ToString()))
                {
                    ImGui.OpenPopup(key);
                }

                ImGuiHelper.ExecuteHotKeysPopup(key + "Edit", string.Empty, $"{status?.Name ?? "Unknown"} ({value})", false,
                    (() => property.SetValue(obj, StatusID.None), [VirtualKey.DELETE]));
            }

            Status[] statusList = [];

            if (Svc.Targets.Target is IBattleChara battle)
            {
                statusList = [..battle.StatusList.Select(s => s.GameData)];
            }

            if (statusList.Length == 0)
            {
                var type = property.GetCustomAttribute<StatusSourceAttribute>()?.Type ?? StatusType.AllStatus;
                statusList = type switch
                {
                    StatusType.BadStatus => StatusHelper.BadStatus,
                    StatusType.AllDispelStatus => StatusHelper.AllDispelStatus,
                    _ => StatusHelper.AllStatus,
                };
            }

            StatusPopUp(key, statusList, ref _statusSearching, s => property.SetValue(obj, (StatusID)s.RowId));

            return null;
        };

        ConditionDrawer.CustomDrawings[typeof(ActionID)] = (obj, property) =>
        {
            var value = (property.GetValue(obj) as ActionID?)!.Value;

            if (obj.GetType().GetRuntimeProperty(nameof(TimelineConditionBase.TimelineItem)) is PropertyInfo info) // TimelineItem
            {
                if (info.GetValue(obj) is not TimelineItem timelineItem) return null;

                var index = Array.IndexOf(timelineItem.ActionIDs, (uint)value);
                var actionNames = timelineItem.ActionIDs.Select(i => (Svc.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()?.GetRow(i)?.Name.RawString ?? "Unnamed Action") + $" ({i})").ToArray();

                if (ImGuiHelper.SelectableCombo("Action ##Select Action" + obj.GetHashCode(), actionNames, ref index))
                {
                    property.SetValue(obj, (ActionID)timelineItem.ActionIDs[index]);
                }
            }
            else //From the rotations.
            {
                if (DataCenter.RightNowRotation == null) return null;

                var popUpKey = $"Action Finder{obj.GetHashCode()}";
                ImGuiHelperRS.ActionSelectorPopUp(popUpKey, _territoryActionsList, DataCenter.RightNowRotation, item => property.SetValue(obj, (ActionID)item.ID));

                if (value.GetTexture(out var icon) || ImageLoader.GetTexture(4, out icon))
                {
                    var cursor = ImGui.GetCursorPos();
                    if (ImGuiHelper.NoPaddingNoColorImageButton(icon.ImGuiHandle, Vector2.One * ConditionDrawer.IconSize, obj.GetHashCode().ToString()))
                    {
                        if (!ImGui.IsPopupOpen(popUpKey)) ImGui.OpenPopup(popUpKey);
                    }
                    ImGuiHelper.DrawActionOverlay(cursor, ConditionDrawer.IconSize, 1);
                }
            }
            return null;
        };
    }

    const float INDENT_WIDTH = 180;

    internal static void DisplayCommandHelp(this Enum command, string extraCommand = "", Func<Enum, string>? getHelp = null, bool sameLine = true)
    {
        var cmdStr = command.GetCommandStr(extraCommand);

        if (ImGui.Button(cmdStr))
        {
            Svc.Commands.ProcessCommand(cmdStr);
        }
        if (ImGui.IsItemHovered())
        {
            ImGuiHelper.ShowTooltip($"{UiString.ConfigWindow_Helper_RunCommand.Local()}: {cmdStr}\n{UiString.ConfigWindow_Helper_CopyCommand.Local()}: {cmdStr}");

            if (ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            {
                ImGui.SetClipboardText(cmdStr);
            }
        }

        var help = getHelp?.Invoke(command);

        if (!string.IsNullOrEmpty(help))
        {
            if (sameLine)
            {
                ImGui.SameLine();
                ImGui.Indent(INDENT_WIDTH);
            }
            ImGui.Text(" → ");
            ImGui.SameLine();
            ImGui.TextWrapped(help);
            if (sameLine)
            {
                ImGui.Unindent(INDENT_WIDTH);
            }
        }
    }
  
    public static bool IsInRect(Vector2 leftTop, Vector2 size)
    {
        var pos = ImGui.GetMousePos() - leftTop;
        if (pos.X <= 0 || pos.Y <= 0 || pos.X >= size.X || pos.Y >= size.Y) return false;
        return true;
    }

    public static void Draw(this CombatType type)
    {
        if (type.HasFlag(CombatType.PvE))
        {
            ImGui.SameLine();
            ImGui.TextColored(ImGuiColors.DalamudYellow, " PvE");
        }
        if (type.HasFlag(CombatType.PvP))
        {
            ImGui.SameLine();
            ImGui.TextColored(ImGuiColors.TankBlue, " PvP");
        }
        if (type == CombatType.None)
        {
            ImGui.SameLine();
            ImGui.TextColored(ImGuiColors.DalamudRed, " None of PvE or PvP!");
        }
    }

    internal static void DrawGitHubBadge(string userName, string repository, string id = "", string link = "", bool center = false)
    {
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(repository)) return;

        var wholeWidth = ImGui.GetWindowWidth();

        link = string.IsNullOrEmpty(link) ? $"https://GitHub.com/{userName}/{repository}" : link;

        if (ImageLoader.GetTexture($"https://github-readme-stats.vercel.app/api/pin/?username={userName}&repo={repository}&theme=dark", out var icon)
            && (center ? ImGuiHelper.TextureButton(icon, wholeWidth, icon.Width, id)
            : ImGuiHelper.NoPaddingNoColorImageButton(icon.ImGuiHandle, new Vector2(icon.Width, icon.Height), id)))
        {
            Util.OpenLink(link);
        }

        var hasDate = ImageLoader.GetTexture($"https://img.shields.io/github/release-date/{userName}/{repository}?style=for-the-badge", out var releaseDate);

        var hasCount = ImageLoader.GetTexture($"https://img.shields.io/github/downloads/{userName}/{repository}/latest/total?style=for-the-badge&label=", out var downloadCount);

        var style = ImGui.GetStyle();
        var spacing = style.ItemSpacing;
        style.ItemSpacing = Vector2.Zero;
        if (center)
        {
            float width = 0;
            if (hasDate) width += releaseDate.Width;
            if (hasCount) width += downloadCount.Width;
            var ratio = MathF.Min(1, wholeWidth / width);
            ImGuiHelper.DrawItemMiddle(() =>
            {
                if (hasDate && ImGuiHelper.NoPaddingNoColorImageButton(releaseDate.ImGuiHandle, new Vector2(releaseDate.Width, releaseDate.Height) * ratio, id))
                {
                    Util.OpenLink(link);
                }
                if (hasDate && hasCount) ImGui.SameLine();
                if (hasCount && ImGuiHelper.NoPaddingNoColorImageButton(downloadCount.ImGuiHandle, new Vector2(downloadCount.Width, downloadCount.Height) * ratio, id))
                {
                    Util.OpenLink(link);
                }
            }, wholeWidth, width * ratio);
        }
        else
        {
            if (hasDate && ImGuiHelper.NoPaddingNoColorImageButton(releaseDate.ImGuiHandle, new Vector2(releaseDate.Width, releaseDate.Height), id))
            {
                Util.OpenLink(link);
            }
            if (hasDate && hasCount) ImGui.SameLine();
            if (hasCount && ImGuiHelper.NoPaddingNoColorImageButton(downloadCount.ImGuiHandle, new Vector2(downloadCount.Width, downloadCount.Height), id))
            {
                Util.OpenLink(link);
            }
        }
        style.ItemSpacing = spacing;
    }

    internal static string ToCommandStr(OtherCommandType type, string str, string extra = "")
    {
        var result = Service.COMMAND + " " + type.ToString() + " " + str;
        if (!string.IsNullOrEmpty(extra)) result += " " + extra;
        return result;
    }

    internal static void DrawTerritoryHeader()
    {
        using var font = ImRaii.PushFont(DrawingExtensions.GetFont(21));

        using var color = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudYellow);

        const int iconSize = 32;
        var contentFinder = DataCenter.ContentFinder;
        var territoryName = DataCenter.TerritoryName;
        if (contentFinder != null && !string.IsNullOrEmpty(contentFinder.Name))
        {
            territoryName += $" ({DataCenter.ContentFinderName})";
        }
        var icon = DataCenter.ContentFinder?.ContentType?.Value?.Icon ?? 23;
        if (icon == 0) icon = 23;
        var getIcon = ImageLoader.GetTexture(icon, out var texture);
        ImGuiHelper.DrawItemMiddle(() =>
        {
            if (getIcon)
            {
                ImGui.Image(texture.ImGuiHandle, Vector2.One * 28 * Scale);
                ImGui.SameLine();
            }
            ImGui.Text(territoryName);
        }, ImGui.GetWindowWidth(), ImGui.CalcTextSize(territoryName).X + ImGui.GetStyle().ItemSpacing.X + iconSize);
    }

    internal static void DrawContentFinder(ContentFinderCondition? content)
    {
        var badge = content?.Image;
        if (badge != null && badge.Value != 0
            && ImageLoader.GetTexture(badge.Value, out var badgeTexture))
        {
            var wholeWidth = ImGui.GetWindowWidth();
            var size = new Vector2(badgeTexture.Width, badgeTexture.Height) * MathF.Min(1, MathF.Min(480, wholeWidth) / badgeTexture.Width);

            ImGuiHelper.DrawItemMiddle(() =>
            {
                ImGui.Image(badgeTexture.ImGuiHandle, size);
            }, wholeWidth, size.X);
        }
    }

    internal static void DrawSupporterWarning()
    {
        if (DownloadHelper.IsSupporter) return;

        using var color = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudRed);

        ImGui.TextWrapped(UiString.SupporterOnlyWarning.Local());
    }

    internal static uint ChangeAlpha(uint color)
    {
        var c = ImGui.ColorConvertU32ToFloat4(color);
        c.W = 0.55f;
        return ImGui.ColorConvertFloat4ToU32(c);
    }

    internal static void StatusPopUp(string popupId, Status[] allStatus, ref string searching, Action<Status> clicked, uint notLoadId = 10100, float size = 32)
    {
        using var popup = ImRaii.Popup(popupId);
        if (popup)
        {
            ImGui.SetNextItemWidth(200 * Scale);
            ImGui.InputTextWithHint("##Searching the status", UiString.ConfigWindow_List_StatusNameOrId.Local(), ref searching, 128);

            ImGui.Spacing();

            using var child = ImRaii.Child("Rotation Solver Add Status", new Vector2(-1, 400 * Scale));
            if (!child) return;

            var count = Math.Max(1, (int)MathF.Floor(ImGui.GetWindowWidth() / ((size * 3 / 4 * Scale) + ImGui.GetStyle().ItemSpacing.X)));
            var index = 0;

            var searchingKey = searching;
            foreach (var status in allStatus.OrderByDescending(s => Searchable.Similarity(s.Name + " " + s.RowId.ToString(), searchingKey)))
            {
                if (ImageLoader.GetTexture(status.Icon, out var texture, notLoadId))
                {
                    if (index++ % count != 0)
                    {
                        ImGui.SameLine();
                    }
                    if (ImGuiHelper.NoPaddingNoColorImageButton(texture.ImGuiHandle, new Vector2(size * 3 / 4, size) * Scale, "Adding" + status.RowId.ToString()))
                    {
                        clicked?.Invoke(status);
                        ImGui.CloseCurrentPopup();
                    }
                    ImGuiHelper.HoveredTooltip($"{status.Name} ({status.RowId})");
                }
            }
        }
    }

    private const int count = 8;
    public static void ActionSelectorPopUp(string popUpId, CollapsingHeaderGroup group, ICustomRotation rotation, Action<IAction> action, System.Action? others = null)
    {
        if (group == null) return;

        using var popUp = ImRaii.Popup(popUpId);

        if (!popUp.Success) return;

        others?.Invoke();

        group.ClearCollapsingHeader();

        foreach (var pair in RotationUpdater.GroupActions(rotation.AllBaseActions.Where(i => i.Action.IsInJob()))!)
        {
            group.AddCollapsingHeader(() => pair.Key, () =>
            {
                var index = 0;
                foreach (var item in pair.OrderBy(t => t.ID))
                {
                    if (!IconSet.GetTexture((ActionID)item.ID, out var icon)) continue;

                    if (index++ % count != 0)
                    {
                        ImGui.SameLine();
                    }

                    using (var group = ImRaii.Group())
                    {
                        var cursor = ImGui.GetCursorPos();
                        if (ImGuiHelper.NoPaddingNoColorImageButton(icon.ImGuiHandle, Vector2.One * ConditionDrawer.IconSize, item.GetHashCode().ToString()))
                        {
                            action?.Invoke(item);
                            ImGui.CloseCurrentPopup();
                        }
                        ImGuiHelper.DrawActionOverlay(cursor, ConditionDrawer.IconSize, 1);
                    }

                    var name = item.Name;
                    if (!string.IsNullOrEmpty(name))
                    {
                        ImGuiHelper.HoveredTooltip(name);
                    }
                }
            });
        }
        group.Draw();
    }

    public static bool DrawStringList(List<string> names, float width, UiString tooltip)
    {
        width -= ImGuiEx.CalcIconSize(FontAwesomeIcon.Ban).X + ImGui.GetStyle().ItemSpacing.X + (10 * Scale);

        var changed = false;
        //Add one.
        if (!names.Any(string.IsNullOrEmpty))
        {
            names.Add(string.Empty);
            changed = true;
        }

        int removeIndex = -1;
        for (int i = 0; i < names.Count; i++)
        {
            var name = names[i];

            if (width > 0)
            {
                ImGui.SetNextItemWidth(width);
                if (ImGui.InputTextWithHint($"##{names.GetHashCode()} Name {i}", tooltip.Local(), ref name, 1024))
                {
                    names[i] = name;
                    changed = true;
                }
                ImGui.SameLine();
            }

            if (ImGuiEx.IconButton(FontAwesomeIcon.Ban, $"##{names.GetHashCode()} Remove Name {i}"))
            {
                removeIndex = i;
            }
        }
        if (removeIndex > -1)
        {
            names.RemoveAt(removeIndex);
            changed = true;
        }
        return changed;
    }
}