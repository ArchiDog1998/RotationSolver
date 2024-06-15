using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using ECommons.DalamudServices;
using ECommons.LanguageHelpers;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Configuration;
using RotationSolver.Commands;
using XIVConfigUI;
using XIVConfigUI.SearchableConfigs;
using XIVDrawer;

namespace RotationSolver.UI;

internal static class ImGuiHelperRS
{
    private static float Scale => ImGuiHelpers.GlobalScale;

    [Obsolete]
    internal static void SetNextWidthWithName(string name)
    {
        ImGui.SetNextItemWidth(Math.Max(80 * ImGuiHelpers.GlobalScale, ImGui.CalcTextSize(name).X + 30 * ImGuiHelpers.GlobalScale));
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

    public static void DisplayMacro(this MacroInfo info)
    {
        ImGui.SetNextItemWidth(50);
        if (ImGui.DragInt($"{UiString.ConfigWindow_Events_MacroIndex.Local()}##MacroIndex{info.GetHashCode()}",
            ref info.MacroIndex, 1, -1, 99))
        {
            Service.Config.Save();
        }

        ImGui.SameLine();

        if (ImGui.Checkbox($"{UiString.ConfigWindow_Events_ShareMacro.Local()}##ShareMacro{info.GetHashCode()}",
            ref info.IsShared))
        {
            Service.Config.Save();
        }
    }

    public static void DisplayEvent(this ActionEventInfo info)
    {
        if (ImGui.InputText($"{UiString.ConfigWindow_Events_ActionName.Local()}##ActionName{info.GetHashCode()}",
            ref info.Name, 100))
        {
            Service.Config.Save();
        }

        info.DisplayMacro();
    }

    public static void SearchCombo<T>(string popId, string name, ref string searchTxt, T[] items, Func<T, string> getSearchName, Action<T> selectAction, string searchingHint, ImFontPtr? font = null, Vector4? color = null)
    {
        if (SelectableButton(name + "##" + popId, font, color))
        {
            if (!ImGui.IsPopupOpen(popId)) ImGui.OpenPopup(popId);
        }

        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        }

        using var popUp = ImRaii.Popup(popId);
        if (!popUp.Success) return;

        if (items == null || items.Length == 0)
        {
            ImGui.TextColored(ImGuiColors.DalamudRed, "ConfigWindow_Condition_NoItemsWarning".Loc("There are no items!"));
            return;
        }

        var searchingKey = searchTxt;

        var members = items.Select(m => (m, getSearchName(m)))
            .OrderByDescending(s => Searchable.Similarity(s.Item2, searchingKey));

        ImGui.SetNextItemWidth(Math.Max(50 * ImGuiHelpers.GlobalScale, members.Max(i => ImGuiHelpers.GetButtonSize(i.Item2).X)));
        ImGui.InputTextWithHint("##Searching the member", searchingHint, ref searchTxt, 128);

        ImGui.Spacing();

        ImRaii.IEndObject? child = null;
        if (members.Count() >= 15)
        {
            ImGui.SetNextWindowSizeConstraints(new Vector2(0, 300), new Vector2(500, 300));
            child = ImRaii.Child(popId);
            if (!child) return;
        }

        foreach (var member in members)
        {
            if (ImGui.Selectable(member.Item2))
            {
                selectAction?.Invoke(member.m);
                ImGui.CloseCurrentPopup();
            }
        }
        child?.Dispose();
    }

    [Obsolete]
    public static unsafe bool SelectableCombo(string popUp, string[] items, ref int index, ImFontPtr? font = null, Vector4? color = null)
    {
        var count = items.Length;
        var originIndex = index;
        index = Math.Max(0, index) % count;
        var name = items[index] + "##" + popUp;

        var result = originIndex != index;

        if (SelectableButton(name, font, color))
        {
            if (count < 3)
            {
                index = (index + 1) % count;
                result = true;
            }
            else
            {
                if (!ImGui.IsPopupOpen(popUp)) ImGui.OpenPopup(popUp);
            }
        }

        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        }

        ImGui.SetNextWindowSizeConstraints(Vector2.Zero, Vector2.One * 500);
        if (ImGui.BeginPopup(popUp))
        {
            for (int i = 0; i < count; i++)
            {
                if (ImGui.Selectable(items[i]))
                {
                    index = i;
                    result = true;
                }
            }
            ImGui.EndPopup();
        }

        return result;
    }

    [Obsolete]
    public static unsafe bool SelectableButton(string name, ImFontPtr? font = null, Vector4? color = null)
    {
        List<IDisposable> disposables = new(2);
        if (font != null)
        {
            disposables.Add(ImRaii.PushFont(font.Value));
        }
        if (color != null)
        {
            disposables.Add(ImRaii.PushColor(ImGuiCol.Text, color.Value));
        }
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, ImGui.ColorConvertFloat4ToU32(*ImGui.GetStyleColorVec4(ImGuiCol.HeaderActive)));
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, ImGui.ColorConvertFloat4ToU32(*ImGui.GetStyleColorVec4(ImGuiCol.HeaderHovered)));
        ImGui.PushStyleColor(ImGuiCol.Button, 0);
        var result = ImGui.Button(name);
        ImGui.PopStyleColor(3);
        foreach (var item in disposables)
        {
            item.Dispose();
        }

        return result;
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
}