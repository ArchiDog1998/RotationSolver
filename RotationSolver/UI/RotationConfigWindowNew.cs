﻿using Dalamud.Interface.Colors;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using ImGuiScene;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using RotationSolver.Updaters;

namespace RotationSolver.UI;

public class RotationConfigWindowNew : Window
{
    private static float _scale => ImGuiHelpers.GlobalScale;

    private RotationConfigWindowTab _activeTab;

    private const float MIN_COLUMN_WIDTH = 24;
    private const float JOB_ICON_WIDTH = 50;

    private string _searchText = string.Empty;

    public RotationConfigWindowNew()
    : base(nameof(RotationConfigWindowNew), ImGuiWindowFlags.NoScrollbar, false)
    {
        SizeCondition = ImGuiCond.FirstUseEver;
        Size = new Vector2(740f, 490f);
        RespectCloseHotkey = true;
    }

    public override void PreDraw()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.SelectableTextAlign, new Vector2(0.5f, 0.5f));
        base.PreDraw();
    }

    public override void PostDraw()
    {
        ImGui.PopStyleVar();
        base.PostDraw();
    }

    public override void Draw()
    {
        if(ImGui.BeginTable("Rotation Config Table", 2, ImGuiTableFlags.Resizable))
        {
            ImGui.TableSetupColumn("Rotation Config Side Bar", ImGuiTableColumnFlags.WidthFixed, 100 * _scale);
            ImGui.TableNextColumn();
            DrawSideBar();

            ImGui.TableNextColumn();
            DrawBody();

            ImGui.EndTable();
        }
    }

    private void DrawSideBar()
    {
        if (ImGui.BeginChild("Rotation Solver Side bar", Vector2.Zero))
        {
            var wholeWidth = ImGui.GetWindowSize().X;

            DrawHeader(wholeWidth);

            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();

            if(wholeWidth > JOB_ICON_WIDTH * _scale)
            {
                ImGui.SetNextItemWidth(wholeWidth);
                ImGui.InputTextWithHint("##Rotation Solver Search Box", "Searching is not available", ref _searchText, 128, ImGuiInputTextFlags.AutoSelectAll);
            }

            foreach (var item in Enum.GetValues<RotationConfigWindowTab>())
            {
                if (item.GetAttribute<TabSkipAttribute>() != null) continue;

                var icon = IconSet.GetTexture(item.GetAttribute<TabIconAttribute>()?.Icon ?? 0);

                if(icon != null && wholeWidth <= JOB_ICON_WIDTH * _scale)
                {
                    var size = Math.Max(_scale * MIN_COLUMN_WIDTH, Math.Min(wholeWidth, _scale * JOB_ICON_WIDTH)) * 0.6f;
                    DrawItemMiddle(() =>
                    {
                        if (SilenceImageButton(icon.ImGuiHandle, Vector2.One * size, _activeTab == item))
                        {
                            _activeTab = item;
                        }
                    }, Math.Max(_scale * MIN_COLUMN_WIDTH, wholeWidth), size);

                    ImguiTooltips.HoveredTooltip(item.ToString());
                }
                else
                {
                    if (ImGui.Selectable(item.ToString(), _activeTab == item, ImGuiSelectableFlags.None, new Vector2(0, 20)))
                    {
                        _activeTab = item;
                    }
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                    }
                }
            }

            var texture = wholeWidth <= 60 * _scale
                ? IconSet.GetTexture("https://storage.ko-fi.com/cdn/brandasset/kofi_s_logo_nolabel.png")
                : IconSet.GetTexture("https://storage.ko-fi.com/cdn/brandasset/kofi_bg_tag_dark.png");

            if (texture != null)
            {
                var width = Math.Min(150 * _scale, Math.Max(_scale * MIN_COLUMN_WIDTH, Math.Min(wholeWidth, texture.Width)));
                var size = new Vector2(width, width * texture.Height / texture.Width);
                size *= MathF.Max(_scale * MIN_COLUMN_WIDTH / size.Y, 1);
                var result = false;
                DrawItemMiddle(() =>
                {
                    ImGui.SetCursorPosY(ImGui.GetWindowSize().Y - size.Y);
                    ImGui.PushStyleColor(ImGuiCol.ButtonActive, 0);
                    ImGui.PushStyleColor(ImGuiCol.ButtonHovered, 0);
                    ImGui.PushStyleColor(ImGuiCol.Button, 0);
                    result = NoPaddingImageButton(texture.ImGuiHandle, size);
                    ImGui.PopStyleColor(3);
                }, wholeWidth, size.X);

                if (result)
                {
                    Util.OpenLink("https://ko-fi.com/B0B0IN5DX");
                }
            }

            ImGui.EndChild();
        }
    }

    private void DrawHeader(float wholeWidth)
    {
        var size = MathF.Max(MathF.Min(wholeWidth, _scale * 120), _scale * MIN_COLUMN_WIDTH);

        var logo = IconSet.GetTexture("https://raw.githubusercontent.com/ArchiDog1998/RotationSolver/main/docs/RotationSolverIcon_128.png");

        if (logo != null)
        {
            DrawItemMiddle(() =>
            {
                if (SilenceImageButton(logo.ImGuiHandle, Vector2.One * size,
                    _activeTab == RotationConfigWindowTab.About))
                {
                    _activeTab = RotationConfigWindowTab.About;
                }
                ImguiTooltips.HoveredTooltip(LocalizationManager.RightLang.ConfigWindow_About_Punchline);
            }, wholeWidth, size);

            ImGui.Spacing();
        }

        var rotation = RotationUpdater.RightNowRotation;
        if (rotation != null)
        {
            var rotations = RotationUpdater.CustomRotations.FirstOrDefault(i => i.ClassJobIds.Contains((Job)(Player.Object?.ClassJob.Id ?? 0)))?.Rotations ?? Array.Empty<ICustomRotation>();

            var iconSize = Math.Max(_scale * MIN_COLUMN_WIDTH, Math.Min(wholeWidth, _scale * JOB_ICON_WIDTH));
            var comboSize = ImGui.CalcTextSize(rotation.RotationName).X + _scale * 30;

            const string slash = " - ";
            var gameVersionSize = ImGui.CalcTextSize(slash + rotation.GameVersion).X + ImGui.GetStyle().ItemSpacing.X;
            var gameVersion = LocalizationManager.RightLang.ConfigWindow_Helper_GameVersion + ": ";
            var drawCenter = ImGui.CalcTextSize(slash + gameVersion + rotation.GameVersion).X + iconSize + ImGui.GetStyle().ItemSpacing.X * 3 < wholeWidth;
            if(drawCenter) gameVersionSize += ImGui.CalcTextSize(gameVersion).X + ImGui.GetStyle().ItemSpacing.X;

            DrawItemMiddle(() =>
            {
                var rotationIcon = rotation.GetTexture();
                if (rotationIcon != null && SilenceImageButton(rotationIcon.ImGuiHandle,
                    Vector2.One * iconSize, _activeTab == RotationConfigWindowTab.Rotation))
                {
                    _activeTab = RotationConfigWindowTab.Rotation;
                }
                var desc = rotation.Name + $" ({rotation.RotationName})";
                if (!string.IsNullOrEmpty(rotation.Description)) desc += "\n \n" + rotation.Description;
                ImguiTooltips.HoveredTooltip(desc);

                if (wholeWidth > _scale * JOB_ICON_WIDTH)
                {
                    ImGui.SameLine();

                    ImGui.BeginGroup();
                    ImGui.SetNextItemWidth(comboSize);
                    ImGui.PushStyleColor(ImGuiCol.Text, rotation.GetColor());
                    var isStartCombo = ImGui.BeginCombo("##RotationName:" + rotation.Name, rotation.RotationName);
                    ImGui.PopStyleColor();

                    if (isStartCombo)
                    {
                        foreach (var r in rotations)
                        {
                            ImGui.PushStyleColor(ImGuiCol.Text, r.GetColor());
                            if (ImGui.Selectable(r.RotationName))
                            {
                                Service.Config.RotationChoices[rotation.ClassJob.RowId] = r.GetType().FullName;
                                Service.Config.Save();
                            }
                            ImguiTooltips.HoveredTooltip(r.Description);
                            ImGui.PopStyleColor();
                        }
                        ImGui.EndCombo();
                    }

                    var warning = !rotation.IsValid ? string.Format(LocalizationManager.RightLang.ConfigWindow_Rotation_InvalidRotation,
                            rotation.GetType().Assembly.GetInfo().Author)
                    : rotation.IsBeta() ? LocalizationManager.RightLang.ConfigWindow_Rotation_BetaRotation : string.Empty;

                    warning = string.IsNullOrEmpty(warning) ? LocalizationManager.RightLang.ConfigWindow_Helper_SwitchRotation
                        : warning + "\n \n" + LocalizationManager.RightLang.ConfigWindow_Helper_SwitchRotation;
                    ImguiTooltips.HoveredTooltip(warning);

                    ImGui.TextDisabled(slash);
                    ImGui.SameLine();

                    if (drawCenter)
                    {
                        ImGui.TextDisabled(gameVersion);
                        ImGui.SameLine();
                    }
                    ImGui.Text(rotation.GameVersion);
                    ImGui.EndGroup();
                }
            }, wholeWidth, Math.Max(comboSize, gameVersionSize) + iconSize + ImGui.GetStyle().ItemSpacing.X);
        }
    }

    private unsafe static bool SilenceImageButton(IntPtr handle, Vector2 size, bool selected)
    {
        ImGui.PushStyleColor(ImGuiCol.ButtonActive,ImGui.ColorConvertFloat4ToU32(*ImGui.GetStyleColorVec4(ImGuiCol.HeaderActive)));
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, ImGui.ColorConvertFloat4ToU32(*ImGui.GetStyleColorVec4(ImGuiCol.HeaderHovered)));
        ImGui.PushStyleColor(ImGuiCol.Button,  selected ? ImGui.ColorConvertFloat4ToU32(*ImGui.GetStyleColorVec4(ImGuiCol.Header)) : 0);

        var result = NoPaddingImageButton(handle, size);
        ImGui.PopStyleColor(3);

        return result;
    }

    private static bool NoPaddingImageButton(IntPtr handle, Vector2 size)
    {
        var padding = ImGui.GetStyle().FramePadding;
        ImGui.GetStyle().FramePadding = Vector2.Zero;

        var result = ImGui.ImageButton(handle, size);
        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        }

        ImGui.GetStyle().FramePadding = padding;
        return result;
    }

    private static bool TextureButton(TextureWrap texture, float wholeWidth)
    {
        if (texture == null) return false;

        var size = new Vector2(texture.Width, texture.Height) * MathF.Min(1, wholeWidth / texture.Width);

        var result = false;
        DrawItemMiddle(() =>
        {
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, 0);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, 0);
            ImGui.PushStyleColor(ImGuiCol.Button, 0);
            result = NoPaddingImageButton(texture.ImGuiHandle, size);
            ImGui.PopStyleColor(3);
        }, wholeWidth, size.X);
        return result;
    }

    private static void DrawItemMiddle(Action drawAction, float wholeWidth, float width, bool leftAlign = true)
    {
        if (drawAction == null) return;
        var distance = (wholeWidth - width) / 2;
        if (leftAlign) distance = MathF.Max(distance, 0);
        ImGui.SetCursorPosX(distance);
        drawAction();
    }

    private void DrawBody()
    {
        var margin = 8 * _scale;
        ImGui.SetCursorPos(ImGui.GetCursorPos() + Vector2.One * margin);
        if (ImGui.BeginChild("Rotation Solver Body", Vector2.One * -margin))
        {
            switch (_activeTab)
            {
                case RotationConfigWindowTab.About:
                    DrawAbout();
                    break;

                case RotationConfigWindowTab.Rotation:
                    DrawRotation();
                    break;
            }
            ImGui.EndChild();
        }
    }

    private static readonly CollapsingHeaderGroup _aboutHeaders = new (new ()
    {
        { () => LocalizationManager.RightLang.ConfigWindow_About_Macros, () => 
        {
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

        StateCommandType.Auto.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

        StateCommandType.Manual.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

        StateCommandType.Cancel.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

        OtherCommandType.NextAction.DisplayCommandHelp(getHelp: i => LocalizationManager.RightLang.ConfigWindow_HelpItem_NextAction);

        ImGui.NewLine();

        SpecialCommandType.EndSpecial.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

        SpecialCommandType.HealArea.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

        SpecialCommandType.HealSingle.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

        SpecialCommandType.DefenseArea.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

        SpecialCommandType.DefenseSingle.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

        SpecialCommandType.MoveForward.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

        SpecialCommandType.MoveBack.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

        SpecialCommandType.Speed.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

        SpecialCommandType.EsunaStanceNorth.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

        SpecialCommandType.RaiseShirk.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

        SpecialCommandType.AntiKnockback.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

        SpecialCommandType.Burst.DisplayCommandHelp(getHelp: EnumTranslations.ToHelp);

        ImGui.PopStyleVar();
        } },

        { () => LocalizationManager.RightLang.ConfigWindow_About_Compatibility, () =>
        {
        } },

        { () => LocalizationManager.RightLang.ConfigWindow_About_Links, () =>
        {
            var width = ImGui.GetWindowWidth();
            if(TextureButton(IconSet.GetTexture("https://raw.githubusercontent.com/ArchiDog1998/RotationSolver/main/Images/Repository.png"), width))
            {
                Util.OpenLink("https://github.com/ArchiDog1998/RotationSolver");
            }

            if(TextureButton(IconSet.GetTexture("https://discordapp.com/api/guilds/1064448004498653245/embed.png?style=banner2"), width))
            {
                Util.OpenLink("https://discord.gg/4fECHunam9");
            }

            if(TextureButton(IconSet.GetTexture("https://badges.crowdin.net/badge/light/crowdin-on-dark.png"), width))
            {
                Util.OpenLink("https://crowdin.com/project/rotationsolver");
            }
        } },
    });
    private static void DrawAbout()
    {
        ImGui.PushFont(ImGuiHelper.GetFont(18));
        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_About_Punchline);
        ImGui.PopFont();
        ImGui.Spacing();

        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_About_Description);

        ImGui.Spacing();
        ImGui.PushStyleColor(ImGuiCol.Text, ImGui.ColorConvertFloat4ToU32(ImGuiColors.DalamudOrange));
        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_About_Warning);
        ImGui.PopStyleColor();

        ImGui.NewLine();
        _aboutHeaders.Draw();
    }

    private static readonly CollapsingHeaderGroup _rotationHeader = new(new()
    {

    });
    private void DrawRotation()
    {
        _rotationHeader.Draw();
    }
}
