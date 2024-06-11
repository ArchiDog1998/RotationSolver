using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.Colors;
using Dalamud.Interface.GameFonts;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Configuration;
using RotationSolver.Helpers;
using RotationSolver.UI.SearchableConfigs;
using RotationSolver.Updaters;
using System.Diagnostics;
using XIVConfigUI;
using XIVConfigUI.SearchableConfigs;
using XIVDrawer;
using GAction = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.UI.ConfigWindows;

public class RotationConfigWindow : ConfigWindow
{
    public override SearchableCollection Collection { get; } = new(Service.Config,
            new SearchableConfigRS());

    protected override string Kofi => "B0B0IN5DX";

    protected override string Patreon => "archidog1998";
    protected override string Crowdin => "rotationsolver";

    protected override string DiscordServerID => "1228953752585637908";
    protected override string DiscordServerInviteLink => "9D4E8eZW5g";

    public override IEnumerable<Searchable> Searchables => [.. base.Searchables, .. DataCenter.RightNowRotation?.Configs];

    public RotationConfigWindow()
        : base(typeof(RotationConfigWindow).Assembly.GetName())
    {
        Size = new Vector2(740f, 490f);

        ImGuiHelper.GetFont(FontSize.Third, GameFontFamily.Axis);
        ImGuiHelper.GetFont(FontSize.Fourth, GameFontFamily.Axis);
        ImGuiHelper.GetFont(FontSize.Fifth, GameFontFamily.Axis);
        ImGuiHelper.GetFont(FontSize.Fourth, GameFontFamily.MiedingerMid);
        ImGuiHelper.GetFont(FontSize.Fifth, GameFontFamily.MiedingerMid);
    }

    public override void OnClose()
    {
        Service.Config.Save();
        ActionSequencerUpdater.SaveFiles();
        base.OnClose();
    }

    private static void DrawDutyRotation()
    {
        var dutyRotation = DataCenter.RightNowDutyRotation;
        if (dutyRotation == null) return;

        var rot = dutyRotation.GetType().GetCustomAttribute<RotationAttribute>();
        if (rot == null) return;

        if (!RotationUpdater.DutyRotations.TryGetValue(Svc.ClientState.TerritoryType, out var rotations)) return;

        if (rotations == null) return;

        const string popUpId = "Right Duty Rotation Popup";
        if (ImGui.Selectable(rot.Name, false, ImGuiSelectableFlags.None, new Vector2(0, 20)))
        {
            ImGui.OpenPopup(popUpId);
        }
        ImguiTooltips.HoveredTooltip(UiString.ConfigWindow_DutyRotationDesc.Local());

        using var popup = ImRaii.Popup(popUpId);
        if (!popup) return;

        foreach (var type in rotations)
        {
            var r = type.GetCustomAttribute<RotationAttribute>();
            if (r == null) continue;

            if (ImGui.Selectable("None"))
            {
                Service.Config.DutyRotationChoice = string.Empty;
            }

            if (ImGui.Selectable(r.Name) && !string.IsNullOrEmpty(type.FullName))
            {
                Service.Config.DutyRotationChoice = type.FullName;
            }
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
        ImguiTooltips.HoveredTooltip(UiString.ConfigWindow_ConditionSetDesc.Local());

        using var popup = ImRaii.Popup(popUpId);
        if (!popup) return;

        var combos = DataCenter.ConditionSets;
        for (int i = 0; i < combos.Length; i++)
        {
            void DeleteFile()
            {
                ActionSequencerUpdater.Delete(combos[i].Name);
            }

            if (combos[i].Name == set.Name)
            {
                ImGuiHelperRS.SetNextWidthWithName(set.Name);
                ImGui.InputText("##MajorConditionSet", ref set.Name, 100);
            }
            else
            {
                var key = "Condition Set At " + i.ToString();
                ImGuiHelper.DrawHotKeysPopup(key, string.Empty, (LocalString.Remove.Local(), DeleteFile, ["Delete"]));

                if (ImGui.Selectable(combos[i].Name))
                {
                    Service.Config.ActionSequencerIndex = i;
                }

                ImGuiHelper.ExecuteHotKeysPopup(key, string.Empty, string.Empty, false,
                    (DeleteFile, [VirtualKey.DELETE]));
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
        ImguiTooltips.HoveredTooltip(UiString.ActionSequencer_Load.Local());
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
                    var url = $"https://raw.githubusercontent.com/{XIVConfigUIMain.UserName}/{XIVConfigUIMain.RepoName}/main/Images/{name}.png";

                    using var client = new HttpClient();
                    var stream = await client.GetStreamAsync(url);

                    using var fs = new FileStream(file, FileMode.CreateNew);

                    await stream.CopyToAsync(fs);
                }


                _loadingList.Remove(name);
            });
        }

        return _logosWrap.TryGetValue(name, out texture!);
    }

    protected override bool DrawSubHeader(float wholeWidth)
    {
        DrawDutyRotation();
        DrawConditionSet();

        return true;
    }

    protected override bool GetLogo(out IDalamudTextureWrap texture)
    {
        var frame = Environment.TickCount / 34 % FRAME_COUNT;
        if (frame <= 0) frame += FRAME_COUNT;

        return GetLocalImage(Service.Config.DrawIconAnimation
                    ? frame.ToString("D4") : "Logo", out texture);
    }

    protected override void DrawHeader(float wholeWidth, float iconSize)
    {
        base.DrawHeader(wholeWidth, iconSize);

        var rotation = DataCenter.RightNowRotation;
        if (rotation == null) return;

        var rotations = RotationUpdater.CustomRotations.FirstOrDefault(i => i.ClassJobIds.Contains((Job)(Player.Object?.ClassJob.Id ?? 0)))?.Rotations ?? [];

        var rot = rotation.GetType().GetCustomAttribute<RotationAttribute>();

        if (rot == null) return;

        if (DataCenter.IsPvP)
        {
            rotations = rotations.Where(r => r.GetCustomAttribute<RotationAttribute>()?.Type.HasFlag(CombatType.PvP) ?? false).ToArray();
        }
        else
        {
            rotations = rotations.Where(r => r.GetCustomAttribute<RotationAttribute>()?.Type.HasFlag(CombatType.PvE) ?? false).ToArray();
        }

        var comboSize = ImGui.CalcTextSize(rot.Name).X;

        const string slash = " - ";
        var gameVersionSize = ImGui.CalcTextSize(slash + rot.GameVersion).X + ImGui.GetStyle().ItemSpacing.X;
        var gameVersion = UiString.ConfigWindow_Helper_GameVersion.Local() + ": ";
        var drawCenter = ImGui.CalcTextSize(slash + gameVersion + rot.GameVersion).X + iconSize + ImGui.GetStyle().ItemSpacing.X * 3 < wholeWidth;
        if (drawCenter) gameVersionSize += ImGui.CalcTextSize(gameVersion).X + ImGui.GetStyle().ItemSpacing.X;

        var horizonalWholeWidth = Math.Max(comboSize, gameVersionSize) + iconSize + ImGui.GetStyle().ItemSpacing.X;

        if (horizonalWholeWidth > wholeWidth)
        {
            ImGuiHelper.DrawItemMiddle(() =>
            {
                DrawRotationIcon(rotation, iconSize);
            }, wholeWidth, iconSize);

            if (MaxIconWidth < wholeWidth)
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
                ImGui.Text(rot.GameVersion);

            }, wholeWidth, horizonalWholeWidth);
        }
    }

    private void DrawRotationIcon(ICustomRotation rotation, float iconSize)
    {
        var cursor = ImGui.GetCursorPos();
        if (rotation.GetTexture(out var jobIcon) && ImGuiHelper.SilenceImageButton(jobIcon.ImGuiHandle,
            Vector2.One * iconSize, _activeTabIndex == 0))
        {
            _activeTabIndex = 0;
            _searchResults = [];
        }
        if (ImGui.IsItemHovered())
        {
            ImguiTooltips.ShowTooltip(() =>
            {
                ImGui.Text(rotation.Name + $" ({rotation.GetType().GetCustomAttribute<RotationAttribute>()!.Name})");
                rotation.GetType().GetCustomAttribute<RotationAttribute>()!.Type.Draw();
                if (!string.IsNullOrEmpty(rotation.Description))
                {
                    ImGui.Text(rotation.Description);
                }
            });
        }

        if (ImageLoader.GetTexture(rotation.GetType().GetCustomAttribute<RotationAttribute>()!.Type.GetIcon(), out var texture))
        {
            ImGui.SetCursorPos(cursor + Vector2.One * iconSize / 2);

            ImGui.Image(texture.ImGuiHandle, Vector2.One * iconSize / 2);
        }
    }

    private static void DrawRotationCombo(float comboSize, Type[] rotations, ICustomRotation rotation, string gameVersion)
    {
        ImGui.SetNextItemWidth(comboSize);
        const string popUp = "Rotation Solver Select Rotation";

        var rot = rotation.GetType().GetCustomAttribute<RotationAttribute>()!;

        using (var color = ImRaii.PushColor(ImGuiCol.Text, rotation.GetColor()))
        {
            if (ImGui.Selectable(rot.Name + "##RotationName:" + rotation.Name))
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
                    var rAttr = r.GetCustomAttribute<RotationAttribute>()!;

                    if (ImageLoader.GetTexture(rAttr.Type.GetIcon(), out var texture))
                    {
                        ImGui.Image(texture.ImGuiHandle, Vector2.One * 20 * Scale);
                        if (ImGui.IsItemHovered())
                        {
                            ImguiTooltips.ShowTooltip(() =>
                            {
                                rotation.GetType().GetCustomAttribute<RotationAttribute>()!.Type.Draw();
                            });
                        }
                    }

                    ImGui.SameLine();

                    DownloadHelper.GetRating(r, out var rate);

                    using (var font = ImRaii.PushFont(ImGuiHelper.GetFont(FontSize.Fifth, GameFontFamily.MiedingerMid)))
                    {
                        ImGui.TextColored(ImGuiColors.DalamudYellow, rate);
                    }

                    ImGui.SameLine();

                    ImGui.PushStyleColor(ImGuiCol.Text, r.GetCustomAttribute<BetaRotationAttribute>() == null
                        ? ImGuiColors.DalamudWhite : ImGuiColors.DalamudOrange);
                    if (ImGui.Selectable(rAttr.Name))
                    {
                        if (DataCenter.IsPvP)
                        {
                            Service.Config.PvPRotationChoice = r.FullName;
                        }
                        else
                        {
                            Service.Config.RotationChoice = r.FullName;
                        }
                    }
                    ImguiTooltips.HoveredTooltip(rAttr.Description);
                    ImGui.PopStyleColor();
                }
            }
        }

        var warning = gameVersion + rot.GameVersion;
        if (!rotation.IsValid) warning += "\n" + string.Format(UiString.ConfigWindow_Rotation_InvalidRotation
            .Local(),
                rotation.GetType().Assembly.GetInfo().Author);

        if (rotation.IsBeta()) warning += "\n" + UiString.ConfigWindow_Rotation_BetaRotation.Local();

        warning += "\n \n" + UiString.ConfigWindow_Helper_SwitchRotation.Local();
        ImguiTooltips.HoveredTooltip(warning);
    }

    #region About
    private static readonly SortedList<uint, string> CountStringPair = new()
    {
        { 100_000, UiString.ConfigWindow_About_Clicking100k.Local() },
        { 500_000, UiString.ConfigWindow_About_Clicking500k.Local() },
    };

    protected override void DrawAbout()
    {
        base.DrawAbout();

        var width = ImGui.GetWindowWidth();

        var clickingCount = OtherConfiguration.RotationSolverRecord.ClickingCount;
        if (clickingCount > 0)
        {
            using var color = ImRaii.PushColor(ImGuiCol.Text, new Vector4(0.2f, 0.6f, 0.95f, 1));
            var countStr = string.Format(UiString.ConfigWindow_About_ClickingCount.Local(),
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
        if (sayHelloCount > 0)
        {
            using var color = ImRaii.PushColor(ImGuiCol.Text, new Vector4(0.2f, 0.8f, 0.95f, 1));
            var countStr = string.Format(UiString.ConfigWindow_About_SayHelloCount.Local(), sayHelloCount);

            ImGuiHelper.DrawItemMiddle(() =>
            {
                ImGui.TextWrapped(countStr);
            }, width, ImGui.CalcTextSize(countStr).X);
        }

        {
            using var color = ImRaii.PushColor(ImGuiCol.Text, new Vector4(0.2f, 1f, 0.95f, 1));
            var countStr = string.Format(UiString.ConfigWindow_About_UserCount.Local(), DownloadHelper.UsersHash.Length);

            ImGuiHelper.DrawItemMiddle(() =>
            {
                ImGui.TextWrapped(countStr);
            }, width, ImGui.CalcTextSize(countStr).X);
        }

        {
            var isSupporter = DownloadHelper.IsSupporter;

            var notSupporter = isSupporter ? UiString.ThanksForSupporting.Local() : UiString.NotSupporterWarning.Local();

            ImGuiHelper.DrawItemMiddle(() =>
            {
                using var color = ImRaii.PushColor(ImGuiCol.Text, isSupporter ? ImGuiColors.DalamudOrange : ImGuiColors.DalamudRed);
                ImGui.TextWrapped(notSupporter);
            }, width, ImGui.CalcTextSize(notSupporter).X);
        }

        _aboutHeaders.Draw();
    }

    private static readonly CollapsingHeaderGroup _aboutHeaders = new(new()
    {
        { () => UiString.ConfigWindow_About_Macros.Local(), DrawAboutMacros},
        { () => UiString.ConfigWindow_About_Compatibility.Local(), DrawAboutCompatibility},
        { () => UiString.ConfigWindow_About_Supporters.Local(), DrawAboutSupporters},
        { () => UiString.ConfigWindow_About_Links.Local(), DrawAboutLinks},
    });

    private static void DrawAboutMacros()
    {
        using var style = ImRaii.PushStyle(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

        StateCommandType.Auto.DisplayCommandHelp(getHelp: i => i.Local());

        StateCommandType.Manual.DisplayCommandHelp(getHelp: i => i.Local());

        StateCommandType.Cancel.DisplayCommandHelp(getHelp: i => i.Local());

        OtherCommandType.NextAction.DisplayCommandHelp(getHelp: i => i.Local());

        ImGui.NewLine();

        SpecialCommandType.EndSpecial.DisplayCommandHelp(getHelp: i => i.Local());

        SpecialCommandType.HealArea.DisplayCommandHelp(getHelp: i => i.Local());

        SpecialCommandType.HealSingle.DisplayCommandHelp(getHelp: i => i.Local());

        SpecialCommandType.DefenseArea.DisplayCommandHelp(getHelp: i => i.Local());

        SpecialCommandType.DefenseSingle.DisplayCommandHelp(getHelp: i => i.Local());

        SpecialCommandType.MoveForward.DisplayCommandHelp(getHelp: i => i.Local());

        SpecialCommandType.MoveBack.DisplayCommandHelp(getHelp: i => i.Local());

        SpecialCommandType.Speed.DisplayCommandHelp(getHelp: i => i.Local());

        SpecialCommandType.DispelStancePositional.DisplayCommandHelp(getHelp: i => i.Local());

        SpecialCommandType.RaiseShirk.DisplayCommandHelp(getHelp: i => i.Local());

        SpecialCommandType.AntiKnockback.DisplayCommandHelp(getHelp: i => i.Local());

        SpecialCommandType.Burst.DisplayCommandHelp(getHelp: i => i.Local());

        SpecialCommandType.LimitBreak.DisplayCommandHelp(getHelp: i => i.Local());

        SpecialCommandType.NoCasting.DisplayCommandHelp(getHelp: i => i.Local());
    }

    private static void DrawAboutCompatibility()
    {
        ImGui.TextWrapped(UiString.ConfigWindow_About_Compatibility_Others.Local());

        ImGui.TextWrapped(UiString.ConfigWindow_About_Compatibility_Description.Local());

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

            foreach (var item in DownloadHelper.IncompatiblePlugins ?? [])
            {
                ImGui.TableNextRow();
                ImGui.TableNextColumn();

                ImGui.Text(item.Name);

                ImGui.TableNextColumn();

                var icon = item.Icon;
                if (string.IsNullOrEmpty(icon)) icon = "https://raw.githubusercontent.com/goatcorp/DalamudAssets/master/UIRes/defaultIcon.png";

                if (ImageLoader.GetTexture(icon, out var texture))
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
                    ImguiTooltips.HoveredTooltip(UiString.ConfigWindow_About_Compatibility_Mistake.Local());
                }
                if (item.Type.HasFlag(CompatibleType.Skill_Selection))
                {
                    ImGui.TextColored(ImGuiColors.DalamudOrange, CompatibleType.Skill_Selection.ToString().Replace('_', ' '));
                    ImguiTooltips.HoveredTooltip(UiString.ConfigWindow_About_Compatibility_Mislead.Local());
                }
                if (item.Type.HasFlag(CompatibleType.Crash))
                {
                    ImGui.TextColored(ImGuiColors.DalamudRed, CompatibleType.Crash.ToString().Replace('_', ' '));
                    ImguiTooltips.HoveredTooltip(UiString.ConfigWindow_About_Compatibility_Crash.Local());
                }
            }
        }
    }
    private static void DrawAboutSupporters()
    {
        ImGui.TextWrapped(UiString.ConfigWindow_About_ThanksToSupporters.Local());

        var width = ImGui.GetWindowWidth();
        using var font = ImRaii.PushFont(DrawingExtensions.GetFont(12));
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

        var text = "My story about FFXIV and Rotation Solver\n - ArchiTed / Youtube";
        var textWidth = ImGuiHelpers.GetButtonSize(text).X;
        ImGuiHelper.DrawItemMiddle(() =>
        {
            if (ImGui.Button(text))
            {
                Util.OpenLink("https://www.youtube.com/watch?v=Adigd5uqDx4");
            }
        }, width, textWidth);


        text = UiString.ConfigWindow_About_OpenConfigFolder.Local();
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

    private static Status[]? _allDispelStatus = null;
    internal static Status[] AllDispelStatus
        => _allDispelStatus ??= Service.GetSheet<Status>()
                    .Where(s => s.CanDispel)
                    .ToArray();


    private static Status[]? _allStatus = null;
    internal static Status[] AllStatus
        => _allStatus ??= Service.GetSheet<Status>()
                    .Where(s => !s.CanDispel && !s.LockMovement && !s.IsGaze && !s.IsFcBuff
                        && !string.IsNullOrEmpty(s.Name.ToString()) && s.Icon != 0)
                    .ToArray();

    private static GAction[]? _allActions = null;
    internal static GAction[] AllActions
        => _allActions ??= Service.GetSheet<GAction>()
                    .Where(a => !string.IsNullOrEmpty(a.Name) && !a.IsPvP && !a.IsPlayerAction
                    && a.ClassJob.Value == null && a.Cast100ms > 0)
                    .ToArray();
    private static Status[]? _badStatus = null;
    internal static Status[] BadStatus
        => _badStatus ??= Service.GetSheet<Status>()
                    .Where(s => s.StatusCategory == 2 && s.Icon != 0)
                    .ToArray();
    internal static void StatusPopUp(string popupId, Status[] allStatus, ref string searching, Action<Status> clicked, uint notLoadId = 10100, float size = 32)
    {
        using var popup = ImRaii.Popup(popupId);
        if (popup)
        {
            ImGui.SetNextItemWidth(200 * Scale);
            ImGui.InputTextWithHint("##Searching the status", UiString.ConfigWindow_List_StatusNameOrId.Local(), ref searching, 128);

            ImGui.Spacing();

            using var child = ImRaii.Child("Rotation Solver Add Status", new Vector2(-1, 400 * Scale));
            if (child)
            {
                var count = Math.Max(1, (int)MathF.Floor(ImGui.GetWindowWidth() / (size * 3 / 4 * Scale + ImGui.GetStyle().ItemSpacing.X)));
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
                        ImguiTooltips.HoveredTooltip($"{status.Name} ({status.RowId})");
                    }
                }
            }
        }
    }


    protected override ConfigWindowItem[] GetItems() =>
    [
        new RotationItem(),
        new ActionsItem(),
        new RotationsItem(),
        new BasicItem(),
        new UIItem(),
        new AutoItem(),
        new TargetItem(),
        new ListItem(),
        new TimelineConfigItem(),
        new TriggerItem(),
        new ExtraItem(),
        new DebugItem(),
        new ChangeLogItem(),
    ];
}