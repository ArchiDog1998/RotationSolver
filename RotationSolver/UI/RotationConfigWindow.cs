using Dalamud.Interface.Colors;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.ImGuiMethods;
using ExCSS;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Configuration;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using RotationSolver.UI.SearchableConfigs;
using RotationSolver.UI.SearchableSettings;
using RotationSolver.Updaters;
using System.Diagnostics;
using GAction = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.UI;

public partial class RotationConfigWindow : Window
{
    private static float Scale => ImGuiHelpers.GlobalScale;
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

    public override void OnClose()
    {
        Service.Config.Save();
        ActionSequencerUpdater.SaveFiles();
        base.OnClose();
    }

    public override void Draw()
    {
        using var style = ImRaii.PushStyle(ImGuiStyleVar.SelectableTextAlign, new Vector2(0.5f, 0.5f));
        try
        {
            var leftTop = ImGui.GetWindowPos() + ImGui.GetCursorPos();
            var rightDown = leftTop + ImGui.GetWindowSize();
            var screenSize = ImGuiHelpers.MainViewport.Size;
            if ((leftTop.X <= 0 || leftTop.Y <= 0 || rightDown.X >= screenSize.X || rightDown.Y >= screenSize.Y)
                && !ImGui.GetIO().ConfigFlags.HasFlag(ImGuiConfigFlags.ViewportsEnable))
            {
                var str = string.Empty;
                for (int i = 0; i < 150; i++)
                {
                    str += "Move away! Don't crash! ";
                }

                using var font = ImRaii.PushFont(ImGuiHelper.GetFont(24));
                using var color = ImRaii.PushColor(ImGuiCol.Text, ImGui.ColorConvertFloat4ToU32(ImGuiColors.DalamudYellow));
                ImGui.TextWrapped(str);
            }
            else
            {
                using var table = ImRaii.Table("Rotation Config Table", 2, ImGuiTableFlags.Resizable);
                if (table)
                {
                    ImGui.TableSetupColumn("Rotation Config Side Bar", ImGuiTableColumnFlags.WidthFixed, 100 * Scale);
                    ImGui.TableNextColumn();

                    try
                    {
                        DrawSideBar();
                    }
                    catch (Exception ex)
                    {
                        Svc.Log.Warning(ex, "Something wrong with sideBar");
                    }

                    ImGui.TableNextColumn();

                    try
                    {
                        DrawBody();
                    }
                    catch (Exception ex)
                    {
                        Svc.Log.Warning(ex, "Something wrong with body");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Svc.Log.Warning(ex, "Something wrong with config window.");
        }
    }

    private void DrawConditionSet()
    {
        var set = DataCenter.RightSet;

        const string popUpId = "Right Set Popup";
        if (ImGui.Selectable(set.Name, false, ImGuiSelectableFlags.None, new Vector2(0, 20)))
        {
            ImGui.OpenPopup(popUpId);
        }
        ImguiTooltips.HoveredTooltip(LocalizationManager.RightLang.ConfigWindow_ConditionSetDesc);

        using var popup = ImRaii.Popup(popUpId);
        if (popup.Success)
        {
            var combos = DataCenter.ConditionSets;
            for (int i = 0; i < combos.Length; i++)
            {
                void DeleteFile()
                {
                    ActionSequencerUpdater.Delete(combos[i].Name);
                }

                if (combos[i].Name == set.Name)
                {
                    ImGuiHelper.SetNextWidthWithName(set.Name);
                    ImGui.InputText("##MajorConditionSet", ref set.Name, 100);
                }
                else
                {
                    var key = "Condition Set At " + i.ToString();
                    ImGuiHelper.DrawHotKeysPopup(key, string.Empty, (LocalizationManager.RightLang.ConfigWindow_List_Remove, DeleteFile, new string[] { "Delete" }));


                    if (ImGui.Selectable(combos[i].Name))
                    {
                        Service.Config.SetValue(PluginConfigInt.ActionSequencerIndex, i);
                    }

                    ImGuiHelper.ExecuteHotKeysPopup(key, string.Empty, string.Empty, false,
        (DeleteFile, new Dalamud.Game.ClientState.Keys.VirtualKey[] { Dalamud.Game.ClientState.Keys.VirtualKey.DELETE }));
                }
            }

            ImGui.PushFont(UiBuilder.IconFont);

            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
            if (ImGui.Selectable(FontAwesomeIcon.Plus.ToIconString()))
            {
                ActionSequencerUpdater.AddNew();
            }
            ImGui.PopStyleColor();

            if (ImGui.Selectable(FontAwesomeIcon.FileDownload.ToIconString()))
            {
                ActionSequencerUpdater.LoadFiles();
            }

            ImGui.PopFont();
            ImguiTooltips.HoveredTooltip(LocalizationManager.RightLang.ActionSequencer_Load);
        }
    }

    private void DrawSideBar()
    {
        using var child = ImRaii.Child("Rotation Solver Side bar", -Vector2.One, false, ImGuiWindowFlags.NoScrollbar);
        if (child)
        {
            var wholeWidth = ImGui.GetWindowSize().X;

            DrawHeader(wholeWidth);

            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();

            var iconSize = Math.Max(Scale * MIN_COLUMN_WIDTH, Math.Min(wholeWidth, Scale * JOB_ICON_WIDTH)) * 0.6f;

            if (wholeWidth > JOB_ICON_WIDTH * Scale)
            {
                DrawConditionSet();

                ImGui.Separator();
                ImGui.Spacing();

                ImGui.SetNextItemWidth(wholeWidth);
                SearchingBox();

                ImGui.Spacing();
            }
            else
            {
                if (IconSet.GetTexture(46, out var icon))
                {
                    ImGuiHelper.DrawItemMiddle(() =>
                    {
                        using var popup = ImRaii.Popup("Searching Popup");
                        if (popup)
                        {
                            ImGui.SetNextItemWidth(200 * Scale);
                            SearchingBox();
                            if (ImGui.IsKeyDown(ImGuiKey.Enter))
                            {
                                ImGui.CloseCurrentPopup();
                            }
                        }

                        var cursor = ImGui.GetCursorPos();
                        if (ImGuiHelper.NoPaddingNoColorImageButton(icon.ImGuiHandle, Vector2.One * iconSize))
                        {
                            ImGui.OpenPopup("Searching Popup");
                        }
                        ImGuiHelper.DrawActionOverlay(cursor, iconSize, -1);
                        ImguiTooltips.HoveredTooltip("Search");

                    }, Math.Max(Scale * MIN_COLUMN_WIDTH, wholeWidth), iconSize);
                }
            }

            foreach (var item in Enum.GetValues<RotationConfigWindowTab>())
            {
                if (item.GetAttribute<TabSkipAttribute>() != null) continue;

                if (IconSet.GetTexture(item.GetAttribute<TabIconAttribute>()?.Icon ?? 0, out var icon) && wholeWidth <= JOB_ICON_WIDTH * Scale)
                {
                    ImGuiHelper.DrawItemMiddle(() =>
                    {
                        var cursor = ImGui.GetCursorPos();
                        if (ImGuiHelper.NoPaddingNoColorImageButton(icon.ImGuiHandle, Vector2.One * iconSize, item.ToString()))
                        {
                            _activeTab = item;
                            _searchResults = Array.Empty<ISearchable>();
                        }
                        ImGuiHelper.DrawActionOverlay(cursor, iconSize, _activeTab == item ? 1 : 0);
                    }, Math.Max(Scale * MIN_COLUMN_WIDTH, wholeWidth), iconSize);

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
                        _searchResults = Array.Empty<ISearchable>();
                    }
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                        var desc = item.ToDescription();
                        if (!string.IsNullOrEmpty(desc)) ImguiTooltips.ShowTooltip(desc);
                    }
                }
            }

            if (wholeWidth <= 60 * Scale
                ? IconSet.GetTexture("https://storage.ko-fi.com/cdn/brandasset/kofi_s_logo_nolabel.png", out var texture)
                : IconSet.GetTexture("https://storage.ko-fi.com/cdn/brandasset/kofi_bg_tag_dark.png", out texture))
            {
                var width = Math.Min(150 * Scale, Math.Max(Scale * MIN_COLUMN_WIDTH, Math.Min(wholeWidth, texture.Width)));
                var size = new Vector2(width, width * texture.Height / texture.Width);
                size *= MathF.Max(Scale * MIN_COLUMN_WIDTH / size.Y, 1);
                var result = false;
                ImGuiHelper.DrawItemMiddle(() =>
                {
                    ImGui.SetCursorPosY(ImGui.GetWindowSize().Y + ImGui.GetScrollY() - size.Y);
                    result = ImGuiHelper.NoPaddingNoColorImageButton(texture.ImGuiHandle, size, "Donate Plugin");
                }, wholeWidth, size.X);

                if (result)
                {
                    Util.OpenLink("https://ko-fi.com/B0B0IN5DX");
                }
            }
        }
    }

    private const int FRAME_COUNT = 180;
    private static readonly List<string> _loadingList = new(FRAME_COUNT);
    private static readonly Dictionary<string, IDalamudTextureWrap> _logosWrap = new(FRAME_COUNT + 1);
    private static bool GetLocalImage(string name, out IDalamudTextureWrap texture)
    {
        var dir = $"{Svc.PluginInterface.ConfigDirectory.FullName}\\Images";

        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        var file = dir + $"\\{name}.png";

        if (File.Exists(file))
        {
            if (!_logosWrap.ContainsKey(file))
            {
                _logosWrap[name] = Svc.PluginInterface.UiBuilder.LoadImage(file);
            }
        }
        else if (!_loadingList.Contains(name))
        {
            _loadingList.Add(name);

            Task.Run(async () =>
            {
                if (!File.Exists(file))
                {
                    var url = $"https://raw.githubusercontent.com/{Service.USERNAME}/{Service.REPO}/main/Images/{name}.png";

                    using var client = new HttpClient();
                    var stream = await client.GetStreamAsync(url);

                    using var fs = new FileStream(file, FileMode.CreateNew);

                    await stream.CopyToAsync(fs);
                }


                _loadingList.Remove(name);
            });
        }

        return _logosWrap.TryGetValue(name, out texture);
    }



    private void DrawHeader(float wholeWidth)
    {
        var size = MathF.Max(MathF.Min(wholeWidth, Scale * 128), Scale * MIN_COLUMN_WIDTH);

        if (IconSet.GetTexture((uint)0, out var overlay))
        {
            ImGuiHelper.DrawItemMiddle(() =>
            {
                var cursor = ImGui.GetCursorPos();
                if (ImGuiHelper.SilenceImageButton(overlay.ImGuiHandle, Vector2.One * size,
                    _activeTab == RotationConfigWindowTab.About, "About Icon"))
                {
                    _activeTab = RotationConfigWindowTab.About;
                    _searchResults = Array.Empty<ISearchable>();
                }
                ImguiTooltips.HoveredTooltip(LocalizationManager.RightLang.ConfigWindow_About_Punchline);

                var frame = Environment.TickCount / 34 % FRAME_COUNT;
                if (frame <= 0) frame += FRAME_COUNT;
                if (GetLocalImage(Service.Config.GetValue(PluginConfigBool.DrawIconAnimation)
                    ? frame.ToString("D4") : "Logo", out var logo))
                {
                    ImGui.SetCursorPos(cursor);
                    ImGui.Image(logo.ImGuiHandle, Vector2.One * size);
                }
            }, wholeWidth, size);

            ImGui.Spacing();
        }

        var rotation = DataCenter.RightNowRotation;
        if (rotation != null)
        {
            var rotations = RotationUpdater.CustomRotations.FirstOrDefault(i => i.ClassJobIds.Contains((Job)(Player.Object?.ClassJob.Id ?? 0)))?.Rotations ?? Array.Empty<ICustomRotation>();

            var iconSize = Math.Max(Scale * MIN_COLUMN_WIDTH, Math.Min(wholeWidth, Scale * JOB_ICON_WIDTH));
            var comboSize = ImGui.CalcTextSize(rotation.RotationName).X;

            const string slash = " - ";
            var gameVersionSize = ImGui.CalcTextSize(slash + rotation.GameVersion).X + ImGui.GetStyle().ItemSpacing.X;
            var gameVersion = LocalizationManager.RightLang.ConfigWindow_Helper_GameVersion + ": ";
            var drawCenter = ImGui.CalcTextSize(slash + gameVersion + rotation.GameVersion).X + iconSize + ImGui.GetStyle().ItemSpacing.X * 3 < wholeWidth;
            if (drawCenter) gameVersionSize += ImGui.CalcTextSize(gameVersion).X + ImGui.GetStyle().ItemSpacing.X;

            var horizonalWholeWidth = Math.Max(comboSize, gameVersionSize) + iconSize + ImGui.GetStyle().ItemSpacing.X;

            if (horizonalWholeWidth > wholeWidth)
            {
                ImGuiHelper.DrawItemMiddle(() =>
                {
                    DrawRotationIcon(rotation, iconSize);
                }, wholeWidth, iconSize);

                if (Scale * JOB_ICON_WIDTH < wholeWidth)
                {
                    DrawRotationCombo(comboSize, rotations, rotation, gameVersion);
                }
            }
            else
            {
                ImGuiHelper.DrawItemMiddle(() =>
                {
                    DrawRotationIcon(rotation, iconSize);

                    ImGui.SameLine();

                    using var group = ImRaii.Group();

                    DrawRotationCombo(comboSize, rotations, rotation, gameVersion);
                    ImGui.TextDisabled(slash);
                    ImGui.SameLine();

                    if (drawCenter)
                    {
                        ImGui.TextDisabled(gameVersion);
                        ImGui.SameLine();
                    }
                    ImGui.Text(rotation.GameVersion);

                }, wholeWidth, horizonalWholeWidth);
            }
        }
    }

    private void DrawRotationIcon(ICustomRotation rotation, float iconSize)
    {
        var cursor = ImGui.GetCursorPos();
        if (rotation.GetTexture(out var jobIcon) && ImGuiHelper.SilenceImageButton(jobIcon.ImGuiHandle,
            Vector2.One * iconSize, _activeTab == RotationConfigWindowTab.Rotation))
        {
            _activeTab = RotationConfigWindowTab.Rotation;
            _searchResults = Array.Empty<ISearchable>();
        }
        var desc = rotation.Name + $" ({rotation.RotationName})";
        var type = rotation.Type;
        if (type.HasFlag(CombatType.PvE))
        {
            desc += " PvE";
        }
        if(type.HasFlag(CombatType.PvP))
        {
            desc += " PvP";
        }
        if (!string.IsNullOrEmpty(rotation.Description)) desc += "\n \n" + rotation.Description;
        ImguiTooltips.HoveredTooltip(desc);

        var icon = type switch
        {
            CombatType.Both => 61540u,
            CombatType.PvE => 61542u,
            CombatType.PvP => 61544u,
            _ => 61523u,
        };

        if(IconSet.GetTexture(icon, out var texture))
        {
            ImGui.SetCursorPos(cursor + Vector2.One * iconSize / 2);

            ImGui.Image(texture.ImGuiHandle, Vector2.One * iconSize / 2);
        }
    }

    private static void DrawRotationCombo(float comboSize, ICustomRotation[] rotations, ICustomRotation rotation, string gameVersion)
    {
        ImGui.SetNextItemWidth(comboSize);
        const string popUp = "Rotation Solver Select Rotation";

        using (var color = ImRaii.PushColor(ImGuiCol.Text, rotation.GetColor()))
        {
            if (ImGui.Selectable(rotation.RotationName + "##RotationName:" + rotation.Name))
            {
                if (!ImGui.IsPopupOpen(popUp)) ImGui.OpenPopup(popUp);
            }
        }

        using (var popup = ImRaii.Popup(popUp))
        {
            if (popup)
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
            }
        }

        var warning = gameVersion + rotation.GameVersion;
        if (!rotation.IsValid) warning += "\n" + string.Format(LocalizationManager.RightLang.ConfigWindow_Rotation_InvalidRotation,
                rotation.GetType().Assembly.GetInfo().Author);

        if (rotation.IsBeta()) warning += "\n" + LocalizationManager.RightLang.ConfigWindow_Rotation_BetaRotation;

        warning += "\n \n" + LocalizationManager.RightLang.ConfigWindow_Helper_SwitchRotation;
        ImguiTooltips.HoveredTooltip(warning);
    }

    private void DrawBody()
    {
        ImGui.SetCursorPos(ImGui.GetCursorPos() + Vector2.One * 8 * Scale);
        using var child = ImRaii.Child("Rotation Solver Body", -Vector2.One);
        if (child)
        {
            if (_searchResults != null && _searchResults.Any())
            {
                using (var font = ImRaii.PushFont(ImGuiHelper.GetFont(18)))
                {
                    using var color = ImRaii.PushColor(ImGuiCol.Text, ImGui.ColorConvertFloat4ToU32(ImGuiColors.DalamudYellow));
                    ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_Search_Result);
                }

                ImGui.Spacing();

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
        }
    }

    #region About
    private static readonly SortedList<uint, string> CountStringPair = new()
    {
        { 100_000, LocalizationManager.RightLang.ConfigWindow_About_Clicking100k },
        { 500_000, LocalizationManager.RightLang.ConfigWindow_About_Clicking500k },
    };

    private static void DrawAbout()
    {
        using (var font = ImRaii.PushFont(ImGuiHelper.GetFont(18)))
        {
            using var color = ImRaii.PushColor(ImGuiCol.Text, ImGui.ColorConvertFloat4ToU32(ImGuiColors.DalamudYellow));
            ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_About_Punchline);
        }

        ImGui.Spacing();

        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_About_Description);

        ImGui.Spacing();
        using (var color = ImRaii.PushColor(ImGuiCol.Text, ImGui.ColorConvertFloat4ToU32(ImGuiColors.DalamudOrange)))
        {
            ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_About_Warning);
        }

        var width = ImGui.GetWindowWidth();
        if (IconSet.GetTexture("https://discordapp.com/api/guilds/1064448004498653245/embed.png?style=banner2", out var icon) && ImGuiHelper.TextureButton(icon, width, width))
        {
            Util.OpenLink("https://discord.gg/4fECHunam9");
        }

        var clickingCount = OtherConfiguration.RotationSolverRecord.ClickingCount;
        if (clickingCount > 0)
        {
            using var color = ImRaii.PushColor(ImGuiCol.Text, new Vector4(0.2f, 0.6f, 0.95f, 1));
            var countStr = string.Format(LocalizationManager.RightLang.ConfigWindow_About_ClickingCount,
                clickingCount);
            ImGuiHelper.DrawItemMiddle(() =>
            {
                ImGui.TextWrapped(countStr);
            }, width, ImGui.CalcTextSize(countStr).X);

            foreach (var pair in CountStringPair.Reverse())
            {
                if (clickingCount >= pair.Key)
                {
                    countStr = pair.Value;
                    ImGuiHelper.DrawItemMiddle(() =>
                    {
                        ImGui.TextWrapped(countStr);
                    }, width, ImGui.CalcTextSize(countStr).X);
                    break;
                }
            }
        }

        var sayHelloCount = OtherConfiguration.RotationSolverRecord.SayingHelloCount;
        if(sayHelloCount > 0)
        {
            using var color = ImRaii.PushColor(ImGuiCol.Text, new Vector4(0.2f, 0.8f, 0.95f, 1));
            var countStr = string.Format(LocalizationManager.RightLang.ConfigWindow_About_SayHelloCount, sayHelloCount);

            ImGuiHelper.DrawItemMiddle(() =>
            {
                ImGui.TextWrapped(countStr);
            }, width, ImGui.CalcTextSize(countStr).X);
        }

        _aboutHeaders.Draw();
    }

    private static readonly CollapsingHeaderGroup _aboutHeaders = new(new()
    {
        { () => LocalizationManager.RightLang.ConfigWindow_About_Macros, DrawAboutMacros},
        { () => LocalizationManager.RightLang.ConfigWindow_About_Compatibility, DrawAboutCompatibility},
        { () => LocalizationManager.RightLang.ConfigWindow_About_Supporters, DrawAboutSupporters},
        { () => LocalizationManager.RightLang.ConfigWindow_About_Links, DrawAboutLinks},
    });

    private static void DrawAboutMacros()
    {
        using var style = ImRaii.PushStyle(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

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
    }

    private static void DrawAboutCompatibility()
    {
        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_About_Compatibility_Others);

        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_About_Compatibility_Description);

        ImGui.Spacing();

        var iconSize = 40 * Scale;

        using var table = ImRaii.Table("Incompatible plugin", 4, ImGuiTableFlags.BordersInner
        | ImGuiTableFlags.Resizable
        | ImGuiTableFlags.SizingStretchProp);
        if (table)
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
                    if (ImGuiHelper.NoPaddingNoColorImageButton(texture.ImGuiHandle, Vector2.One * iconSize))
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
        }
    }
    private static void DrawAboutSupporters()
    {
        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_About_ThanksToSupporters);

        var width = ImGui.GetWindowWidth();
        using var font = ImRaii.PushFont(ImGuiHelper.GetFont(12));
        using var color = ImRaii.PushColor(ImGuiCol.Text, ImGui.ColorConvertFloat4ToU32(ImGuiColors.DalamudYellow));

        foreach (var name in DownloadHelper.Supporters)
        {
            ImGuiHelper.DrawItemMiddle(() =>
            {
                ImGui.TextWrapped(name);
            }, width, ImGui.CalcTextSize(name).X);
        }
    }

    private static void DrawAboutLinks()
    {
        var width = ImGui.GetWindowWidth();

        if (IconSet.GetTexture("https://GitHub-readme-stats.vercel.app/api/pin/?username=ArchiDog1998&repo=RotationSolver&theme=dark", out var icon) && ImGuiHelper.TextureButton(icon, width, width))
        {
            Util.OpenLink($"https://GitHub.com/{Service.USERNAME}/{Service.REPO}");
        }

        if (IconSet.GetTexture("https://badges.crowdin.net/badge/light/crowdin-on-dark.png", out icon) && ImGuiHelper.TextureButton(icon, width, width))
        {
            Util.OpenLink("https://crowdin.com/project/rotationsolver");
        }

        var text = "My story about FFXIV and Rotation Solver\n - ArchiTed / Youtube";
        var textWidth = ImGuiHelpers.GetButtonSize(text).X;
        ImGuiHelper.DrawItemMiddle(() =>
        {
            if (ImGui.Button(text))
            {
                Util.OpenLink("https://www.youtube.com/watch?v=Adigd5uqDx4");
            }
        }, width, textWidth);


        text = LocalizationManager.RightLang.ConfigWindow_About_OpenConfigFolder;
        textWidth = ImGuiHelpers.GetButtonSize(text).X;
        ImGuiHelper.DrawItemMiddle(() =>
        {
            if (ImGui.Button(text))
            {
                Process.Start("explorer.exe", Svc.PluginInterface.ConfigDirectory.FullName);
            }
        }, width, textWidth);
    }
    #endregion

    #region Rotation
    private static void DrawRotation()
    {
        var rotation = DataCenter.RightNowRotation;
        if (rotation == null) return;

        var desc = rotation.Description;
        if (!string.IsNullOrEmpty(desc))
        {
            using var font = ImRaii.PushFont(ImGuiHelper.GetFont(15));
            ImGuiEx.TextWrappedCopy(desc);
        }

        var wholeWidth = ImGui.GetWindowWidth();
        var type = rotation.GetType();
        var info = type.Assembly.GetInfo();

        if (!string.IsNullOrEmpty(rotation.WhyNotValid))
        {
            var author = info.Author;
            if (string.IsNullOrEmpty(author)) author = "Author";

            using var color = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DPSRed);
            ImGui.TextWrapped(string.Format(rotation.WhyNotValid, author));
        }

        if (!string.IsNullOrEmpty(info.DonateLink))
        {
            if (IconSet.GetTexture("https://storage.ko-fi.com/cdn/brandasset/kofi_button_red.png", out var icon) && ImGuiHelper.TextureButton(icon, wholeWidth, 250 * Scale, "Ko-fi link"))
            {
                Util.OpenLink(info.DonateLink);
            }
        }

        _rotationHeader.Draw();
    }

    private static readonly uint[] RatingColors = new uint[]
    {
        ImGui.ColorConvertFloat4ToU32(ImGuiColors.TankBlue),
        ImGui.ColorConvertFloat4ToU32(ImGuiColors.HealerGreen),
        ImGui.ColorConvertFloat4ToU32(ImGuiColors.DalamudYellow),
        ImGui.ColorConvertFloat4ToU32(ImGuiColors.DalamudOrange),
        ImGui.ColorConvertFloat4ToU32(ImGuiColors.DPSRed),
    };
    private static uint ChangeAlpha(uint color)
    {
        var c = ImGui.ColorConvertU32ToFloat4(color);
        c.W = 0.55f;
        return ImGui.ColorConvertFloat4ToU32(c);
    }

    private static bool DrawRating(float value1, int value2, float max)
    {
        var ratio1 = value1 / max;
        var ratio2 = value2 / max;
        var count = RatingColors.Length;

        var start = ImGui.GetCursorPos() + ImGui.GetWindowPos();

        var spacing = ImGui.GetStyle().ItemSpacing;
        ImGui.GetStyle().ItemSpacing = Vector2.Zero;

        var size = Vector2.Zero;
        using (var font = ImRaii.PushFont(ImGuiHelper.GetFont(16)))
        {
            using (var color = ImRaii.PushColor(ImGuiCol.Text, RatingColors[(int)(ratio1 * count)]))
            {
                ImGui.Text($"{value1:F2}");
            }
            size += ImGui.GetItemRectSize();

            ImGui.SameLine();
            ImGui.Text("/");
            size.X += ImGui.GetItemRectSize().X;

            ImGui.SameLine();

            using (var color = ImRaii.PushColor(ImGuiCol.Text, RatingColors[(int)(ratio2 * count)]))
            {
                ImGui.Text($"{value2}  ");
            }

            size.X += ImGui.GetItemRectSize().X;
        }
        ImGui.GetStyle().ItemSpacing = spacing;

        var radius = size.Y * 0.2f;
        var wholeWidth = ImGui.GetWindowSize().X - size.X - 2 * radius;
        var step = new Vector2(wholeWidth / count, size.Y);
        var shift = new Vector2(0, size.Y * 0.2f);

        var result = ImGuiHelper.IsInRect(start, new Vector2(ImGui.GetWindowSize().X, size.Y));
        if (wholeWidth <= 0) return result;

        var veryStart = start += new Vector2(size.X, 0);

        for (var i = 0; i < count; i++)
        {
            var isStart = i == 0;
            var isLast = i == count - 1;
            var stepThis = step;
            if (isStart || isLast)
            {
                stepThis = step + new Vector2(radius, 0);
            }

            ImGui.GetWindowDrawList().AddRectFilled(start + shift, start + stepThis - shift, ChangeAlpha(RatingColors[i]), radius,
               isStart ? ImDrawFlags.RoundCornersLeft : isLast ? ImDrawFlags.RoundCornersRight : ImDrawFlags.RoundCornersNone);
            start += new Vector2(stepThis.X, 0);
        }

        ImGui.GetWindowDrawList().AddRect(veryStart + shift, start + new Vector2(0, size.Y) - shift, ImGui.ColorConvertFloat4ToU32(ImGuiColors.DalamudWhite2), radius);

        var linePt = veryStart + shift + new Vector2(radius + wholeWidth * ratio1, 0);
        ImGui.GetWindowDrawList().AddLine(linePt, linePt + new Vector2(0, step.Y - 2 * shift.Y), uint.MaxValue, 3);

        linePt = veryStart + shift + new Vector2(radius + wholeWidth * ratio2, 0);
        ImGui.GetWindowDrawList().AddLine(linePt, linePt + new Vector2(0, step.Y - 2 * shift.Y), uint.MaxValue, 3);

        return result;
    }

    private static readonly CollapsingHeaderGroup _rotationHeader = new(new()
    {
        { () => LocalizationManager.RightLang.ConfigWindow_Rotation_Description, DrawRotationDescription },

        { GetRotationStatusHead,  DrawRotationStatus },

        { () => LocalizationManager.RightLang.ConfigWindow_Rotation_Configuration, DrawRotationConfiguration },
        { () => LocalizationManager.RightLang.ConfigWindow_Rotation_Rating, DrawRotationRating },

        { () => LocalizationManager.RightLang.ConfigWindow_Rotation_Information, DrawRotationInformation },
    });

    private static void DrawRotationRating()
    {
        var rotation = DataCenter.RightNowRotation;
        if (rotation == null) return;

        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_Rotation_Rating_Description);

        if (DrawRating((float)rotation.AverageCountOfLastUsing, rotation.MaxCountOfLastUsing, 10))
        {
            ImguiTooltips.ShowTooltip(LocalizationManager.RightLang.ConfigWindow_Rotation_Rating_CountOfLastUsing);
        }
        if (DrawRating((float)rotation.AverageCountOfCombatTimeUsing, rotation.MaxCountOfCombatTimeUsing, 20))
        {
            ImguiTooltips.ShowTooltip(LocalizationManager.RightLang.ConfigWindow_Rotation_Rating_CountOfCombatTimeUsing);
        }
    }

    private const float DESC_SIZE = 24;
    private static void DrawRotationDescription()
    {
        var rotation = DataCenter.RightNowRotation;
        if (rotation == null) return;

        var wholeWidth = ImGui.GetWindowWidth();
        var type = rotation.GetType();

        var attrs = new List<RotationDescAttribute> { RotationDescAttribute.MergeToOne(type.GetCustomAttributes<RotationDescAttribute>()) };

        foreach (var m in type.GetAllMethodInfo())
        {
            attrs.Add(RotationDescAttribute.MergeToOne(m.GetCustomAttributes<RotationDescAttribute>()));
        }

        using (var table = ImRaii.Table("Rotation Description", 2, ImGuiTableFlags.Borders
            | ImGuiTableFlags.Resizable
            | ImGuiTableFlags.SizingStretchProp))
        {
            if (table)
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

                    if (IconSet.GetTexture(attr.IconID, out var image)) ImGui.Image(image.ImGuiHandle, Vector2.One * DESC_SIZE * Scale);

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
                    var size = DESC_SIZE * Scale;
                    var y = ImGui.GetCursorPosY() + size * 4 / 82;
                    foreach (var item in allActions)
                    {
                        if (item == null) continue;

                        if (notStart)
                        {
                            ImGui.SameLine();
                        }

                        if (item.GetTexture(out var texture))
                        {
                            ImGui.SetCursorPosY(y);
                            var cursor = ImGui.GetCursorPos();
                            ImGuiHelper.NoPaddingNoColorImageButton(texture.ImGuiHandle, Vector2.One * size);
                            ImGuiHelper.DrawActionOverlay(cursor, size, 1);
                            ImguiTooltips.HoveredTooltip(item.Name);
                        }
                        notStart = true;
                    }
                }
            }
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

        if (hasTexture && ImGuiHelper.TextureButton(texture, wholeWidth, wholeWidth))
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
        var rotation = DataCenter.RightNowRotation;
        if (Service.Config.GetValue(PluginConfigBool.InDebug))
        {
            return LocalizationManager.RightLang.ConfigWindow_Rotation_Status;
        }
        if (rotation == null || !rotation.ShowStatus) return string.Empty;
        return LocalizationManager.RightLang.ConfigWindow_Rotation_Status;
    }

    private static void DrawRotationStatus()
    {
        DataCenter.RightNowRotation?.DisplayStatus();
    }

    private static string ToCommandStr(OtherCommandType type, string str, string extra = "")
    {
        var result = Service.COMMAND + " " + type.ToString() + " " + str;
        if (!string.IsNullOrEmpty(extra)) result += " " + extra;
        return result;
    }
    private static void DrawRotationConfiguration()
    {
        var rotation = DataCenter.RightNowRotation;
        if (rotation == null) return;

        var enable = rotation.IsEnabled;
        if (ImGui.Checkbox(rotation.Name, ref enable))
        {
            rotation.IsEnabled = enable;
        }
        if (!enable) return;

        var set = rotation.Configs;

        if (set.Any()) ImGui.Separator();

        foreach (var config in set.Configs)
        {
            if(DataCenter.Territory?.IsPvpZone ?? false)
            {
                if (!config.Type.HasFlag(CombatType.PvP)) continue;
            }
            else
            {
                if (!config.Type.HasFlag(CombatType.PvE)) continue;
            }

            var key = config.Name;
            var name = $"##{config.GetHashCode()}_{config.Name}";
            string command = ToCommandStr(OtherCommandType.Rotations, config.Name, config.DefaultValue);
            void Reset() => set.SetValue(config.Name, config.DefaultValue);

            ImGuiHelper.PrepareGroup(key, command, Reset);

            if (config is RotationConfigCombo c)
            {
                var val = set.GetCombo(c.Name);
                ImGui.SetNextItemWidth(ImGui.CalcTextSize(c.Items[val]).X + 50 * Scale);
                var openCombo = ImGui.BeginCombo(name, c.Items[val]);
                ImGuiHelper.ReactPopup(key, command, Reset);
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
                ImGuiHelper.ReactPopup(key, command, Reset);
            }
            else if (config is RotationConfigFloat f)
            {
                float val = set.GetFloat(config.Name);
                ImGui.SetNextItemWidth(Scale * Searchable.DRAG_WIDTH);
                if (f.UnitType == ConfigUnitType.Percent)
                {
                    var v = (int)val * 100;
                    if (ImGui.SliderInt(name, ref v, (int)(f.Min * 100), (int)(f.Max * 100)))
                    {
                        set.SetValue(config.Name, (v / 100f).ToString());
                    }
                }
                else
                {
                    if (ImGui.DragFloat(name, ref val, f.Speed, f.Min, f.Max))
                    {
                        set.SetValue(config.Name, val.ToString());
                    }
                }

                f.UnitType.Draw();
                ImGuiHelper.ReactPopup(key, command, Reset);
            }
            else if (config is RotationConfigString s)
            {
                string val = set.GetString(config.Name);

                ImGui.SetNextItemWidth(ImGui.GetWindowWidth());
                if (ImGui.InputTextWithHint(name, config.DisplayName, ref val, 128))
                {
                    set.SetValue(config.Name, val.ToString());
                }
                ImGuiHelper.ReactPopup(key, command, Reset);
                continue;
            }
            else if (config is RotationConfigInt i)
            {
                int val = set.GetInt(config.Name);
                ImGui.SetNextItemWidth(Scale * Searchable.DRAG_WIDTH);
                if (ImGui.DragInt(name, ref val, i.Speed, i.Min, i.Max))
                {
                    set.SetValue(config.Name, val.ToString());
                }
                ImGuiHelper.ReactPopup(key, command, Reset);
            }
            else continue;

            ImGui.SameLine();
            ImGui.TextWrapped(config.DisplayName);
            ImGuiHelper.ReactPopup(key, command, Reset, false);
        }
    }

    private static void DrawRotationInformation()
    {
        var rotation = DataCenter.RightNowRotation;
        if (rotation == null) return;

        var youtubeLink = rotation.GetType().GetCustomAttribute<YoutubeLinkAttribute>()?.ID;

        var wholeWidth = ImGui.GetWindowWidth();
        if (!string.IsNullOrEmpty(youtubeLink))
        {
            ImGui.NewLine();
            if (IconSet.GetTexture("https://www.gstatic.com/youtube/img/branding/youtubelogo/svg/youtubelogo.svg", out var icon) && ImGuiHelper.TextureButton(icon, wholeWidth, 250 * Scale, "Youtube Link"))
            {
                Util.OpenLink("https://www.youtube.com/watch?v=" + youtubeLink);
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
                DrawGitHubBadge(userName, repository, link.Path, $"https://github.com/{userName}/{repository}/blob/{link.Path}", center: true);
            }
            ImGui.NewLine();

            ImGuiHelper.DrawItemMiddle(() =>
            {
                using var group = ImRaii.Group();
                if (group)
                {
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
                }

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

        using var table = ImRaii.Table("Rotation Solver Actions", 2, ImGuiTableFlags.Resizable);

        if (table)
        {
            ImGui.TableSetupColumn("Action Column", ImGuiTableColumnFlags.WidthFixed, ImGui.GetWindowWidth() / 2);
            ImGui.TableNextColumn();

            if (_actionsList != null)
            {
                _actionsList.ClearCollapsingHeader();

                if (DataCenter.RightNowRotation != null)
                {
                    var size = 30 * Scale;
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
                                if (ImGuiHelper.NoPaddingNoColorImageButton(icon.ImGuiHandle, Vector2.One * size, item.Name))
                                {
                                    _activeAction = item;
                                }
                                ImGuiHelper.DrawActionOverlay(cursor, size, _activeAction == item ? 1 : 0);

                                if (IconSet.GetTexture("ui/uld/readycheck_hr1.tex", out var texture))
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
                                ImGuiHelper.DrawHotKeysPopup(key, cmd);
                                ImGuiHelper.ExecuteHotKeysPopup(key, cmd, item.Name, false);
                            }
                        });
                    }
                }

                _actionsList.Draw();
            }

            ImGui.TableNextColumn();

            if (Service.Config.GetValue(PluginConfigBool.InDebug))
            {
                if (_activeAction is IBaseAction action)
                {

                    try
                    {
#if DEBUG
                        ImGui.Text("Is Real GCD: " + action.IsRealGCD.ToString());
                        ImGui.Text("Status: " + FFXIVClientStructs.FFXIV.Client.Game.ActionManager.Instance()->GetActionStatus(FFXIVClientStructs.FFXIV.Client.Game.ActionType.Action, action.AdjustedID).ToString());
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
                ImGuiHelper.DrawHotKeysPopup(key, cmd);
                ImGuiHelper.ExecuteHotKeysPopup(key, cmd, string.Empty, false);

                enable = _activeAction.IsInCooldown;
                if (ImGui.Checkbox($"{LocalizationManager.RightLang.ConfigWindow_Actions_ShowOnCDWindow}##{_activeAction.Name}InCooldown", ref enable))
                {
                    _activeAction.IsInCooldown = enable;
                }

                if (_activeAction is IBaseAction action)
                {
                    ImGui.Separator();

                    var ttk = action.TimeToKill;
                    ImGui.SetNextItemWidth(Scale * 150);
                    if (ImGui.DragFloat($"{LocalizationManager.RightLang.ConfigWindow_Actions_TTK}##{action}",
                        ref ttk, 0.1f, 0, 120))
                    {
                        action.TimeToKill = ttk;
                    }

                    if (!action.IsSingleTarget)
                    {
                        var aoeCount = (int)action.AOECount;
                        ImGui.SetNextItemWidth(Scale * 150);
                        if (ImGui.DragInt($"{LocalizationManager.RightLang.ConfigWindow_Actions_AOECount}##{action}",
                            ref aoeCount, 0.05f, 1, 10))
                        {
                            action.AOECount = (byte)aoeCount;
                        }
                    }

                    if (action.IsHeal)
                    {
                        var ratio = action.AutoHealRatio;
                        ImGui.SetNextItemWidth(Scale * 150);
                        if (ImGui.DragFloat($"{LocalizationManager.RightLang.ConfigWindow_Actions_HealRatio}##{action}",
                            ref ratio, 0.002f, 0, 1))
                        {
                            action.AutoHealRatio = ratio;
                        }
                    }
                }

                ImGui.Separator();
            }

            ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_Actions_ConditionDescription);
            _sequencerList?.Draw();
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

            var rotation = DataCenter.RightNowRotation;
            var set = DataCenter.RightSet;

            if (set == null || _activeAction == null || rotation == null) return;
            set.GetCondition(_activeAction.ID)?.DrawMain(rotation);
        } },

        { () => LocalizationManager.RightLang.ConfigWindow_Actions_DisabledConditionSet, () =>
        {
            ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_Actions_DisabledConditionSet_Description);

            var rotation = DataCenter.RightNowRotation;
            var set = DataCenter.RightSet;

            if (set == null || _activeAction == null || rotation == null) return;
            set.GetDisabledCondition(_activeAction.ID)?.DrawMain(rotation);
        } },
    })
    {
        HeaderSize = 18,
    };
    #endregion

    #region Rotations
    private static void DrawRotations()
    {
        var width = ImGui.GetWindowWidth();


        var text = LocalizationManager.RightLang.ConfigWindow_Rotations_Download;
        var textWidth = ImGuiHelpers.GetButtonSize(text).X;

        ImGuiHelper.DrawItemMiddle(() =>
        {
            if (ImGui.Button(text))
            {
                Task.Run(async () =>
                {
                    await RotationUpdater.GetAllCustomRotationsAsync(DownloadOption.MustDownload | DownloadOption.ShowList);
                });
            }
        }, width, textWidth);

        text = LocalizationManager.RightLang.ConfigWindow_Rotations_Links;
        textWidth = ImGuiHelpers.GetButtonSize(text).X;
        ImGuiHelper.DrawItemMiddle(() =>
        {
            if (ImGui.Button(text))
            {
                Util.OpenLink($"https://github.com/{Service.USERNAME}/{Service.REPO}/blob/main/RotationsLink.md");
            }
        }, width, textWidth);

        _rotationsHeader?.Draw();
    }
    private static readonly CollapsingHeaderGroup _rotationsHeader = new(new()
    {
        {() => LocalizationManager.RightLang.ConfigWindow_Rotations_Settings, DrawRotationsSettings},
        {() => LocalizationManager.RightLang.ConfigWindow_Rotations_Loaded, DrawRotationsLoaded},
        {() => LocalizationManager.RightLang.ConfigWindow_Rotations_GitHub, DrawRotationsGitHub},
        {() => LocalizationManager.RightLang.ConfigWindow_Rotations_Libraries, DrawRotationsLibraries},
    });

    private static readonly CheckBoxSearchPlugin _updateRotation = new(PluginConfigBool.DownloadRotations,
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

        using var table = ImRaii.Table("Rotation Solver AssemblyTable", 3, ImGuiTableFlags.BordersInner
            | ImGuiTableFlags.Resizable
            | ImGuiTableFlags.SizingStretchProp);

        if (table)
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

                    if (IconSet.GetTexture(IconSet.GetJobIcon(jobs.First(), IconType.Framed), out var texture, 62574))
                        ImGui.Image(texture.ImGuiHandle, Vector2.One * 30 * Scale);

                    ImguiTooltips.HoveredTooltip(string.Join('\n', jobs));
                }

                ImGui.TableNextColumn();

                DrawGitHubBadge(info.GitHubUserName, info.GitHubRepository, info.FilePath);

                if (!string.IsNullOrEmpty(info.DonateLink)
                    && IconSet.GetTexture("https://storage.ko-fi.com/cdn/brandasset/kofi_button_red.png", out var icon)
                    && ImGuiHelper.NoPaddingNoColorImageButton(icon.ImGuiHandle, new Vector2(1, (float)icon.Height / icon.Width) * MathF.Min(250, icon.Width) * Scale, info.FilePath))
                {
                    Util.OpenLink(info.DonateLink);
                }
            }
        }
    }

    private static void DrawGitHubBadge(string userName, string repository, string id = "", string link = "", bool center = false)
    {
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(repository)) return;

        var wholeWidth = ImGui.GetWindowWidth();

        link = string.IsNullOrEmpty(link) ? $"https://GitHub.com/{userName}/{repository}" : link;

        if (IconSet.GetTexture($"https://github-readme-stats.vercel.app/api/pin/?username={userName}&repo={repository}&theme=dark", out var icon)
            && (center ? ImGuiHelper.TextureButton(icon, wholeWidth, icon.Width, id)
            : ImGuiHelper.NoPaddingNoColorImageButton(icon.ImGuiHandle, new Vector2(icon.Width, icon.Height), id)))
        {
            Util.OpenLink(link);
        }

        var hasDate = IconSet.GetTexture($"https://img.shields.io/github/release-date/{userName}/{repository}?style=for-the-badge", out var releaseDate);

        var hasCount = IconSet.GetTexture($"https://img.shields.io/github/downloads/{userName}/{repository}/latest/total?style=for-the-badge&label=", out var downloadCount);

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

    private static void DrawRotationsGitHub()
    {
        if (!Service.Config.GlobalConfig.GitHubLibs.Any(s => string.IsNullOrEmpty(s) || s == "||"))
        {
            Service.Config.GlobalConfig.GitHubLibs = Service.Config.GlobalConfig.GitHubLibs.Append("||").ToArray();
        }

        ImGui.Spacing();

        foreach (var gitHubLink in DownloadHelper.LinkLibraries ?? Array.Empty<string>())
        {
            var strs = gitHubLink.Split('|');
            var userName = strs.FirstOrDefault() ?? string.Empty;
            var repository = strs.Length > 1 ? strs[1] : string.Empty;
            var fileName = strs.LastOrDefault() ?? string.Empty;

            DrawGitHubBadge(userName, repository, fileName, center: true);
            ImGui.Spacing();
            ImGui.Separator();
        }

        int removeIndex = -1;
        for (int i = 0; i < Service.Config.GlobalConfig.GitHubLibs.Length; i++)
        {
            var strs = Service.Config.GlobalConfig.GitHubLibs[i].Split('|');
            var userName = strs.FirstOrDefault() ?? string.Empty;
            var repository = strs.Length > 1 ? strs[1] : string.Empty;
            var fileName = strs.LastOrDefault() ?? string.Empty;

            DrawGitHubBadge(userName, repository, fileName, center: true);

            var changed = false;

            var width = ImGui.GetWindowWidth() - ImGuiEx.CalcIconSize(FontAwesomeIcon.Ban).X - ImGui.GetStyle().ItemSpacing.X * 3 - 10 * Scale;
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

            if (changed)
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

        var width = ImGui.GetWindowWidth() - ImGuiEx.CalcIconSize(FontAwesomeIcon.Ban).X - ImGui.GetStyle().ItemSpacing.X - 10 * Scale;

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
                    .Where(s => !s.CanDispel && !s.LockMovement && !s.IsGaze && !s.IsFcBuff && s.ClassJobCategory.Row == 1 && s.StatusCategory == 1
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

        using var table = ImRaii.Table("Rotation Solver List Statuses", 2, ImGuiTableFlags.BordersInner | ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingStretchSame);
        if (table)
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

        var count = Math.Max(1, (int)MathF.Floor(ImGui.GetColumnWidth() / (24 * Scale + ImGui.GetStyle().ItemSpacing.X)));
        var index = 0;

        if (IconSet.GetTexture(16220, out var text))
        {
            if (index++ % count != 0)
            {
                ImGui.SameLine();
            }
            if (ImGuiHelper.NoPaddingNoColorImageButton(text.ImGuiHandle, new Vector2(24, 32) * Scale, name))
            {
                if (!ImGui.IsPopupOpen(popupId)) ImGui.OpenPopup(popupId);
            }
            ImguiTooltips.HoveredTooltip(LocalizationManager.RightLang.ConfigWindow_List_AddStatus);
        }

        foreach (var status in statuses.Select(a => Service.GetSheet<Status>().GetRow(a))
            .Where(a => a != null)
            .OrderByDescending(s => Similarity(s.Name + " " + s.RowId.ToString(), _statusSearching)))
        {
            void Delete() => removeId = status.RowId;

            var key = "Status" + status.RowId.ToString();

            ImGuiHelper.DrawHotKeysPopup(key, string.Empty, (LocalizationManager.RightLang.ConfigWindow_List_Remove, Delete, new string[] { "Delete" }));

            if (IconSet.GetTexture(status.Icon, out var texture, notLoadId))
            {
                if (index++ % count != 0)
                {
                    ImGui.SameLine();
                }
                ImGuiHelper.NoPaddingNoColorImageButton(texture.ImGuiHandle, new Vector2(24, 32) * Scale, "Status" + status.RowId.ToString());

                ImGuiHelper.ExecuteHotKeysPopup(key, string.Empty, $"{status.Name} ({status.RowId})", false,
                    (Delete, new Dalamud.Game.ClientState.Keys.VirtualKey[] { Dalamud.Game.ClientState.Keys.VirtualKey.DELETE }));
            }
        }

        if (removeId != 0)
        {
            statuses.Remove(removeId);
            OtherConfiguration.Save();
        }
    }

    internal static void StatusPopUp(string popupId, Status[] allStatus, ref string searching, Action<Status> clicked, uint notLoadId = 10100, float size = 32)
    {
        using var popup = ImRaii.Popup(popupId);
        if (popup)
        {
            ImGui.SetNextItemWidth(200 * Scale);
            ImGui.InputTextWithHint("##Searching the status", LocalizationManager.RightLang.ConfigWindow_List_StatusNameOrId, ref searching, 128);

            ImGui.Spacing();

            using var child = ImRaii.Child("Rotation Solver Add Status", new Vector2(-1, 400 * Scale));
            if (child)
            {
                var count = Math.Max(1, (int)MathF.Floor(ImGui.GetWindowWidth() / (size * 3 / 4 * Scale + ImGui.GetStyle().ItemSpacing.X)));
                var index = 0;

                var searchingKey = searching;
                foreach (var status in allStatus.OrderByDescending(s => Similarity(s.Name + " " + s.RowId.ToString(), searchingKey)))
                {
                    if (IconSet.GetTexture(status.Icon, out var texture, notLoadId))
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
                        ImguiTooltips.HoveredTooltip($"{status.Name} ({status.RowId})");
                    }
                }
            }
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

        using var table = ImRaii.Table("Rotation Solver List Actions", 2, ImGuiTableFlags.BordersInner | ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingStretchSame);
        if (table)
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
        }
    }

    private static string _actionSearching = string.Empty;
    private static void DrawActionsList(string name, HashSet<uint> actions)
    {
        uint removeId = 0;

        var popupId = "Rotation Solver Action Popup" + name;

        if (ImGui.Button(LocalizationManager.RightLang.ConfigWindow_List_AddAction + "##" + name))
        {
            if (!ImGui.IsPopupOpen(popupId)) ImGui.OpenPopup(popupId);
        }

        ImGui.Spacing();

        foreach (var action in actions.Select(a => Service.GetSheet<GAction>().GetRow(a))
            .Where(a => a != null)
            .OrderByDescending(s => Similarity(s.Name + " " + s.RowId.ToString(), _actionSearching)))
        {
            void Reset() => removeId = action.RowId;

            var key = "Action" + action.RowId.ToString();

            ImGuiHelper.DrawHotKeysPopup(key, string.Empty, (LocalizationManager.RightLang.ConfigWindow_List_Remove, Reset, new string[] { "Delete" }));

            ImGui.Selectable($"{action.Name} ({action.RowId})");

            ImGuiHelper.ExecuteHotKeysPopup(key, string.Empty, string.Empty, false, (Reset, new Dalamud.Game.ClientState.Keys.VirtualKey[] { Dalamud.Game.ClientState.Keys.VirtualKey.DELETE }));
        }

        if (removeId != 0)
        {
            actions.Remove(removeId);
            OtherConfiguration.Save();
        }

        using var popup = ImRaii.Popup(popupId);
        if (popup)
        {
            ImGui.SetNextItemWidth(200 * Scale);
            ImGui.InputTextWithHint("##Searching the action pop up", LocalizationManager.RightLang.ConfigWindow_List_ActionNameOrId, ref _actionSearching, 128);

            ImGui.Spacing();

            using var child = ImRaii.Child("Rotation Solver Add action", new Vector2(-1, 400 * Scale));
            if (child)
            {
                foreach (var action in AllActions.OrderByDescending(s => Similarity(s.Name + " " + s.RowId.ToString(), _actionSearching)))
                {
                    var selected = ImGui.Selectable($"{action.Name} ({action.RowId})");
                    if (ImGui.IsItemHovered())
                    {
                        ImguiTooltips.ShowTooltip($"{action.Name} ({action.RowId})");
                        if (selected)
                        {
                            actions.Add(action.RowId);
                            OtherConfiguration.Save(); ImGui.CloseCurrentPopup();
                        }
                    }
                }
            }
        }
    }

    public static Vector3 HoveredPosition { get; private set; } = Vector3.Zero;
    private static void DrawListTerritories()
    {
        if (Svc.ClientState == null) return;

        var territoryId = Svc.ClientState.TerritoryType;

        using (var font = ImRaii.PushFont(ImGuiHelper.GetFont(21)))
        {
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
            var getIcon = IconSet.GetTexture(icon, out var texture);
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

        using var table = ImRaii.Table("Rotation Solver List Territories", 3, ImGuiTableFlags.BordersInner | ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingStretchSame);
        if (table)
        {
            ImGui.TableSetupScrollFreeze(0, 1);
            ImGui.TableNextRow(ImGuiTableRowFlags.Headers);

            ImGui.TableNextColumn();
            ImGui.TableHeader(LocalizationManager.RightLang.ConfigWindow_List_NoHostile);

            ImGui.TableNextColumn();
            ImGui.TableHeader(LocalizationManager.RightLang.ConfigWindow_List_NoProvoke);

            ImGui.TableNextColumn();
            ImGui.TableHeader(LocalizationManager.RightLang.ConfigWindow_List_BeneficialPositions);

            ImGui.TableNextRow();

            ImGui.TableNextColumn();

            ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_List_NoHostileDesc);

            var width = ImGui.GetColumnWidth() - ImGuiEx.CalcIconSize(FontAwesomeIcon.Ban).X - ImGui.GetStyle().ItemSpacing.X - 10 * Scale;

            if (!OtherConfiguration.NoHostileNames.TryGetValue(territoryId, out var libs))
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
                if (ImGui.InputTextWithHint($"##Rotation Solver Territory Target Name {i}", LocalizationManager.RightLang.ConfigWindow_List_NoHostilesName, ref libs[i], 1024))
                {
                    OtherConfiguration.NoHostileNames[territoryId] = libs;
                    OtherConfiguration.SaveNoHostileNames();
                }
                ImGui.SameLine();

                if (ImGuiEx.IconButton(FontAwesomeIcon.Ban, $"##Rotation Solver Remove Territory Target Name {i}"))
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

            ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_List_NoProvokeDesc);

            width = ImGui.GetColumnWidth() - ImGuiEx.CalcIconSize(FontAwesomeIcon.Ban).X - ImGui.GetStyle().ItemSpacing.X - 10 * Scale;

            if (!OtherConfiguration.NoProvokeNames.TryGetValue(territoryId, out libs))
            {
                OtherConfiguration.NoProvokeNames[territoryId] = libs = Array.Empty<string>();
            }
            //Add one.
            if (!libs.Any(string.IsNullOrEmpty))
            {
                OtherConfiguration.NoProvokeNames[territoryId] = libs.Append(string.Empty).ToArray();
            }
            removeIndex = -1;
            for (int i = 0; i < libs.Length; i++)
            {
                ImGui.SetNextItemWidth(width);
                if (ImGui.InputTextWithHint($"##Rotation Solver Territory Provoke Name {i}", LocalizationManager.RightLang.ConfigWindow_List_NoProvokeName, ref libs[i], 1024))
                {
                    OtherConfiguration.NoProvokeNames[territoryId] = libs;
                    OtherConfiguration.SaveNoProvokeNames();
                }
                ImGui.SameLine();

                if (ImGuiEx.IconButton(FontAwesomeIcon.Ban, $"##Rotation Solver Remove Territory Provoke Name {i}"))
                {
                    removeIndex = i;
                }
            }
            if (removeIndex > -1)
            {
                var list = libs.ToList();
                list.RemoveAt(removeIndex);
                OtherConfiguration.NoProvokeNames[territoryId] = list.ToArray();
                OtherConfiguration.SaveNoProvokeNames();
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

                ImGuiHelper.DrawHotKeysPopup(key, string.Empty, (LocalizationManager.RightLang.ConfigWindow_List_Remove, Reset, new string[] { "Delete" }));

                ImGui.Selectable(pts[i].ToString());

                if (ImGui.IsItemHovered())
                {
                    HoveredPosition = pts[i];
                }

                ImGuiHelper.ExecuteHotKeysPopup(key, string.Empty, string.Empty, false, (Reset, new Dalamud.Game.ClientState.Keys.VirtualKey[] { Dalamud.Game.ClientState.Keys.VirtualKey.DELETE }));
            }
            if (removeIndex > -1)
            {
                var list = pts.ToList();
                list.RemoveAt(removeIndex);
                OtherConfiguration.BeneficialPositions[territoryId] = list.ToArray();
                OtherConfiguration.SaveBeneficialPositions();
            }
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

        _debugHeader?.Draw();
    }
    private static readonly ISearchable[] _debugSearchable = new ISearchable[]
    {
        new CheckBoxSearchPlugin(PluginConfigBool.InDebug),
    };

    private static readonly CollapsingHeaderGroup _debugHeader = new(new()
    {
        {() => DataCenter.RightNowRotation != null ? "Rotation" : string.Empty, DrawDebugRotationStatus},
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
        DataCenter.RightNowRotation?.DisplayStatus();
    }

    private static unsafe void DrawStatus()
    {
        if ((IntPtr)FateManager.Instance() != IntPtr.Zero)
        {
            ImGui.Text("Fate: " + DataCenter.FateId.ToString());
        }
        ImGui.Text("Height: " + Player.Character->CalculateHeight().ToString());
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

        ImGui.Text($"Your combat state: {DataCenter.InCombat}");
        ImGui.Text($"Your character combat: {Player.Object.InCombat()}");
        foreach (var p in Svc.Party)
        {
            if (p.GameObject is not BattleChara b) continue;
            ImGui.Text($"In Combat: {b.InCombat()}");
        }
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
            ImGui.Text("Is Boss TTK: " + b.IsBossFromTTK().ToString());
            ImGui.Text("Is Boss Icon: " + b.IsBossFromIcon().ToString());
            ImGui.Text("Rank: " + b.GetObjectNPC().Rank.ToString());
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
        ImGui.Text(DataCenter.RightNowRotation.RotationName);
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
        //ImGui.Text("Hate");
        //foreach (var hate in UIState.Instance()->Hate.HateArraySpan)
        //{
        //    var name = Svc.Objects.SearchById(hate.ObjectId)?.Name ?? "Unknown";
        //    ImGui.Text($"{name} : {hate.Enmity}");
        //}
        //ImGui.Spacing();
        //ImGui.Text("Hater");
        //foreach (var hater in UIState.Instance()->Hater.HaterArraySpan)
        //{
        //    var name = Svc.Objects.SearchById(hater.ObjectId)?.Name ?? "Unknown";
        //    ImGui.Text($"{name} : {hater.Enmity}");
        //}

        ImGui.Text(CustomRotation.LimitBreakLevel.ToString());
    }

    private static void DrawAction(ActionID id, string type)
    {
        ImGui.Text($"{type}: {id}");
    }
    #endregion

    #region Child
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