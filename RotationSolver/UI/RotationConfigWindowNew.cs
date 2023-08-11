using Dalamud.Interface.Colors;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using ECommons.ImGuiMethods;
using ImGuiScene;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.ActionSequencer;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using RotationSolver.TextureItems;
using RotationSolver.Updaters;
using System.Diagnostics;
using GAction = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.UI;

public partial class RotationConfigWindowNew : Window
{
    private static float _scale => ImGuiHelpers.GlobalScale;
    private static Job Job => RotationUpdater.Job;

    private RotationConfigWindowTab _activeTab;

    private const float MIN_COLUMN_WIDTH = 24;
    private const float JOB_ICON_WIDTH = 50;

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

    public override void OnClose()
    {
        Service.Config.Save();
        base.OnClose();
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

            var iconSize = Math.Max(_scale * MIN_COLUMN_WIDTH, Math.Min(wholeWidth, _scale * JOB_ICON_WIDTH)) * 0.6f;

            if (wholeWidth > JOB_ICON_WIDTH * _scale)
            {
                ImGui.SetNextItemWidth(wholeWidth);
                SearchingBox();
            }
            else
            {
                if(IconSet.GetTexture(46, out var icon))
                {
                    DrawItemMiddle(() =>
                    {
                        if (ImGui.BeginPopup("Searching Popup"))
                        {
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
                        if (NoPaddingNoColorImageButton(icon.ImGuiHandle, Vector2.One * iconSize))
                        {
                            _activeTab = item;
                        }
                        DrawActionOverlay(cursor, iconSize, _activeTab == item ? 1 : 0);
                    }, Math.Max(_scale * MIN_COLUMN_WIDTH, wholeWidth), iconSize);
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

        int realFrame = 180;
        if (Service.ConfigNew.GetValue(Basic.Configuration.PluginConfigBool.DrawIconAnimation))
        {
            var frame = Environment.TickCount / 100; //10fps
            realFrame = frame % 60 * 3; // convert to 30 fps.
            if (realFrame == 0) realFrame = 180;
        }

        if (IconSet.GetTexture($"https://raw.githubusercontent.com/ArchiDog1998/RotationSolver/main/Images/Logos/{realFrame: D4}.png", out var logo) 
            || IconSet.GetTexture(0, out logo))
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

            var horizonalWholeWidth = Math.Max(comboSize, gameVersionSize) + iconSize + ImGui.GetStyle().ItemSpacing.X;

            if(horizonalWholeWidth > wholeWidth)
            {
                DrawItemMiddle(() =>
                {
                    DrawRotationIcon(rotation, iconSize);
                }, wholeWidth, iconSize);

                if(_scale * JOB_ICON_WIDTH < wholeWidth)
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
                    Service.Config.RotationChoices[rotation.ClassJob.RowId] = r.GetType().FullName;
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
        var margin = 8 * _scale;
        ImGui.SetCursorPos(ImGui.GetCursorPos() + Vector2.One * margin);
        if (ImGui.BeginChild("Rotation Solver Body", Vector2.One * -margin))
        {
            //Search box
            if (_searchResults != null && _searchResults.Any())
            {
                foreach (var searchable in _searchResults)
                {
                    searchable?.Draw(Job);
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

                    case RotationConfigWindowTab.IDs:
                        DrawIDs();
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

        ImGui.NewLine();
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

        if (ImGui.BeginTable("Incompatible plugin", 4, ImGuiTableFlags.Borders
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

        if (IconSet.GetTexture("https://github-readme-stats.vercel.app/api/pin/?username=ArchiDog1998&repo=RotationSolver&theme=dark", out var icon) && TextureButton(icon, width, width))
        {
            Util.OpenLink("https://github.com/ArchiDog1998/RotationSolver");
        }

        if (IconSet.GetTexture("https://discordapp.com/api/guilds/1064448004498653245/embed.png?style=banner2", out icon) && TextureButton(icon, width, width))
        {
            Util.OpenLink("https://discord.gg/4fECHunam9");
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
            if (IconSet.GetTexture("https://storage.ko-fi.com/cdn/brandasset/kofi_button_red.png", out var icon) && TextureButton(icon, wholeWidth, 250 * _scale))
            {
                Util.OpenLink(info.DonateLink);
            }
        }

        _rotationHeader.Draw();
    }

    private static readonly CollapsingHeaderGroup _rotationHeader = new(new()
    {
        { () => LocalizationManager.RightLang.ConfigWindow_Rotation_Description, DrawRotationDescription},

        { () => LocalizationManager.RightLang.ConfigWindow_Rotation_Configuration, DrawRotationConfiguration},

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
            DrawLinkDescription(link.LinkDescription, wholeWidth);
        }
    }

    internal static void DrawLinkDescription(LinkDescription link, float wholeWidth)
    {
        var hasTexture = IconSet.GetTexture(link.Path, out var texture);

        if (hasTexture && TextureButton(texture, wholeWidth, wholeWidth))
        {
            Util.OpenLink(link.Path);
        }

        ImGui.TextWrapped(link.Description);

        if (!hasTexture && !string.IsNullOrEmpty(link.Path))
        {
            if (ImGuiEx.IconButton(FontAwesomeIcon.Question, link.Description))
            {
                Util.OpenLink(link.Path);
            }
        }
    }

    private static void DrawRotationConfiguration()
    {
        var rotation = RotationUpdater.RightNowRotation;
        if (rotation == null) return;

        rotation.Configs.Draw(Player.Available
                && rotation.Jobs.Contains((Job)Player.Object.ClassJob.Id));
    }

    private static void DrawRotationInformation()
    {
        var rotation = RotationUpdater.RightNowRotation;
        if (rotation == null) return;
        var wholeWidth = ImGui.GetWindowWidth();

        var link = rotation.GetType().GetCustomAttribute<SourceCodeAttribute>();

        if (link != null)
        {
            if (IconSet.GetTexture("https://github.githubassets.com/images/modules/logos_page/GitHub-Logo.png", out var icon) && TextureButton(icon, wholeWidth, 200 * _scale))
            {
                Util.OpenLink(link.Url);
            }
        }

        var assembly = rotation.GetType().Assembly;
        var info = assembly.GetInfo();

        if (info != null )
        {
            ImGui.Text("Assembly Name: ");
            ImGui.SameLine();
            if (ImGui.Button(info.Name))
            {
                Process.Start("explorer.exe", "/select, \"" + info.FilePath + "\"");
            }
            
            var version = assembly.GetName().Version;
            if (version != null)
            {
                ImGui.Text("Assembly Version: " + version.ToString());
            }

            ImGui.Text("Assembly Author: " + info.Author);
        }
    }
    #endregion

    #region Actions
    private static void DrawActions()
    {
        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_Actions_Description);

        if (ImGui.BeginTable("Rotation Solver Actions", 2, ImGuiTableFlags.Resizable))
        {
            ImGui.TableSetupColumn("Action Column", ImGuiTableColumnFlags.WidthFixed, ImGui.GetWindowWidth() / 2);
            ImGui.TableNextColumn();

            if (_actionsList != null && ImGui.BeginChild("Rotation Solver Action List"))
            {
                _actionsList.ClearCollapsingHeader();

                if (RotationUpdater.RightNowRotation != null)
                {
                    var size = 30 * _scale;
                    var count = (int)MathF.Floor(ImGui.GetWindowWidth() / (size + ImGui.GetStyle().ItemSpacing.X));
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

                                var cursor = ImGui.GetCursorPos();
                                ImGui.BeginGroup();
                                if (NoPaddingNoColorImageButton(icon.ImGuiHandle, Vector2.One * size, item.Name))
                                {
                                    _activeAction = item;
                                }
                                ImguiTooltips.HoveredTooltip(item.Name);
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
                            }
                        });
                    }
                }

                _actionsList.Draw();
                ImGui.EndChild();
            }

            ImGui.TableNextColumn();

            if (_sequencerList != null && _activeAction != null && ImGui.BeginChild("Rotation Solver Sequencer List"))
            {
                var enable = _activeAction.IsEnabled;
                if (ImGui.Checkbox($"{_activeAction.Name}##{_activeAction.Name} Enabled", ref enable))
                {
                    _activeAction.IsEnabled = enable;
                }

                ImGui.SameLine();
                OtherCommandType.ToggleActions.DisplayCommandHelp(_activeAction.ToString());

                enable = _activeAction.IsInCooldown;
                if (ImGui.Checkbox($"{LocalizationManager.RightLang.ConfigWindow_Actions_ShowOnCDWindow}##{_activeAction.Name}InCooldown", ref enable))
                {
                    _activeAction.IsInCooldown = enable;
                }

                ImGui.SameLine();
                OtherCommandType.DoActions.DisplayCommandHelp($"{_activeAction}-{5}",
                    type => string.Format(LocalizationManager.RightLang.ConfigWindow_Actions_InsertCommand, _activeAction, 5), false);

                _sequencerList.Draw();
                ImGui.EndChild();
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

            if (!set.Conditions.TryGetValue(_activeAction.ID, out var conditionSet))
            {
                conditionSet = set.Conditions[_activeAction.ID] = new ConditionSet();
            }
            conditionSet?.Draw(rotation);
        } },

        { () => LocalizationManager.RightLang.ConfigWindow_Actions_DisabledConditionSet, () =>
        {
            ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_Actions_DisabledConditionSet_Description);

            var rotation = RotationUpdater.RightNowRotation;
            var set = ActionSequencerUpdater.RightSet;
            if (set == null || _activeAction == null || rotation == null) return;

            if (!set.DiabledConditions.TryGetValue(_activeAction.ID, out var disableConditionSet))
            {
                disableConditionSet = set.DiabledConditions[_activeAction.ID] = new ConditionSet();
            }
            disableConditionSet?.Draw(rotation);
        } },
    })
    {
        HeaderSize = 18,
    };
    #endregion

    private static readonly CollapsingHeaderGroup _rotationsHeader = new(new()
    {

    });
    private static void DrawRotations()
    {
        ImGui.Text("Rotations");
        _rotationsHeader?.Draw();
    }

    #region Ids
    private static uint _territoryId = 0;
    private static TerritoryTypeTexture[] _allTerritories = null;
    internal static TerritoryTypeTexture[] AllTerritories
    {
        get
        {
            _allTerritories ??= Service.GetSheet<TerritoryType>()
                    .Where(t => t != null
                        && t.ContentFinderCondition?.Value?.ContentType?.Value?.RowId != 0)
                    .OrderBy(t => t.ContentFinderCondition?.Value?.ContentType?.Value?.RowId)
                    .Select(t => new TerritoryTypeTexture(t))
                    .ToArray();
            return _allTerritories;
        }
    }

    private static StatusTexture[] _allDispelStatus = null;
    internal static StatusTexture[] AllDispelStatus
    {
        get
        {
            _allDispelStatus ??= Service.GetSheet<Status>()
                    .Where(s => s.CanDispel)
                    .Select(s => new StatusTexture(s))
                    .ToArray();
            return _allDispelStatus;
        }
    }

    private static StatusTexture[] _allInvStatus = null;
    internal static StatusTexture[] AllInvStatus
    {
        get
        {
            _allInvStatus ??= Service.GetSheet<Status>()
                    .Where(s => !s.CanDispel && !s.LockMovement && !s.IsGaze && !s.IsFcBuff && s.HitEffect.Row == 16 && s.ClassJobCategory.Row == 1 && s.StatusCategory == 1
                        && !string.IsNullOrEmpty(s.Name.ToString()) && s.Icon != 0)
                    .Select(s => new StatusTexture(s))
                    .ToArray();
            return _allInvStatus;
        }
    }

    private static ActionTexture[] _allActions = null;
    internal static ActionTexture[] AllActions
    {
        get
        {
            _allActions ??= Service.GetSheet<GAction>()
                    .Where(a => !string.IsNullOrEmpty(a.Name) && !a.IsPvP && !a.IsPlayerAction
                    && a.ClassJob.Value == null && a.Cast100ms > 0)
                    .Select(a => new ActionTexture(a))
                    .ToArray();
            return _allActions;
        }
    }
    private static readonly CollapsingHeaderGroup _idsHeader = new(new()
    {

    });
    private static void DrawIDs()
    {
        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_List_Description); 
        _idsHeader?.Draw();
    }
    #endregion

    private static readonly CollapsingHeaderGroup _debugHeader = new(new()
    {

    });
    private static void DrawDebug()
    {
        ImGui.Text("Debug");
        _debugHeader?.Draw();
    }

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
}