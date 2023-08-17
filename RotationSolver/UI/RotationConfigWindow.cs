using Dalamud.Interface.Colors;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using Dalamud.Utility;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.ImGuiMethods;
using ExCSS;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;
using ImGuiScene;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Configuration;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using RotationSolver.UI.SearchableConfigs;
using RotationSolver.UI.SearchableSettings;
using RotationSolver.Updaters;
using System;
using System.Diagnostics;
using GAction = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.UI;

public partial class RotationConfigWindow : Window
{
    private static float _scale => ImGuiHelpers.GlobalScale;
    private static Job Job => DataCenter.Job;

    private RotationConfigWindowTab _activeTab;

    private const float MIN_COLUMN_WIDTH = 24;
    private const float JOB_ICON_WIDTH = 50;

    public RotationConfigWindow()
        : base("", ImGuiWindowFlags.NoScrollbar, false)
    {
        SizeCondition = ImGuiCond.FirstUseEver;
        Size = new Vector2(740f, 490f);
        SizeConstraints = new WindowSizeConstraints()
        {
             MinimumSize = new Vector2(250, 300),
             MaximumSize = new Vector2(5000, 5000),
        };
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

    public override void OnClose()
    {
        Service.Config.Save();
        base.OnClose();
    }

    public override void Draw()
    {
        try
        {
            if (ImGui.BeginTable("Rotation Config Table", 2, ImGuiTableFlags.Resizable))
            {
                ImGui.TableSetupColumn("Rotation Config Side Bar", ImGuiTableColumnFlags.WidthFixed, 100 * _scale);
                ImGui.TableNextColumn();
                DrawSideBar();

                ImGui.TableNextColumn();
                DrawBody();

                ImGui.EndTable();
            }
        }
        catch(Exception ex)
        {
            PluginLog.Warning(ex, "Something wrong with new ui");
        }
    }

    private void DrawSideBar()
    {
        if (BeginChild("Rotation Solver Side bar", -Vector2.One, false, ImGuiWindowFlags.NoScrollbar))
        {
            var wholeWidth = ImGui.GetWindowSize().X;

            DrawHeader(wholeWidth);

            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing(); 

            var iconSize = Math.Max(_scale * MIN_COLUMN_WIDTH, Math.Min(wholeWidth, _scale * JOB_ICON_WIDTH)) * 0.6f;

            if (wholeWidth > JOB_ICON_WIDTH * _scale)
            {
                ImGui.SetNextItemWidth(wholeWidth);
                SearchingBox();

                ImGui.Spacing();
            }
            else
            {
                if(IconSet.GetTexture(46, out var icon))
                {
                    DrawItemMiddle(() =>
                    {
                        if (ImGui.BeginPopup("Searching Popup"))
                        {
                            ImGui.SetNextItemWidth(200 * _scale);
                            SearchingBox();
                            if (ImGui.IsKeyDown(ImGuiKey.Enter))
                            {
                                ImGui.CloseCurrentPopup();
                            }
                            ImGui.EndPopup();
                        }

                        var cursor = ImGui.GetCursorPos();
                        if (NoPaddingNoColorImageButton(icon.ImGuiHandle, Vector2.One * iconSize))
                        {
                            ImGui.OpenPopup("Searching Popup");
                        }
                        DrawActionOverlay(cursor, iconSize, -1);
                        ImguiTooltips.HoveredTooltip("Search");

                    }, Math.Max(_scale * MIN_COLUMN_WIDTH, wholeWidth), iconSize);
                }
            }

            foreach (var item in Enum.GetValues<RotationConfigWindowTab>())
            {
                if (item.GetAttribute<TabSkipAttribute>() != null) continue;

                if(IconSet.GetTexture(item.GetAttribute<TabIconAttribute>()?.Icon ?? 0, out var icon) && wholeWidth <= JOB_ICON_WIDTH * _scale)
                {
                    DrawItemMiddle(() =>
                    {
                        var cursor = ImGui.GetCursorPos();
                        if (NoPaddingNoColorImageButton(icon.ImGuiHandle, Vector2.One * iconSize, item.ToString()))
                        {
                            _activeTab = item;
                        }
                        DrawActionOverlay(cursor, iconSize, _activeTab == item ? 1 : 0);
                    }, Math.Max(_scale * MIN_COLUMN_WIDTH, wholeWidth), iconSize);

                    var desc = item.ToString();
                    var addition = item.ToDescription();
                    if (!string.IsNullOrEmpty(addition)) desc += "\n \n" + addition;
                    ImguiTooltips.HoveredTooltip(desc);
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
                        var desc = item.ToDescription();
                        if (!string.IsNullOrEmpty(desc)) ImguiTooltips.ShowTooltip(desc);
                    }
                }
            }

            if (wholeWidth <= 60 * _scale 
                ? IconSet.GetTexture("https://storage.ko-fi.com/cdn/brandasset/kofi_s_logo_nolabel.png", out var texture)
                : IconSet.GetTexture("https://storage.ko-fi.com/cdn/brandasset/kofi_bg_tag_dark.png", out texture))
            {
                var width = Math.Min(150 * _scale, Math.Max(_scale * MIN_COLUMN_WIDTH, Math.Min(wholeWidth, texture.Width)));
                var size = new Vector2(width, width * texture.Height / texture.Width);
                size *= MathF.Max(_scale * MIN_COLUMN_WIDTH / size.Y, 1);
                var result = false;
                DrawItemMiddle(() =>
                {
                    ImGui.SetCursorPosY(ImGui.GetWindowSize().Y + ImGui.GetScrollY() - size.Y);
                    result = NoPaddingNoColorImageButton(texture.ImGuiHandle, size, "Donate Plugin");
                }, wholeWidth, size.X);

                if (result)
                {
                    Util.OpenLink("https://ko-fi.com/B0B0IN5DX");
                }
            }

            ImGui.EndChild();
        }
    }

    private const int FRAME_COUNT = 180;
    private static SortedList<string, TextureWrap> _textureWrapList = new SortedList<string, TextureWrap>(FRAME_COUNT);
    private static bool GetLocalImage(string name, out TextureWrap texture)
    {
        var url = $"RotationSolver.Logos.{name}.png";
        if (_textureWrapList.TryGetValue(name, out texture)) return texture != null;

        using var stream = typeof(RotationConfigWindow).Assembly.GetManifestResourceStream(url);
        if (stream == null)
        {
            PluginLog.Warning($"Failed to load the pic: {url} when getting the stream from assembly.");
            _textureWrapList[url] = null;
            return false;
        }

        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        texture = Svc.PluginInterface.UiBuilder.LoadImage(memory.ToArray());
        if(texture == null)
        {
            PluginLog.Warning($"Failed to load the pic: {url} when convert bytes to image.");
            _textureWrapList[url] = null;
            return false;
        }
        _textureWrapList[url] = texture;
        return true;
    }

    private void DrawHeader(float wholeWidth)
    {
        var size = MathF.Max(MathF.Min(wholeWidth, _scale * 128), _scale * MIN_COLUMN_WIDTH);

        if (IconSet.GetTexture((uint)0, out var overlay))
        {
            DrawItemMiddle(() =>
            {
                var cursor = ImGui.GetCursorPos();
                if (SilenceImageButton(overlay.ImGuiHandle, Vector2.One * size,
                    _activeTab == RotationConfigWindowTab.About, "About Icon"))
                {
                    _activeTab = RotationConfigWindowTab.About;
                }
                ImguiTooltips.HoveredTooltip(LocalizationManager.RightLang.ConfigWindow_About_Punchline);

                var frame = Environment.TickCount / 34 % FRAME_COUNT;
                if (frame <= 0) frame += FRAME_COUNT;
                if (Service.Config.GetValue(PluginConfigBool.DrawIconAnimation) 
                    ? GetLocalImage(frame.ToString("D4"), out var logo)
                    : IconSet.GetTexture("https://raw.githubusercontent.com/ArchiDog1998/RotationSolver/main/Images/Logo.png", out logo))
                {
                    ImGui.SetCursorPos(cursor);
                    ImGui.Image(logo.ImGuiHandle, Vector2.One * size);
                }
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
            if (drawCenter) gameVersionSize += ImGui.CalcTextSize(gameVersion).X + ImGui.GetStyle().ItemSpacing.X;

            var horizonalWholeWidth = Math.Max(comboSize, gameVersionSize) + iconSize + ImGui.GetStyle().ItemSpacing.X;

            if (horizonalWholeWidth > wholeWidth)
            {
                DrawItemMiddle(() =>
                {
                    DrawRotationIcon(rotation, iconSize);
                }, wholeWidth, iconSize);

                if (_scale * JOB_ICON_WIDTH < wholeWidth)
                {
                    DrawItemMiddle(() =>
                    {
                        DrawRotationCombo(comboSize, rotations, rotation, gameVersion);
                    }, wholeWidth, comboSize);
                }
            }
            else
            {
                DrawItemMiddle(() =>
                {
                    DrawRotationIcon(rotation, iconSize);

                    ImGui.SameLine();

                    ImGui.BeginGroup();

                    DrawRotationCombo(comboSize, rotations, rotation, gameVersion);
                    ImGui.TextDisabled(slash);
                    ImGui.SameLine();

                    if (drawCenter)
                    {
                        ImGui.TextDisabled(gameVersion);
                        ImGui.SameLine();
                    }
                    ImGui.Text(rotation.GameVersion);
                    ImGui.EndGroup();
                }, wholeWidth, horizonalWholeWidth);
            }
        }
    }

    private void DrawRotationIcon(ICustomRotation rotation, float iconSize)
    {
        if (rotation.GetTexture(out var jobIcon) && SilenceImageButton(jobIcon.ImGuiHandle,
            Vector2.One * iconSize, _activeTab == RotationConfigWindowTab.Rotation))
        {
            _activeTab = RotationConfigWindowTab.Rotation;
        }
        var desc = rotation.Name + $" ({rotation.RotationName})";
        if (!string.IsNullOrEmpty(rotation.Description)) desc += "\n \n" + rotation.Description;
        ImguiTooltips.HoveredTooltip(desc);
    }

    private static void DrawRotationCombo(float comboSize, ICustomRotation[] rotations, ICustomRotation rotation, string gameVersion)
    {
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
                    Service.Config.GetJobConfig(Job).RotationChoice = r.GetType().FullName;
                    Service.Config.Save();
                }
                ImguiTooltips.HoveredTooltip(r.Description);
                ImGui.PopStyleColor();
            }
            ImGui.EndCombo();
        }

        var warning = gameVersion + rotation.GameVersion;
        if (!rotation.IsValid) warning += "\n" + string.Format(LocalizationManager.RightLang.ConfigWindow_Rotation_InvalidRotation,
                rotation.GetType().Assembly.GetInfo().Author);

        if (rotation.IsBeta()) warning += "\n" + LocalizationManager.RightLang.ConfigWindow_Rotation_BetaRotation;

        warning += "\n \n" + LocalizationManager.RightLang.ConfigWindow_Helper_SwitchRotation;
        ImguiTooltips.HoveredTooltip(warning);
    }

    private static void DrawItemMiddle(System.Action drawAction, float wholeWidth, float width, bool leftAlign = true)
    {
        if (drawAction == null) return;
        var distance = (wholeWidth - width) / 2;
        if (leftAlign) distance = MathF.Max(distance, 0);
        ImGui.SetCursorPosX(distance);
        drawAction();
    }

    private void DrawBody()
    {
        ImGui.SetCursorPos(ImGui.GetCursorPos() + Vector2.One * 8 * _scale);
        if (BeginChild("Rotation Solver Body", -Vector2.One))
        {
            if (_searchResults != null && _searchResults.Any())
            {
                ImGui.PushFont(ImGuiHelper.GetFont(18));
                ImGui.PushStyleColor(ImGuiCol.Text, ImGui.ColorConvertFloat4ToU32(ImGuiColors.DalamudYellow));
                ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_Search_Result);
                ImGui.PopStyleColor();
                ImGui.PopFont();
                ImGui.Spacing();

                foreach (var searchable in _searchResults)
                {
                    searchable?.Draw(Job, true);
                }
            }
            else
            {
                switch (_activeTab)
                {
                    case RotationConfigWindowTab.About:
                        DrawAbout();
                        break;

                    case RotationConfigWindowTab.Rotation:
                        DrawRotation();
                        break;

                    case RotationConfigWindowTab.Actions:
                        DrawActions();
                        break;

                    case RotationConfigWindowTab.Rotations:
                        DrawRotations();
                        break;

                    case RotationConfigWindowTab.List:
                        DrawList();
                        break;

                    case RotationConfigWindowTab.Basic:
                        DrawBasic();
                        break;

                    case RotationConfigWindowTab.UI:
                        DrawUI();
                        break;

                    case RotationConfigWindowTab.Auto:
                        DrawAuto();
                        break;

                    case RotationConfigWindowTab.Target:
                        DrawTarget();
                        break;

                    case RotationConfigWindowTab.Extra:
                        DrawExtra();
                        break;

                    case RotationConfigWindowTab.Debug:
                        DrawDebug();
                        break;
                }
            }

            ImGui.EndChild();
        }
    }

    #region About
    private static void DrawAbout()
    {
        ImGui.PushFont(ImGuiHelper.GetFont(18));
        ImGui.PushStyleColor(ImGuiCol.Text, ImGui.ColorConvertFloat4ToU32(ImGuiColors.DalamudYellow));
        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_About_Punchline);
        ImGui.PopStyleColor();
        ImGui.PopFont();
        ImGui.Spacing();

        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_About_Description);

        ImGui.Spacing();
        ImGui.PushStyleColor(ImGuiCol.Text, ImGui.ColorConvertFloat4ToU32(ImGuiColors.DalamudOrange));
        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_About_Warning);
        ImGui.PopStyleColor();

        var width = ImGui.GetWindowWidth();
        if (IconSet.GetTexture("https://discordapp.com/api/guilds/1064448004498653245/embed.png?style=banner2", out var icon) && TextureButton(icon, width, width))
        {
            Util.OpenLink("https://discord.gg/4fECHunam9");
        }

        _aboutHeaders.Draw();
    }

    private static readonly CollapsingHeaderGroup _aboutHeaders = new (new ()
    {
        { () => LocalizationManager.RightLang.ConfigWindow_About_Macros, DrawAboutMacros},
        { () => LocalizationManager.RightLang.ConfigWindow_About_Compatibility, DrawAboutCompatibility},
        { () => LocalizationManager.RightLang.ConfigWindow_About_Links, DrawAboutLinks},
    });

    private static void DrawAboutMacros()
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
    }

    private static void DrawAboutCompatibility()
    {
        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_About_Compatibility_Description);

        ImGui.Spacing();

        var iconSize = 40 * _scale;

        if (ImGui.BeginTable("Incompatible plugin", 4, ImGuiTableFlags.BordersInner
        | ImGuiTableFlags.Resizable
        | ImGuiTableFlags.SizingStretchProp))
        {
            ImGui.TableSetupScrollFreeze(0, 1);
            ImGui.TableNextRow(ImGuiTableRowFlags.Headers);

            ImGui.TableNextColumn();
            ImGui.TableHeader("Name");

            ImGui.TableNextColumn();
            ImGui.TableHeader("Icon/Link");

            ImGui.TableNextColumn();
            ImGui.TableHeader("Features");

            ImGui.TableNextColumn();
            ImGui.TableHeader("Type");

            foreach (var item in DownloadHelper.IncompatiblePlugins ?? Array.Empty<IncompatiblePlugin>())
            {
                ImGui.TableNextRow();
                ImGui.TableNextColumn();

                ImGui.Text(item.Name);

                ImGui.TableNextColumn();

                var icon = item.Icon;
                if (string.IsNullOrEmpty(icon)) icon = "https://raw.githubusercontent.com/goatcorp/DalamudAssets/master/UIRes/defaultIcon.png";

                if (IconSet.GetTexture(icon, out var texture))
                {
                    if (NoPaddingNoColorImageButton(texture.ImGuiHandle, Vector2.One * iconSize))
                    {
                        Util.OpenLink(item.Url);
                    }
                }

                ImGui.TableNextColumn();
                ImGui.TextWrapped(item.Features);

                ImGui.TableNextColumn();

                if (item.Type.HasFlag(CompatibleType.Skill_Usage))
                {
                    ImGui.TextColored(ImGuiColors.DalamudYellow, CompatibleType.Skill_Usage.ToString().Replace('_', ' '));
                    ImguiTooltips.HoveredTooltip(LocalizationManager.RightLang.ConfigWindow_About_Compatibility_Mistake);
                }
                if (item.Type.HasFlag(CompatibleType.Skill_Selection))
                {
                    ImGui.TextColored(ImGuiColors.DalamudOrange, CompatibleType.Skill_Selection.ToString().Replace('_', ' '));
                    ImguiTooltips.HoveredTooltip(LocalizationManager.RightLang.ConfigWindow_About_Compatibility_Mislead);
                }
                if (item.Type.HasFlag(CompatibleType.Crash))
                {
                    ImGui.TextColored(ImGuiColors.DalamudRed, CompatibleType.Crash.ToString().Replace('_', ' '));
                    ImguiTooltips.HoveredTooltip(LocalizationManager.RightLang.ConfigWindow_About_Compatibility_Crash);
                }
            }
            ImGui.EndTable();
        }

    }

    private static void DrawAboutLinks()
    {
        var width = ImGui.GetWindowWidth();

        if (IconSet.GetTexture("https://GitHub-readme-stats.vercel.app/api/pin/?username=ArchiDog1998&repo=RotationSolver&theme=dark", out var icon) && TextureButton(icon, width, width))
        {
            Util.OpenLink("https://GitHub.com/ArchiDog1998/RotationSolver");
        }

        if (IconSet.GetTexture("https://badges.crowdin.net/badge/light/crowdin-on-dark.png", out icon) && TextureButton(icon, width, width))
        {
            Util.OpenLink("https://crowdin.com/project/rotationsolver");
        }
    }
    #endregion

    #region Rotation
    private static void DrawRotation()
    {
        var rotation = RotationUpdater.RightNowRotation;
        if (rotation == null) return;

        var desc = rotation.Description;
        if (!string.IsNullOrEmpty(desc))
        {
            ImGui.PushFont(ImGuiHelper.GetFont(15));
            ImGuiEx.TextWrappedCopy(desc);
            ImGui.PopFont();
        }

        var wholeWidth = ImGui.GetWindowWidth();
        var type = rotation.GetType();
        var info = type.Assembly.GetInfo();

        if (!string.IsNullOrEmpty(rotation.WhyNotValid))
        {
            var author = info.Author;
            if (string.IsNullOrEmpty(author)) author = "Author";
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DPSRed);
            ImGui.TextWrapped(string.Format(rotation.WhyNotValid, author));
            ImGui.PopStyleColor();
        }

        if (!string.IsNullOrEmpty(info.DonateLink))
        {
            if (IconSet.GetTexture("https://storage.ko-fi.com/cdn/brandasset/kofi_button_red.png", out var icon) && TextureButton(icon, wholeWidth, 250 * _scale, "Ko-fi link"))
            {
                Util.OpenLink(info.DonateLink);
            }
        }

        _rotationHeader.Draw();
    }
   
    private static readonly CollapsingHeaderGroup _rotationHeader = new(new()
    {
        { () => LocalizationManager.RightLang.ConfigWindow_Rotation_Description, DrawRotationDescription },

        { GetRotationStatusHead ,  DrawRotationStatus },

        { () => LocalizationManager.RightLang.ConfigWindow_Rotation_Configuration, DrawRotationConfiguration },

        { () => LocalizationManager.RightLang.ConfigWindow_Rotation_Information, DrawRotationInformation },
    });

    private const float DESC_SIZE = 24;
    private static void DrawRotationDescription()
    {
        var rotation = RotationUpdater.RightNowRotation;
        if (rotation == null) return;

        var wholeWidth = ImGui.GetWindowWidth();
        var type = rotation.GetType();

        var attrs = new List<RotationDescAttribute> { RotationDescAttribute.MergeToOne(type.GetCustomAttributes<RotationDescAttribute>()) };

        foreach (var m in type.GetAllMethodInfo())
        {
            attrs.Add(RotationDescAttribute.MergeToOne(m.GetCustomAttributes<RotationDescAttribute>()));
        }

        if (ImGui.BeginTable("Rotation Description", 2, ImGuiTableFlags.Borders
            | ImGuiTableFlags.Resizable
            | ImGuiTableFlags.SizingStretchProp))
        {
            foreach (var a in RotationDescAttribute.Merge(attrs))
            {
                var attr = RotationDescAttribute.MergeToOne(a);
                if (attr == null) continue;

                var allActions = attr.Actions.Select(i => rotation.AllBaseActions
                .FirstOrDefault(a => a.ID == (uint)i))
                .Where(i => i != null);

                bool hasDesc = !string.IsNullOrEmpty(attr.Description);

                if (!hasDesc && !allActions.Any()) continue;

                ImGui.TableNextRow();
                ImGui.TableNextColumn();

                if(IconSet.GetTexture(attr.IconID, out var image)) ImGui.Image(image.ImGuiHandle, Vector2.One * DESC_SIZE * _scale);

                ImGui.SameLine();
                var isOnCommand = attr.IsOnCommand;
                if (isOnCommand) ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudYellow);
                ImGui.Text(" " + attr.Type.ToName());
                if (isOnCommand) ImGui.PopStyleColor();

                ImGui.TableNextColumn();

                if (hasDesc)
                {
                    ImGui.Text(attr.Description);
                }

                bool notStart = false;
                foreach (var item in allActions)
                {
                    if (item == null) continue;

                    if (notStart)
                    {
                        ImGui.SameLine();
                        ImGui.Text(" ");
                        ImGui.SameLine();
                    }

                    if(item.GetTexture(out var texture))
                    {
                        ControlWindow.DrawIAction(texture.ImGuiHandle, DESC_SIZE * _scale, 1);
                        ImguiTooltips.HoveredTooltip(item.Name);
                    }
                    notStart = true;
                }
            }
            ImGui.EndTable();
        }

        var links = type.GetCustomAttributes<LinkDescriptionAttribute>();

        foreach (var link in links)
        {
            DrawLinkDescription(link.LinkDescription, wholeWidth, true);
        }
    }

    internal static void DrawLinkDescription(LinkDescription link, float wholeWidth, bool drawQuestion)
    {
        var hasTexture = IconSet.GetTexture(link.Url, out var texture);

        if (hasTexture && TextureButton(texture, wholeWidth, wholeWidth))
        {
            Util.OpenLink(link.Url);
        }

        ImGui.TextWrapped(link.Description);

        if (drawQuestion && !hasTexture && !string.IsNullOrEmpty(link.Url))
        {
            if (ImGuiEx.IconButton(FontAwesomeIcon.Question, link.Description))
            {
                Util.OpenLink(link.Url);
            }
        }
    }

    private static string GetRotationStatusHead()
    {
        var rotation = RotationUpdater.RightNowRotation;
        if (rotation == null || !rotation.ShowStatus) return string.Empty;
        return LocalizationManager.RightLang.ConfigWindow_Rotation_Status;
    }

    private static void DrawRotationStatus()
    {
        RotationUpdater.RightNowRotation?.DisplayStatus();
    }

    private static string ToCommandStr(OtherCommandType type, string str, string extra = "")
    {
        var result = Service.Command + " " + type.ToString() + " " + str;
        if (!string.IsNullOrEmpty(extra)) result += " " + extra;
        return result;
    }
    private static void DrawRotationConfiguration()
    {
        var rotation = RotationUpdater.RightNowRotation;
        if (rotation == null) return;

        var enable = rotation.IsEnabled;
        if(ImGui.Checkbox(rotation.Name, ref  enable))
        {
            rotation.IsEnabled = enable;
        }
        if (!enable) return;

        var set = rotation.Configs;

        if(set.Any()) ImGui.Separator();

        foreach (var config in set.Configs)
        {
            var key = config.Name;
            var name = $"##{config.GetHashCode()}_{config.Name}";
            string command = ToCommandStr(OtherCommandType.Rotations,config.Name,  config.DefaultValue);
            void Reset() => set.SetValue(config.Name, config.DefaultValue);

            Searchable.PrepareGroup(key, command, Reset);

            if (config is RotationConfigCombo c)
            {
                var val = set.GetCombo(c.Name);
                ImGui.SetNextItemWidth(ImGui.CalcTextSize(c.Items[val]).X + 50 * _scale);
                var openCombo = ImGui.BeginCombo(name, c.Items[val]);
                Searchable.ReactPopup(key, command, Reset);
                if (openCombo)
                {
                    for (int comboIndex = 0; comboIndex < c.Items.Length; comboIndex++)
                    {
                        if (ImGui.Selectable(c.Items[comboIndex]))
                        {
                            set.SetValue(config.Name, comboIndex.ToString());
                        }
                    }
                    ImGui.EndCombo();
                }
            }
            else if (config is RotationConfigBoolean b)
            {
                bool val = set.GetBool(config.Name);

                if (ImGui.Checkbox(name, ref val))
                {
                    set.SetValue(config.Name, val.ToString());
                }
                Searchable.ReactPopup(key, command, Reset);
            }
            else if (config is RotationConfigFloat f)
            {
                float val = set.GetFloat(config.Name);
                ImGui.SetNextItemWidth(_scale * Searchable.DRAG_WIDTH);
                if (ImGui.DragFloat(name, ref val, f.Speed, f.Min, f.Max))
                {
                    set.SetValue(config.Name, val.ToString());
                }
                Searchable.ReactPopup(key, command, Reset);
            }
            else if (config is RotationConfigString s)
            {
                string val = set.GetString(config.Name);

                ImGui.SetNextItemWidth(ImGui.GetWindowWidth());
                if (ImGui.InputTextWithHint(name, config.DisplayName, ref val, 128))
                {
                    set.SetValue(config.Name, val.ToString());
                }
                Searchable.ReactPopup(key, command, Reset);
                continue;
            }
            else if (config is RotationConfigInt i)
            {
                int val = set.GetInt(config.Name);
                ImGui.SetNextItemWidth(_scale * Searchable.DRAG_WIDTH);
                if (ImGui.DragInt(name, ref val, i.Speed, i.Min, i.Max))
                {
                    set.SetValue(config.Name, val.ToString());
                }
                Searchable.ReactPopup(key, command, Reset);
            }
            else continue;

            ImGui.SameLine();
            ImGui.TextWrapped(config.DisplayName);
            Searchable.ReactPopup(key, command, Reset, false);
        }
    }

    private static void DrawRotationInformation()
    {
        var rotation = RotationUpdater.RightNowRotation;
        if (rotation == null) return;

        var youtubeLink = rotation.GetType().GetCustomAttribute<YoutubeLinkAttribute>()?.ID;

        var wholeWidth = ImGui.GetWindowWidth();
        if (!string.IsNullOrEmpty(youtubeLink))
        {
            ImGui.NewLine();
            if (IconSet.GetTexture("https://www.gstatic.com/youtube/img/branding/youtubelogo/svg/youtubelogo.svg", out var icon) && TextureButton(icon, wholeWidth, 250 * _scale, "Youtube Link"))
            {
                Util.OpenLink("https://youtu.be/" + youtubeLink);
            }
        }

        var assembly = rotation.GetType().Assembly;
        var info = assembly.GetInfo();

        if (info != null)
        {
            ImGui.NewLine();

            var link = rotation.GetType().GetCustomAttribute<SourceCodeAttribute>();
            if (link != null)
            {
                var userName = info.GitHubUserName;
                var repository = info.GitHubRepository;
                DrawGitHubBadge(userName, repository, $"https://github.com/{userName}/{repository}/blob/{link.Path}", center: true);
            }
            ImGui.NewLine();

            DrawItemMiddle(() =>
            {
                ImGui.BeginGroup();
                if (ImGui.Button(info.Name))
                {
                    Process.Start("explorer.exe", "/select, \"" + info.FilePath + "\"");
                }

                var version = assembly.GetName().Version;
                if (version != null)
                {
                    ImGui.Text(" v " + version.ToString());
                }
                ImGui.Text(" - " + info.Author);
                ImGui.EndGroup();
            }, wholeWidth, _groupWidth);

            _groupWidth = ImGui.GetItemRectSize().X;
        }
    }

    private static float _groupWidth = 100;
    #endregion

    #region Actions
    private static unsafe void DrawActions()
    {
        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_Actions_Description);

        if (ImGui.BeginTable("Rotation Solver Actions", 2, ImGuiTableFlags.Resizable))
        {
            ImGui.TableSetupColumn("Action Column", ImGuiTableColumnFlags.WidthFixed, ImGui.GetWindowWidth() / 2);
            ImGui.TableNextColumn();

            if (_actionsList != null)
            {
                _actionsList.ClearCollapsingHeader();

                if (RotationUpdater.RightNowRotation != null)
                {
                    var size = 30 * _scale;
                    var count = Math.Max(1, (int)MathF.Floor(ImGui.GetColumnWidth() / (size * 1.1f + ImGui.GetStyle().ItemSpacing.X)));
                    foreach (var pair in RotationUpdater.AllGroupedActions)
                    {
                        _actionsList.AddCollapsingHeader(() => pair.Key, () =>
                        {
                            var index = 0;
                            foreach (var item in pair.OrderBy(t => t.ID))
                            {
                                if (!item.GetTexture(out var icon)) continue;

                                if (index++ % count != 0)
                                {
                                    ImGui.SameLine();
                                }

                                ImGui.BeginGroup();
                                var cursor = ImGui.GetCursorPos();
                                if (NoPaddingNoColorImageButton(icon.ImGuiHandle, Vector2.One * size, item.Name))
                                {
                                    _activeAction = item;
                                }
                                DrawActionOverlay(cursor, size, _activeAction == item ? 1 : 0);

                                if(IconSet.GetTexture("ui/uld/readycheck_hr1.tex", out var texture))
                                {
                                    var offset = new Vector2(1 / 12f, 1 / 6f);
                                    ImGui.SetCursorPos(cursor + new Vector2(0.6f, 0.7f) * size);
                                    ImGui.Image(texture.ImGuiHandle, Vector2.One * size * 0.5f, 
                                        new Vector2(item.IsEnabled ? 0 : 0.5f, 0) + offset,
                                        new Vector2(item.IsEnabled ? 0.5f : 1, 1) - offset);
                                }
                                ImGui.EndGroup();

                                string key = $"Action Macro Usage {item.Name} {item.ID}";
                                var cmd = ToCommandStr(OtherCommandType.DoActions, $"{item}-{5}");
                                Searchable.DrawHotKeysPopup(key, cmd);
                                Searchable.ExecuteHotKeysPopup(key, cmd, item.Name, false);
                            }
                        });
                    }
                }

                _actionsList.Draw();
            }

            ImGui.TableNextColumn();

            if (Service.Config.GetValue(PluginConfigBool.InDebug))
            {
                if(_activeAction is IBaseAction action)
                {
                    try
                    {
#if DEBUG
                        ImGui.Text("Is Real GCD: " + action.IsRealGCD.ToString());
                        ImGui.Text("Status: " + FFXIVClientStructs.FFXIV.Client.Game.ActionManager.Instance()->GetActionStatus(FFXIVClientStructs.FFXIV.Client.Game.ActionType.Spell, action.AdjustedID).ToString());
                        ImGui.Text("Cast Time: " + action.CastTime.ToString());
                        ImGui.Text("MP: " + action.MPNeed.ToString());
#endif
                        ImGui.Text("AttackType: " + action.AttackType.ToString());
                        ImGui.Text("Aspect: " + action.Aspect.ToString());
                        ImGui.Text("Has One:" + action.HasOneCharge.ToString());
                        ImGui.Text("Recast One: " + action.RecastTimeOneChargeRaw.ToString());
                        ImGui.Text("Recast Elapsed: " + action.RecastTimeElapsedRaw.ToString());

                        var option = CanUseOption.IgnoreTarget | CanUseOption.IgnoreClippingCheck;
                        ImGui.Text($"Can Use: {action.CanUse(out _, option)} ");
                        ImGui.Text("Must Use:" + action.CanUse(out _, option | CanUseOption.MustUse).ToString());
                        ImGui.Text("Empty Use:" + action.CanUse(out _, option | CanUseOption.EmptyOrSkipCombo).ToString());
                        ImGui.Text("Must & Empty Use:" + action.CanUse(out _, option | CanUseOption.MustUseEmpty).ToString());
                        if (action.Target != null)
                        {
                            ImGui.Text("Target Name: " + action.Target.Name);
                        }
                    }
                    catch
                    {

                    }
                }
                else if (_activeAction is IBaseItem item)
                {
                    try
                    {
                        ImGui.Text("Status: " + FFXIVClientStructs.FFXIV.Client.Game.ActionManager.Instance()->GetActionStatus(FFXIVClientStructs.FFXIV.Client.Game.ActionType.Item, item.ID).ToString());
                        ImGui.Text("Status HQ: " + FFXIVClientStructs.FFXIV.Client.Game.ActionManager.Instance()->GetActionStatus(FFXIVClientStructs.FFXIV.Client.Game.ActionType.Item, item.ID + 1000000).ToString());
                        var remain = FFXIVClientStructs.FFXIV.Client.Game.ActionManager.Instance()->GetRecastTime(FFXIVClientStructs.FFXIV.Client.Game.ActionType.Item, item.ID) - FFXIVClientStructs.FFXIV.Client.Game.ActionManager.Instance()->GetRecastTimeElapsed(FFXIVClientStructs.FFXIV.Client.Game.ActionType.Item, item.ID);
                        ImGui.Text("remain: " + remain.ToString());
                        ImGui.Text("CanUse: " + item.CanUse(out _, true).ToString());

                        if (item is HealPotionItem healPotionItem)
                        {
                            ImGui.Text("MaxHP:" + healPotionItem.MaxHealHp.ToString());
                        }
                    }
                    catch
                    {

                    }
                }
            }


            if (_activeAction != null)
            {
                var enable = _activeAction.IsEnabled;
                if (ImGui.Checkbox($"{_activeAction.Name}##{_activeAction.Name} Enabled", ref enable))
                {
                    _activeAction.IsEnabled = enable;
                }

                const string key = "Action Enable Popup";
                var cmd = ToCommandStr(OtherCommandType.ToggleActions, _activeAction.ToString());
                Searchable.DrawHotKeysPopup(key, cmd);
                Searchable.ExecuteHotKeysPopup(key, cmd, string.Empty, false);

                enable = _activeAction.IsInCooldown;
                if (ImGui.Checkbox($"{LocalizationManager.RightLang.ConfigWindow_Actions_ShowOnCDWindow}##{_activeAction.Name}InCooldown", ref enable))
                {
                    _activeAction.IsInCooldown = enable;
                }

                ImGui.Separator();
            }

            ActionSequencerUpdater.DrawHeader(30 * _scale);
            if (_sequencerList != null)
            {
                _sequencerList.Draw();
            }

            ImGui.EndTable();
        }
    }

    private static IAction _activeAction;

    private static readonly CollapsingHeaderGroup _actionsList = new()
    {
         HeaderSize = 18,
    };

    private static readonly CollapsingHeaderGroup _sequencerList = new(new()
    {
        { () => LocalizationManager.RightLang.ConfigWindow_Actions_ForcedConditionSet, () =>
        {
            ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_Actions_ForcedConditionSet_Description);

            var rotation = RotationUpdater.RightNowRotation;
            var set = ActionSequencerUpdater.RightSet;

            if (set == null || _activeAction == null || rotation == null) return;
            set.DrawCondition(_activeAction.ID, rotation);
        } },

        { () => LocalizationManager.RightLang.ConfigWindow_Actions_DisabledConditionSet, () =>
        {
            ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_Actions_DisabledConditionSet_Description);

            var rotation = RotationUpdater.RightNowRotation;
            var set = ActionSequencerUpdater.RightSet;

            if (set == null || _activeAction == null || rotation == null) return;
            set.DrawDisabledCondition(_activeAction.ID, rotation);
        } },
    })
    {
        HeaderSize = 18,
    };
    #endregion

    #region Rotations
    private static void DrawRotations()
    {
        if (ImGui.Button("Download Rotations"))
        {
            Task.Run(async () =>
            {
                await RotationUpdater.GetAllCustomRotationsAsync(DownloadOption.MustDownload | DownloadOption.ShowList);
            });
        }

        _rotationsHeader?.Draw();
    }
    private static readonly CollapsingHeaderGroup _rotationsHeader = new(new()
    {
        {() => LocalizationManager.RightLang.ConfigWindow_Rotations_Settings, DrawRotationsSettings},
        {() => LocalizationManager.RightLang.ConfigWindow_Rotations_Loaded, DrawRotationsLoaded},
        {() => LocalizationManager.RightLang.ConfigWindow_Rotations_GitHub, DrawRotationsGitHub},
        {() => LocalizationManager.RightLang.ConfigWindow_Rotations_Libraries, DrawRotationsLibraries},
    });

    private static readonly CheckBoxSearchPlugin _updateRotation = new (PluginConfigBool.DownloadRotations,
        new CheckBoxSearchPlugin(PluginConfigBool.AutoUpdateRotations));

    private static readonly CheckBoxSearchPlugin _autoLoadRotations = new(PluginConfigBool.AutoLoadCustomRotations);

    private static void DrawRotationsSettings()
    {
        _updateRotation?.Draw(Job);
        _autoLoadRotations?.Draw(Job);
    }

    private static void DrawRotationsLoaded()
    {
        var assemblyGrps = RotationUpdater.CustomRotationsDict
            .SelectMany(d => d.Value)
            .SelectMany(g => g.Rotations)
            .GroupBy(r => r.GetType().Assembly);


        if (ImGui.BeginTable("Rotation Solver AssemblyTable", 3, ImGuiTableFlags.BordersInner
            | ImGuiTableFlags.Resizable
            | ImGuiTableFlags.SizingStretchProp))
        {
            ImGui.TableSetupScrollFreeze(0, 1);
            ImGui.TableNextRow(ImGuiTableRowFlags.Headers);

            ImGui.TableNextColumn();
            ImGui.TableHeader("Information");

            ImGui.TableNextColumn();
            ImGui.TableHeader("Rotations");

            ImGui.TableNextColumn();
            ImGui.TableHeader("Links");

            foreach (var grp in assemblyGrps)
            {
                ImGui.TableNextRow();

                var assembly = grp.Key;

                var info = assembly.GetInfo();
                ImGui.TableNextColumn();

                if (ImGui.Button(info.Name))
                {
                    Process.Start("explorer.exe", "/select, \"" + info.FilePath + "\"");
                }

                var version = assembly.GetName().Version;
                if (version != null)
                {
                    ImGui.Text(" v " + version.ToString());
                }

                ImGui.Text(" - " + info.Author);

                ImGui.TableNextColumn();

                var lastRole = JobRole.None;
                foreach (var jobs in grp.GroupBy(r => r.IconID))
                {
                    var role = jobs.FirstOrDefault().ClassJob.GetJobRole();
                    if (lastRole == role && lastRole != JobRole.None) ImGui.SameLine();
                    lastRole = role;

                    if(IconSet.GetTexture(IconSet.GetJobIcon(jobs.First(), IconType.Framed), out var texture, 62574))
                    ImGui.Image(texture.ImGuiHandle, Vector2.One * 30 * _scale);

                    ImguiTooltips.HoveredTooltip(string.Join('\n', jobs));
                }

                ImGui.TableNextColumn();

                DrawGitHubBadge(info.GitHubUserName, info.GitHubRepository, info.FilePath);

                if (!string.IsNullOrEmpty(info.DonateLink)
                    && IconSet.GetTexture("https://storage.ko-fi.com/cdn/brandasset/kofi_button_red.png", out var icon)
                    && NoPaddingNoColorImageButton(icon.ImGuiHandle, new Vector2(1, (float)icon.Height/ icon.Width) * MathF.Min(250, icon.Width) * _scale, info.FilePath))
                {
                    Util.OpenLink(info.DonateLink);
                }
            }
            ImGui.EndTable();
        }
    }

    private static void DrawGitHubBadge(string userName, string repository, string id = "", string link = "", bool center = false)
    {
        if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(repository)
            && IconSet.GetTexture($"https://github-readme-stats.vercel.app/api/pin/?username={userName}&repo={repository}&theme=dark", out var icon)
            && (center ? TextureButton(icon, ImGui.GetWindowWidth(), icon.Width, id)
            : NoPaddingNoColorImageButton(icon.ImGuiHandle, new Vector2(icon.Width, icon.Height), id)))
        {
            Util.OpenLink(string.IsNullOrEmpty(link) ? $"https://GitHub.com/{userName}/{repository}" : link);
        }
    }

    private static void DrawRotationsGitHub()
    {
        if (!Service.Config.GlobalConfig.GitHubLibs.Any(s => string.IsNullOrEmpty(s) || s == "||"))
        {
            Service.Config.GlobalConfig.GitHubLibs = Service.Config.GlobalConfig.GitHubLibs.Append("||").ToArray();
        }

        ImGui.Spacing();

        int removeIndex = -1;
        for (int i = 0; i < Service.Config.GlobalConfig.GitHubLibs.Length; i++)
        {
            var strs = Service.Config.GlobalConfig.GitHubLibs[i].Split('|');
            var userName = strs.FirstOrDefault() ?? string.Empty;
            var repository = strs.Length > 1 ? strs[1] : string.Empty;
            var fileName = strs.LastOrDefault() ?? string.Empty;

            DrawGitHubBadge(userName, repository, fileName);

            var changed = false;

            var width = ImGui.GetWindowWidth() - ImGuiEx.CalcIconSize(FontAwesomeIcon.Ban).X - ImGui.GetStyle().ItemSpacing.X * 3 - 10 * _scale;
            width /= 3;

            ImGui.SetNextItemWidth(width);
            changed |= ImGui.InputTextWithHint($"##GitHubLib{i}UserName", LocalizationManager.RightLang.ConfigWindow_Rotations_UserName, ref userName, 1024);
            ImGui.SameLine();

            ImGui.SetNextItemWidth(width);
            changed |= ImGui.InputTextWithHint($"##GitHubLib{i}Repository", LocalizationManager.RightLang.ConfigWindow_Rotations_Repository, ref repository, 1024);
            ImGui.SameLine();

            ImGui.SetNextItemWidth(width);
            changed |= ImGui.InputTextWithHint($"##GitHubLib{i}FileName", LocalizationManager.RightLang.ConfigWindow_Rotations_FileName, ref fileName, 1024);
            ImGui.SameLine();

            if(changed)
            {
                Service.Config.GlobalConfig.GitHubLibs[i] = $"{userName}|{repository}|{fileName}";
            }

            if (ImGuiEx.IconButton(FontAwesomeIcon.Ban, $"##Rotation Solver Remove GitHubLibs{i}"))
            {
                removeIndex = i;
            }
        }
        if (removeIndex > -1)
        {
            var list = Service.Config.GlobalConfig.GitHubLibs.ToList();
            list.RemoveAt(removeIndex);
            Service.Config.GlobalConfig.GitHubLibs = list.ToArray();
        }
    }

    private static void DrawRotationsLibraries()
    {
        if (!Service.Config.GlobalConfig.OtherLibs.Any(string.IsNullOrEmpty))
        {
            Service.Config.GlobalConfig.OtherLibs = Service.Config.GlobalConfig.OtherLibs.Append(string.Empty).ToArray();
        }

        ImGui.Spacing();

        var width = ImGui.GetWindowWidth() - ImGuiEx.CalcIconSize(FontAwesomeIcon.Ban).X - ImGui.GetStyle().ItemSpacing.X - 10 * _scale;

        int removeIndex = -1;
        for (int i = 0; i < Service.Config.GlobalConfig.OtherLibs.Length; i++)
        {
            ImGui.SetNextItemWidth(width);
            ImGui.InputTextWithHint($"##Rotation Solver OtherLib{i}", LocalizationManager.RightLang.ConfigWindow_Rotations_Library, ref Service.Config.GlobalConfig.OtherLibs[i], 1024);
            ImGui.SameLine();

            if (ImGuiEx.IconButton(FontAwesomeIcon.Ban, $"##Rotation Solver Remove OtherLibs{i}"))
            {
                removeIndex = i;
            }
        }
        if (removeIndex > -1)
        {
            var list = Service.Config.GlobalConfig.OtherLibs.ToList();
            list.RemoveAt(removeIndex);
            Service.Config.GlobalConfig.OtherLibs = list.ToArray();
        }
    }
    #endregion 

    #region List
    private static TerritoryType[] _allTerritories = null;
    internal static TerritoryType[] AllTerritories
    {
        get
        {
            _allTerritories ??= Service.GetSheet<TerritoryType>()
                    .Where(t => t != null
                        && t.ContentFinderCondition?.Value?.ContentType?.Value?.RowId != 0)
                    .OrderBy(t => t.ContentFinderCondition?.Value?.ContentType?.Value?.RowId)
                    .ToArray();
            return _allTerritories;
        }
    }

    private static Status[] _allDispelStatus = null;
    internal static Status[] AllDispelStatus
    {
        get
        {
            _allDispelStatus ??= Service.GetSheet<Status>()
                    .Where(s => s.CanDispel)
                    .ToArray();
            return _allDispelStatus;
        }
    }

    private static Status[] _allInvStatus = null;
    internal static Status[] AllInvStatus
    {
        get
        {
            _allInvStatus ??= Service.GetSheet<Status>()
                    .Where(s => !s.CanDispel && !s.LockMovement && !s.IsGaze && !s.IsFcBuff && s.HitEffect.Row == 16 && s.ClassJobCategory.Row == 1 && s.StatusCategory == 1
                        && !string.IsNullOrEmpty(s.Name.ToString()) && s.Icon != 0)
                    .ToArray();
            return _allInvStatus;
        }
    }

    private static GAction[] _allActions = null;
    internal static GAction[] AllActions
    {
        get
        {
            _allActions ??= Service.GetSheet<GAction>()
                    .Where(a => !string.IsNullOrEmpty(a.Name) && !a.IsPvP && !a.IsPlayerAction
                    && a.ClassJob.Value == null && a.Cast100ms > 0)
                    .ToArray();
            return _allActions;
        }
    }

    private static void DrawList()
    {
        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_List_Description); 
        _idsHeader?.Draw();
    }
    private static readonly CollapsingHeaderGroup _idsHeader = new(new()
    {
        { () => LocalizationManager.RightLang.ConfigWindow_List_Statuses, DrawActionsStatuses},
        { () => Service.Config.GetValue(PluginConfigBool.UseDefenseAbility) ? LocalizationManager.RightLang.ConfigWindow_List_Actions : string.Empty, DrawListActions},
        { () => LocalizationManager.RightLang.ConfigWindow_List_Territories, DrawListTerritories},
    });
    private static void DrawActionsStatuses()
    {
        ImGui.SetNextItemWidth(ImGui.GetWindowWidth());
        ImGui.InputTextWithHint("##Searching the action", LocalizationManager.RightLang.ConfigWindow_List_StatusNameOrId, ref _statusSearching, 128);

        if (ImGui.BeginTable("Rotation Solver List Statuses", 2, ImGuiTableFlags.BordersInner | ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingStretchSame))
        {
            ImGui.TableSetupScrollFreeze(0, 1);
            ImGui.TableNextRow(ImGuiTableRowFlags.Headers);

            ImGui.TableNextColumn();
            ImGui.TableHeader(LocalizationManager.RightLang.ConfigWindow_List_Invincibility);

            ImGui.TableNextColumn();
            ImGui.TableHeader(LocalizationManager.RightLang.ConfigWindow_List_DangerousStatus);

            ImGui.TableNextRow();

            ImGui.TableNextColumn();

            ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_List_InvincibilityDesc);
            DrawStatusList(nameof(OtherConfiguration.InvincibleStatus), OtherConfiguration.InvincibleStatus, AllInvStatus);

            ImGui.TableNextColumn();

            ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_List_DangerousStatusDesc);

            DrawStatusList(nameof(OtherConfiguration.DangerousStatus), OtherConfiguration.DangerousStatus, AllDispelStatus);

            ImGui.EndTable();
        }
    }

    static string _statusSearching = string.Empty;
    private static void DrawStatusList(string name, HashSet<uint> statuses, Status[] allStatus)
    {
        uint removeId = 0;
        uint notLoadId = 10100;

        var popupId = "Rotation Solver Popup" + name;

        StatusPopUp(popupId, allStatus, ref _statusSearching, status =>
        {
            statuses.Add(status.RowId);
            OtherConfiguration.Save();
        }, notLoadId);

        var count = Math.Max(1, (int)MathF.Floor(ImGui.GetColumnWidth() / (24 * _scale + ImGui.GetStyle().ItemSpacing.X)));
        var index = 0;

        if (IconSet.GetTexture(16220, out var text))
        {
            if (index++ % count != 0)
            {
                ImGui.SameLine();
            }
            if (NoPaddingNoColorImageButton(text.ImGuiHandle, new Vector2(24, 32) * _scale, name))
            {
                if (!ImGui.IsPopupOpen(popupId)) ImGui.OpenPopup(popupId);
            }
            ImguiTooltips.HoveredTooltip(LocalizationManager.RightLang.ConfigWindow_List_AddStatus);
        }

        foreach (var status in statuses.Select(a => Service.GetSheet<Status>().GetRow(a))
            .Where(a => a != null)
            .OrderBy(s => Math.Min(StringComparer.Distance(s.Name, _statusSearching)
            , StringComparer.Distance(s.RowId.ToString(), _statusSearching))))
        {
            void Delete() => removeId = status.RowId;

            var key = "Status" + status.RowId.ToString();

            Searchable.DrawHotKeysPopup(key, string.Empty, (LocalizationManager.RightLang.ConfigWindow_List_Remove, Delete, new string[] { "Delete" }));

            if (IconSet.GetTexture(status.Icon, out var texture, notLoadId))
            {
                if (index++ % count != 0)
                {
                    ImGui.SameLine();
                }
                NoPaddingNoColorImageButton(texture.ImGuiHandle, new Vector2(24, 32) * _scale, "Status" + status.RowId.ToString());

                Searchable.ExecuteHotKeysPopup(key, string.Empty, $"{status.Name} ({status.RowId})", false, 
                    (Delete, new Dalamud.Game.ClientState.Keys.VirtualKey[] { Dalamud.Game.ClientState.Keys.VirtualKey.DELETE }));
            }
        }

        if (removeId != 0)
        {
            statuses.Remove(removeId);
            OtherConfiguration.Save();
        }
    }

    internal static void StatusPopUp(string popupId, Status[] allStatus, ref string searching, Action<Status> clicked, uint notLoadId = 10100)
    {
        if (ImGui.BeginPopup(popupId))
        {
            ImGui.SetNextItemWidth(200 * _scale);
            ImGui.InputTextWithHint("##Searching the status", LocalizationManager.RightLang.ConfigWindow_List_StatusNameOrId, ref searching, 128);

            ImGui.Spacing();

            if (ImGui.BeginChild("Rotation Solver Add Status", new Vector2(-1, 400 * _scale)))
            {
                var count = Math.Max(1, (int)MathF.Floor(ImGui.GetWindowWidth() / (24 * _scale + ImGui.GetStyle().ItemSpacing.X)));
                var index = 0;

                var searchingKey = searching;
                foreach (var status in allStatus.OrderBy(s => Math.Min(StringComparer.Distance(s.Name, searchingKey)
                , StringComparer.Distance(s.RowId.ToString(), searchingKey))))
                {
                    if (IconSet.GetTexture(status.Icon, out var texture, notLoadId))
                    {
                        if (index++ % count != 0)
                        {
                            ImGui.SameLine();
                        }
                        if (NoPaddingNoColorImageButton(texture.ImGuiHandle, new Vector2(24, 32) * _scale, "Adding" + status.RowId.ToString()))
                        {
                            clicked?.Invoke(status);
                            ImGui.CloseCurrentPopup();
                        }
                        ImguiTooltips.HoveredTooltip($"{status.Name} ({status.RowId})");
                    }
                }
                ImGui.EndChild();
            }

            ImGui.EndPopup();
        }
    }

    private static readonly ISearchable[] _listSearchable = new ISearchable[]
    {
        new CheckBoxSearchPlugin(PluginConfigBool.RecordCastingArea),
    };
    private static void DrawListActions()
    {
        ImGui.SetNextItemWidth(ImGui.GetWindowWidth());
        ImGui.InputTextWithHint("##Searching the action", LocalizationManager.RightLang.ConfigWindow_List_ActionNameOrId, ref _actionSearching, 128);

        if (ImGui.BeginTable("Rotation Solver List Actions", 2, ImGuiTableFlags.BordersInner | ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingStretchSame))
        {
            ImGui.TableSetupScrollFreeze(0, 1);
            ImGui.TableNextRow(ImGuiTableRowFlags.Headers);

            ImGui.TableNextColumn();
            ImGui.TableHeader(LocalizationManager.RightLang.ConfigWindow_List_HostileCastingTank);

            ImGui.TableNextColumn();
            ImGui.TableHeader(LocalizationManager.RightLang.ConfigWindow_List_HostileCastingArea);

            ImGui.TableNextRow();

            ImGui.TableNextColumn();
            ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_List_HostileCastingTankDesc);
            DrawActionsList(nameof(OtherConfiguration.HostileCastingTank), OtherConfiguration.HostileCastingTank);

            ImGui.TableNextColumn();
            foreach (var searchable in _listSearchable)
            {
                searchable?.Draw(Job);
            }
            ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_List_HostileCastingAreaDesc);

            DrawActionsList(nameof(OtherConfiguration.HostileCastingArea), OtherConfiguration.HostileCastingArea);

            ImGui.EndTable();
        }
    }

    private static string _actionSearching = string.Empty;
    private static void DrawActionsList(string name, HashSet<uint> actions)
    {
        uint removeId = 0;

        var popupId = "Rotation Solver Action Popup" + name;

        if (ImGui.BeginPopup(popupId))
        {
            ImGui.SetNextItemWidth(200 * _scale);
            ImGui.InputTextWithHint("##Searching the action pop up", LocalizationManager.RightLang.ConfigWindow_List_ActionNameOrId, ref _actionSearching, 128);

            ImGui.Spacing();

            if (ImGui.BeginChild("Rotation Solver Add action", new Vector2(-1, 400 * _scale)))
            {
                foreach (var action in AllActions.OrderBy(s => Math.Min(StringComparer.Distance(s.Name, _actionSearching)
                , StringComparer.Distance(s.RowId.ToString(), _actionSearching))))
                {
                    var selected = ImGui.Selectable($"{action.Name} ({action.RowId})");
                    if (ImGui.IsItemHovered())
                    {
                        ImguiTooltips.ShowTooltip($"{action.Name} ({action.RowId})");
                        if(selected)
                        {
                            actions.Add(action.RowId);
                            OtherConfiguration.Save();
                            ImGui.CloseCurrentPopup();
                        }
                    }
                }
                ImGui.EndChild();
            }

            ImGui.EndPopup();
        }


        if (ImGui.Button(LocalizationManager.RightLang.ConfigWindow_List_AddAction + "##" + name))
        {
            if (!ImGui.IsPopupOpen(popupId)) ImGui.OpenPopup(popupId);
        }

        ImGui.Spacing();

        foreach (var action in actions.Select(a => Service.GetSheet<GAction>().GetRow(a))
            .Where(a => a != null)
            .OrderBy(s => Math.Min(StringComparer.Distance(s.Name, _actionSearching)
            , StringComparer.Distance(s.RowId.ToString(), _actionSearching))))
        {
            void Reset() => removeId = action.RowId;

            var key = "Action" + action.RowId.ToString();

            Searchable.DrawHotKeysPopup(key, string.Empty, (LocalizationManager.RightLang.ConfigWindow_List_Remove, Reset, new string[] { "Delete" }));

            ImGui.Selectable($"{action.Name} ({action.RowId})");

            Searchable.ExecuteHotKeysPopup(key, string.Empty, string.Empty, false, (Reset, new Dalamud.Game.ClientState.Keys.VirtualKey[] { Dalamud.Game.ClientState.Keys.VirtualKey.DELETE }));
        }

        if (removeId != 0)
        {
            actions.Remove(removeId);
            OtherConfiguration.Save();
        }
    }

    private static string _territorySearching = string.Empty;
    public static Vector3 HoveredPosition { get; private set; } = Vector3.Zero;
    private static void DrawListTerritories()
    {
        if (Svc.ClientState == null) return;

        var territoryId = Svc.ClientState.TerritoryType;

        ImGui.PushFont(ImGuiHelper.GetFont(21));
        ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudYellow);
        const int iconSize = 32;
        var territory = Service.GetSheet<TerritoryType>().GetRow(territoryId);
        if (territory == null) return;
        var contentFinder = territory?.ContentFinderCondition?.Value;
        var territoryName = territory?.PlaceName?.Value?.Name ?? string.Empty;
        if(contentFinder != null && !string.IsNullOrEmpty(contentFinder.Name))
        {
            territoryName += $" ({contentFinder.Name})";
        }
        var icon = territory?.ContentFinderCondition?.Value?.ContentType?.Value?.Icon ?? 23;
        if (icon == 0) icon = 23;
        var getIcon = IconSet.GetTexture(icon, out var texture);
        DrawItemMiddle(() =>
        {
            if (getIcon)
            {
                ImGui.Image(texture.ImGuiHandle, Vector2.One * 28 * _scale);
                ImGui.SameLine();
            }
            ImGui.Text(territoryName);
        }, ImGui.GetWindowWidth(), ImGui.CalcTextSize(territoryName).X + ImGui.GetStyle().ItemSpacing.X + iconSize);

        ImGui.PopStyleColor();
        ImGui.PopFont();

        if (ImGui.BeginTable("Rotation Solver List Territories", 2, ImGuiTableFlags.BordersInner | ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingStretchSame))
        {
            ImGui.TableSetupScrollFreeze(0, 1);
            ImGui.TableNextRow(ImGuiTableRowFlags.Headers);

            ImGui.TableNextColumn();
            ImGui.TableHeader(LocalizationManager.RightLang.ConfigWindow_List_NoHostile);

            ImGui.TableNextColumn();
            ImGui.TableHeader(LocalizationManager.RightLang.ConfigWindow_List_BeneficialPositions);

            ImGui.TableNextRow();

            ImGui.TableNextColumn();

            ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_List_NoHostileDesc);

            var width = ImGui.GetColumnWidth() - ImGuiEx.CalcIconSize(FontAwesomeIcon.Ban).X - ImGui.GetStyle().ItemSpacing.X - 10 * _scale;

            if(!OtherConfiguration.NoHostileNames.TryGetValue(territoryId, out var libs))
            {
                OtherConfiguration.NoHostileNames[territoryId] = libs = Array.Empty<string>();
            }

            //Add one.
            if (!libs.Any(string.IsNullOrEmpty))
            {
                OtherConfiguration.NoHostileNames[territoryId] = libs.Append(string.Empty).ToArray();
            }

            int removeIndex = -1;
            for (int i = 0; i < libs.Length; i++)
            {
                ImGui.SetNextItemWidth(width);
                if(ImGui.InputTextWithHint($"##Rotation Solver Territory Name {i}", LocalizationManager.RightLang.ConfigWindow_List_NoHostilesName, ref libs[i], 1024))
                {
                    OtherConfiguration.NoHostileNames[territoryId] = libs;
                    OtherConfiguration.SaveNoHostileNames();
                }
                ImGui.SameLine();

                if (ImGuiEx.IconButton(FontAwesomeIcon.Ban, $"##Rotation Solver Remove Territory Name {i}"))
                {
                    removeIndex = i;
                }
            }
            if (removeIndex > -1)
            {
                var list = libs.ToList();
                list.RemoveAt(removeIndex);
                OtherConfiguration.NoHostileNames[territoryId] = list.ToArray();
                OtherConfiguration.SaveNoHostileNames();
            }
            ImGui.TableNextColumn();

            if (!OtherConfiguration.BeneficialPositions.TryGetValue(territoryId, out var pts))
            {
                OtherConfiguration.BeneficialPositions[territoryId] = pts = Array.Empty<Vector3>();
            }

            if (ImGui.Button(LocalizationManager.RightLang.ConfigWindow_List_AddPosition) && Player.Available) unsafe
                {
                    var point = Player.Object.Position;
                    int* unknown = stackalloc int[] { 0x4000, 0, 0x4000, 0 };

                    RaycastHit hit = default;

                    OtherConfiguration.BeneficialPositions[territoryId]
                    = pts.Append(FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance()->BGCollisionModule
                        ->RaycastEx(&hit, point + Vector3.UnitY * 5, -Vector3.UnitY, 20, 1, unknown) ? hit.Point : point).ToArray();
                    OtherConfiguration.SaveBeneficialPositions();
                }

            HoveredPosition = Vector3.Zero;
            removeIndex = -1;
            for (int i = 0; i < pts.Length; i++)
            {
                void Reset() => removeIndex = i;

                var key = "Beneficial Positions" + i.ToString();

                Searchable.DrawHotKeysPopup(key, string.Empty, (LocalizationManager.RightLang.ConfigWindow_List_Remove, Reset, new string[] { "Delete" }));

                ImGui.Selectable(pts[i].ToString());

                if (ImGui.IsItemHovered())
                {
                    HoveredPosition = pts[i];
                }

                Searchable.ExecuteHotKeysPopup(key, string.Empty, string.Empty, false, (Reset, new Dalamud.Game.ClientState.Keys.VirtualKey[] { Dalamud.Game.ClientState.Keys.VirtualKey.DELETE }));
            }
            if (removeIndex > -1)
            {
                var list = pts.ToList();
                list.RemoveAt(removeIndex);
                OtherConfiguration.BeneficialPositions[territoryId] = list.ToArray();
                OtherConfiguration.SaveBeneficialPositions();
            }

            ImGui.EndTable();
        }
    }
    #endregion

    #region Debug

    private static void DrawDebug()
    {
        foreach (var searchable in _debugSearchable)
        {
            searchable?.Draw(Job);
        }

        if (!Player.Available || !Service.Config.GetValue(PluginConfigBool.InDebug)) return;

        var str = SocialUpdater.EncryptString(Player.Object);
        ImGui.SetNextItemWidth(ImGui.CalcTextSize(str).X + 10);
        ImGui.InputText("That is your HASH", ref str, 100);

        _debugHeader?.Draw();
    }
    private static readonly ISearchable[] _debugSearchable = new ISearchable[]
    {
        new CheckBoxSearchPlugin(PluginConfigBool.InDebug),
    };

    private static readonly CollapsingHeaderGroup _debugHeader = new(new()
    {
        {() => RotationUpdater.RightNowRotation != null ? "Rotation" : string.Empty, DrawDebugRotationStatus},
        {() =>"Status", DrawStatus },
        {() =>"Party", DrawParty },
        {() =>"Target Data", DrawTargetData },
        {() =>"Next Action", DrawNextAction },
        {() =>"Last Action", DrawLastAction },
        {() =>"Icon", DrawIcon },
        {() =>"Effect",  () =>
            {
                ImGui.Text(Watcher.ShowStrSelf);
                ImGui.Separator();
                ImGui.Text(Watcher.ShowStrEnemy);
            } },
        });

    private static void DrawDebugRotationStatus()
    {
        RotationUpdater.RightNowRotation?.DisplayStatus();
    }

    private static unsafe void DrawStatus()
    {
        if ((IntPtr)FateManager.Instance() != IntPtr.Zero)
        {
            ImGui.Text("Fate: " + DataCenter.FateId.ToString());
        }
        ImGui.Text("Moving: " + DataCenter.IsMoving.ToString());
        ImGui.Text("Stop Moving: " + DataCenter.StopMovingRaw.ToString());

        ImGui.Text("TerritoryType: " + DataCenter.TerritoryContentType.ToString());
        ImGui.Text("DPSTaken: " + DataCenter.DPSTaken.ToString());
        ImGui.Text("TimeToNext: " + DataCenter.NextAbilityToNextGCD.ToString());
        ImGui.Text("WeaponElapsed: " + DataCenter.WeaponElapsed.ToString());
        ImGui.Text("AnimationLock: " + DataCenter.ActionRemain.ToString());

        ImGui.Text("Have pet: " + DataCenter.HasPet.ToString());
        ImGui.Text("Hostile Near Count: " + DataCenter.NumberOfHostilesInRange.ToString());
        ImGui.Text("Hostile Near Count Max Range: " + DataCenter.NumberOfHostilesInMaxRange.ToString());
        ImGui.Text("Have Companion: " + DataCenter.HasCompanion.ToString());
        ImGui.Text("Ping: " + DataCenter.Ping.ToString());
        ImGui.Text("MP: " + DataCenter.CurrentMp.ToString());
        ImGui.Text("Count Down: " + Service.CountDownTime.ToString());
        ImGui.Text("Fetch Time: " + DataCenter.FetchTime.ToString());

        foreach (var status in Player.Object.StatusList)
        {
            var source = status.SourceId == Player.Object.ObjectId ? "You" : Svc.Objects.SearchById(status.SourceId) == null ? "None" : "Others";
            ImGui.Text($"{status.GameData.Name}: {status.StatusId} From: {source}");
        }
    }
    private static unsafe void DrawParty()
    {
        ImGui.Text("Party Burst Ratio: " + DataCenter.RatioOfMembersIn2minsBurst.ToString());
        ImGui.Text("Party: " + DataCenter.PartyMembers.Count().ToString());
        ImGui.Text("CanHealSingleAbility: " + DataCenter.CanHealSingleAbility.ToString());
        ImGui.Text("CanHealSingleSpell: " + DataCenter.CanHealSingleSpell.ToString());
        ImGui.Text("CanHealAreaAbility: " + DataCenter.CanHealAreaAbility.ToString());
        ImGui.Text("CanHealAreaSpell: " + DataCenter.CanHealAreaSpell.ToString());
        ImGui.Text("CanHealAreaSpell: " + DataCenter.CanHealAreaSpell.ToString());
        ImGui.Text("PartyMembersAverHP: " + DataCenter.PartyMembersAverHP.ToString());
    }

    private static unsafe void DrawTargetData()
    {
        if (Svc.Targets.Target != null)
        {
            ImGui.Text("Height: " + Svc.Targets.Target.Struct()->Height.ToString());
            ImGui.Text("Kind: " + Svc.Targets.Target.GetObjectKind().ToString());
            ImGui.Text("SubKind: " + Svc.Targets.Target.GetBattleNPCSubKind().ToString());
            var owner = Svc.Objects.SearchById(Svc.Targets.Target.OwnerId);
            if (owner != null)
            {
                ImGui.Text("Owner: " + owner.Name.ToString());
            }
        }
        if (Svc.Targets.Target is BattleChara b)
        {
            ImGui.Text("HP: " + b.CurrentHp + " / " + b.MaxHp);
            ImGui.Text("Is Boss: " + b.IsBoss().ToString());
            ImGui.Text("Has Positional: " + b.HasPositional().ToString());
            ImGui.Text("Is Dying: " + b.IsDying().ToString());
            ImGui.Text("EventType: " + b.GetEventType().ToString());
            ImGui.Text("NamePlate: " + b.GetNamePlateIcon().ToString());
            ImGui.Text("StatusFlags: " + b.StatusFlags.ToString());
            ImGui.Text("InView: " + Svc.GameGui.WorldToScreen(b.Position, out _).ToString());
            ImGui.Text("Name Id: " + b.NameId.ToString());
            ImGui.Text("Data Id: " + b.DataId.ToString());
            ImGui.Text("Targetable: " + b.Struct()->Character.GameObject.TargetableStatus.ToString());

            var npc = b.GetObjectNPC();
            if (npc != null)
            {
                ImGui.Text("Unknown12: " + npc.Unknown12.ToString());

                //ImGui.Text("Unknown15: " + npc.Unknown15.ToString());
                //ImGui.Text("Unknown18: " + npc.Unknown18.ToString());
                //ImGui.Text("Unknown19: " + npc.Unknown19.ToString());
                //ImGui.Text("Unknown20: " + npc.Unknown20.ToString());
                //ImGui.Text("Unknown21: " + npc.Unknown21.ToString());
            }

            foreach (var status in b.StatusList)
            {
                var source = status.SourceId == Player.Object.ObjectId ? "You" : Svc.Objects.SearchById(status.SourceId) == null ? "None" : "Others";
                ImGui.Text($"{status.GameData.Name}: {status.StatusId} From: {source}");
            }
        }

        ImGui.Text("All: " + DataCenter.AllTargets.Count().ToString());
        ImGui.Text("Hostile: " + DataCenter.HostileTargets.Count().ToString());
        foreach (var item in DataCenter.HostileTargets)
        {
            ImGui.Text(item.Name.ToString());
        }
    }

    private static void DrawNextAction()
    {
        ImGui.Text(RotationUpdater.RightNowRotation.RotationName);
        ImGui.Text(DataCenter.SpecialType.ToString());

        ImGui.Text("Ability Remain: " + DataCenter.AbilityRemain.ToString());
        ImGui.Text("Action Remain: " + DataCenter.ActionRemain.ToString());
        ImGui.Text("Weapon Remain: " + DataCenter.WeaponRemain.ToString());
        ImGui.Text("Time: " + (DataCenter.CombatTimeRaw + DataCenter.WeaponRemain).ToString());
    }

    private static void DrawLastAction()
    {
        DrawAction(DataCenter.LastAction, nameof(DataCenter.LastAction));
        DrawAction(DataCenter.LastAbility, nameof(DataCenter.LastAbility));
        DrawAction(DataCenter.LastGCD, nameof(DataCenter.LastGCD));
        DrawAction(DataCenter.LastComboAction, nameof(DataCenter.LastComboAction));
    }

    private static unsafe void DrawIcon()
    {
        //if(ImGui.BeginTable("BLUAction", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingFixedFit))
        //{
        //    foreach (var item in Svc.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>())
        //    {
        //        if (item == null) continue;
        //        if (item.ClassJob?.Row != 36) continue;
        //        //if (item.RowId <= 20000) continue;

        //        ImGui.TableNextRow();
        //        ImGui.TableNextColumn();

        //        ImGui.Text($"{item.Name}");

        //        ImGui.TableNextColumn();

        //        try
        //        {
        //            var tex = IconSet.GetTexture(item.Icon);
        //            if (tex != null)
        //            {
        //                ImGui.Image(tex.ImGuiHandle, Vector2.One * 32);
        //            }
        //        }
        //        catch
        //        {

        //        }
        //        ImGui.TableNextColumn();

        //        try
        //        {
        //            ImGui.Text($"{(Aspect)item.Aspect}");
        //            ImGui.SameLine();
        //            var desc = item.AttackType?.Value?.Name?.ToString();
        //            if (!string.IsNullOrEmpty(desc)) ImGui.Text(desc);
        //        }
        //        catch
        //        {

        //        }
        //        ImGui.TableNextColumn();

        //        //ImGui.TableNextColumn();

        //        //try
        //        //{
        //        //    var desc = Svc.Data.GetExcelSheet<ActionTransient>()?.GetRow(item.RowId)?.Description?.ToString();
        //        //    //ImGui.Text((!string.IsNullOrEmpty(desc)).ToString());
        //        //    //if (!string.IsNullOrEmpty(desc)) ImGui.Text(desc);
        //        //}
        //        //catch
        //        //{

        //        //}
        //    }
        //    ImGui.EndTable();
        //}

    }

    private static void DrawAction(ActionID id, string type)
    {
        ImGui.Text($"{type}: {id}");
    }
    #endregion

    #region Image
    internal unsafe static bool SilenceImageButton(IntPtr handle, Vector2 size, bool selected, string id = "")
    {
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, ImGui.ColorConvertFloat4ToU32(*ImGui.GetStyleColorVec4(ImGuiCol.HeaderActive)));
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, ImGui.ColorConvertFloat4ToU32(*ImGui.GetStyleColorVec4(ImGuiCol.HeaderHovered)));
        ImGui.PushStyleColor(ImGuiCol.Button, selected ? ImGui.ColorConvertFloat4ToU32(*ImGui.GetStyleColorVec4(ImGuiCol.Header)) : 0);

        var result = NoPaddingImageButton(handle, size, id);
        ImGui.PopStyleColor(3);

        return result;
    }

    internal unsafe static bool NoPaddingNoColorImageButton(IntPtr handle, Vector2 size, string id = "")
    {
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, 0);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, 0);
        ImGui.PushStyleColor(ImGuiCol.Button, 0);
        var result = NoPaddingImageButton(handle, size, id);
        ImGui.PopStyleColor(3);

        return result;
    }

    private static bool NoPaddingImageButton(IntPtr handle, Vector2 size, string id = "")
    {
        var padding = ImGui.GetStyle().FramePadding;
        ImGui.GetStyle().FramePadding = Vector2.Zero;

        ImGui.PushID(id);
        var result = ImGui.ImageButton(handle, size);
        ImGui.PopID();
        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        }

        ImGui.GetStyle().FramePadding = padding;
        return result;
    }

    private static bool TextureButton(TextureWrap texture, float wholeWidth, float maxWidth, string id = "")
    {
        if (texture == null) return false;

        var size = new Vector2(texture.Width, texture.Height) * MathF.Min(1, MathF.Min(maxWidth, wholeWidth) / texture.Width);

        var result = false;
        DrawItemMiddle(() =>
        {
            result = NoPaddingNoColorImageButton(texture.ImGuiHandle, size, id);
        }, wholeWidth, size.X);
        return result;
    }

    internal static void DrawActionOverlay(Vector2 cursor, float width, float percent)
    {
        var pixPerUnit = width / 82;

        if (percent < 0)
        {
            if (IconSet.GetTexture("ui/uld/icona_frame_hr1.tex", out var cover))
            {
                ImGui.SetCursorPos(cursor - new Vector2(pixPerUnit * 3, pixPerUnit * 4));

                var step = new Vector2(88f / cover.Width, 96f / cover.Height);
                var start = new Vector2((96f * 0 + 4f) / cover.Width, (96f * 2) / cover.Height);

                //Out Size is 88, 96
                //Inner Size is 82, 82
                ImGui.Image(cover.ImGuiHandle, new Vector2(pixPerUnit * 88, pixPerUnit * 96),
                    start, start + step);
            }
        }
        else if (percent < 1)
        {
            if (IconSet.GetTexture("ui/uld/icona_recast_hr1.tex", out var cover))
            {
                ImGui.SetCursorPos(cursor - new Vector2(pixPerUnit * 3, pixPerUnit * 0));

                var P = (int)(percent * 81);


                var step = new Vector2(88f / cover.Width, 96f / cover.Height);
                var start = new Vector2(P % 9 * step.X, P / 9 * step.Y);

                //Out Size is 88, 96
                //Inner Size is 82, 82
                ImGui.Image(cover.ImGuiHandle, new Vector2(pixPerUnit * 88, pixPerUnit * 96),
                    start, start + step);
            }
        }
        else
        {
            if (IconSet.GetTexture("ui/uld/icona_frame_hr1.tex", out var cover))
            {

                ImGui.SetCursorPos(cursor - new Vector2(pixPerUnit * 3, pixPerUnit * 4));

                //Out Size is 88, 96
                //Inner Size is 82, 82
                ImGui.Image(cover.ImGuiHandle, new Vector2(pixPerUnit * 88, pixPerUnit * 96),
                    new Vector2(4f / cover.Width, 0f / cover.Height),
                    new Vector2(92f / cover.Width, 96f / cover.Height));
            }
        }

        if (percent > 1)
        {
            if (IconSet.GetTexture("ui/uld/icona_recast2_hr1.tex", out var cover))
            {
                ImGui.SetCursorPos(cursor - new Vector2(pixPerUnit * 3, pixPerUnit * 0));

                var P = (int)(percent % 1 * 81);

                var step = new Vector2(88f / cover.Width, 96f / cover.Height);
                var start = new Vector2((P % 9 + 9) * step.X, P / 9 * step.Y);

                //Out Size is 88, 96
                //Inner Size is 82, 82
                ImGui.Image(cover.ImGuiHandle, new Vector2(pixPerUnit * 88, pixPerUnit * 96),
                    start, start + step);
            }
        }
    }
    #endregion

    #region Child
    private static bool BeginChild(string str_id)
    {
        if (IsFailed()) return false;
        return ImGui.BeginChild(str_id);
    }

    private static bool BeginChild(string str_id, Vector2 size)
    {
        if (IsFailed()) return false;
        return ImGui.BeginChild(str_id, size);
    }

    private static bool BeginChild(string str_id, Vector2 size, bool border, ImGuiWindowFlags flags)
    {
        if (IsFailed()) return false;
        return ImGui.BeginChild(str_id, size, border, flags);
    }

    private static bool IsFailed()
    {
        var style = ImGui.GetStyle();
        var min = style.WindowPadding.X + style.WindowBorderSize;
        var columnWidth = ImGui.GetColumnWidth();
        var windowSize = ImGui.GetWindowSize();
        var cursor = ImGui.GetCursorPos();

        return columnWidth > 0 && columnWidth <= min
            || windowSize.Y - cursor.Y <= min
            || windowSize.X - cursor.X <= min;
    }
    #endregion
}