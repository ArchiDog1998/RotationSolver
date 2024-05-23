using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.Colors;
using Dalamud.Interface.GameFonts;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.ImGuiMethods;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Configuration;
using RotationSolver.Basic.Configuration.Condition;
using RotationSolver.Basic.Configuration.Trigger;
using RotationSolver.Basic.Record;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.UI.SearchableConfigs;
using RotationSolver.Updaters;
using System.ComponentModel;
using System.Diagnostics;
using XIVConfigUI;
using XIVConfigUI.Attributes;
using XIVConfigUI.SearchableConfigs;
using XIVDrawer;
using GAction = Lumina.Excel.GeneratedSheets.Action;
using TargetType = RotationSolver.Basic.Actions.TargetType;

namespace RotationSolver.UI;

public class RotationConfigWindow : ConfigWindow
{
    public override SearchableCollection Collection { get; } = new(Service.Config,
            new SearchableConfigRS());

    public static Vector3 HoveredPosition { get; private set; } = Vector3.Zero;

    protected override string Kofi => "B0B0IN5DX";

    protected override string Patreon => "archidog1998";
    protected override string Crowdin => "rotationsolver";

    protected override string DiscordServerID => "1228953752585637908";
    protected override string DiscordServerInviteLink => "9D4E8eZW5g";

    public override IEnumerable<Searchable> Searchables => [.. base.Searchables, ..DataCenter.RightNowRotation?.Configs];

    public RotationConfigWindow()
        : base(typeof(RotationConfigWindow).Assembly.GetName())
    {
        Size = new Vector2(740f, 490f);

        ImGuiHelper.GetFont(FontSize.Third, GameFontFamily.Axis);
        ImGuiHelper.GetFont(FontSize.Forth, GameFontFamily.Axis);
        ImGuiHelper.GetFont(FontSize.Fifth, GameFontFamily.Axis);
        ImGuiHelper.GetFont(FontSize.Forth, GameFontFamily.MiedingerMid);
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
                ImGuiHelper.DrawHotKeysPopup(key, string.Empty, (UiString.ConfigWindow_List_Remove.Local(), DeleteFile, ["Delete"]));

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
                    ImGui.PushStyleColor(ImGuiCol.Text, r.GetCustomAttribute<BetaRotationAttribute>() == null
                        ? ImGuiColors.DalamudWhite : ImGuiColors.DalamudOrange);
                    if (ImGui.Selectable(rAttr.Name))
                    {
                        if( DataCenter.IsPvP)
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

    public abstract class ConfigWindowItemRS : ConfigWindowItem
    {
        public abstract uint Icon { get; }

        public sealed override bool GetIcon(out IDalamudTextureWrap texture)
        {
            return ImageLoader.GetTexture(Icon, out texture);
        }
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

        if (!DownloadHelper.IsSupporter)
        {
            var notSupporter = UiString.NotSupporterWarning.Local();

            ImGuiHelper.DrawItemMiddle(() =>
            {
                using var color = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudRed);
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

    public class RotationItem : ConfigWindowItemRS
    {
        public override bool IsSkip => true;

        public override uint Icon => 4;


        private static readonly CollapsingHeaderGroup _rotationHeader = new(new()
        {
            { () => UiString.ConfigWindow_Rotation_Description.Local(), DrawRotationDescription },

            { GetRotationStatusHead,  DrawRotationStatus },

            { () => UiString.ConfigWindow_Rotation_Configuration.Local(), DrawRotationConfiguration },
            { () => UiString.ConfigWindow_Rotation_Rating.Local(), DrawRotationRating },

            { () => UiString.ConfigWindow_Rotation_Information.Local(), DrawRotationInformation },
        });

        public override void Draw(ConfigWindow window)
        {
            var rotation = DataCenter.RightNowRotation;
            if (rotation == null) return;

            var desc = rotation.Description;
            if (!string.IsNullOrEmpty(desc))
            {
                using var font = ImRaii.PushFont(DrawingExtensions.GetFont(15));
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
                if (ImageLoader.GetTexture("https://storage.ko-fi.com/cdn/brandasset/kofi_button_red.png", out var icon) && ImGuiHelper.TextureButton(icon, wholeWidth, 250 * Scale, "Ko-fi link"))
                {
                    Util.OpenLink(info.DonateLink);
                }
            }

            _rotationHeader.Draw();
        }

        private static void DrawRotationDescription()
        {
            var rotation = DataCenter.RightNowRotation;
            if (rotation == null) return;

            var wholeWidth = ImGui.GetWindowWidth();
            var type = rotation.GetType();

            var links = type.GetCustomAttributes<LinkDescriptionAttribute>();

            foreach (var link in links)
            {
                DrawLinkDescription(link.LinkDescription, wholeWidth, true);
            }
        }

        private static string GetRotationStatusHead()
        {
            var rotation = DataCenter.RightNowRotation;
            var status = UiString.ConfigWindow_Rotation_Status.Local();
            if (Service.Config.InDebug)
            {
                return status;
            }
            if (rotation == null || !rotation.ShowStatus) return string.Empty;
            return status;
        }

        private static void DrawRotationStatus()
        {
            DataCenter.RightNowRotation?.DisplayStatus();
        }

        private static void DrawRate(string name, string rate, int count, Vector4 btnColor, string popId)
        {
            using var grp = ImRaii.Group();

            using (var font = ImRaii.PushFont(ImGuiHelper.GetFont(FontSize.Third)))
            {
                ImGui.TextColored(ImGuiColors.DalamudWhite2, name);
            }

            ImGui.Spacing();

            using (var color = ImRaii.PushColor(ImGuiCol.Text, btnColor))
            {
                using var style = ImRaii.PushColor(ImGuiCol.Button, 0);
                ImGui.SetWindowFontScale(2f);
                if (ImGuiEx.IconButton(FontAwesomeIcon.Star, popId))
                {
                    ImGui.OpenPopup(popId);
                }
                ImGui.SetWindowFontScale(1);
            }

            ImGui.SameLine();

            using var gp = ImRaii.Group();
            using (var font = ImRaii.PushFont(ImGuiHelper.GetFont(FontSize.Forth, GameFontFamily.MiedingerMid)))
            {
                ImGui.Text(rate);
            }

            using (var font = ImRaii.PushFont(ImGuiHelper.GetFont(FontSize.Fifth, GameFontFamily.MiedingerMid)))
            {
                ImGui.SameLine();
                ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(-3, 3));
                ImGui.TextColored(ImGuiColors.DalamudWhite2, "/10");

                if (count != 0)
                {
                    ImGui.TextColored(ImGuiColors.DalamudWhite2, count.ToString("N0"));
                }
            }
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

            set.DrawItems(0);

            var duty = DataCenter.RightNowDutyRotation;

            if (duty == null) return;

            set = duty.Configs;

            if (set.Any()) ImGui.Separator();

            set.DrawItems(0);
        }

        private static DateTime _nextChangeTime = DateTime.MinValue;
        private static void DrawRotationRating()
        {
            var rotation = DataCenter.RightNowRotation;
            if (rotation == null) return;

            var ratings = DownloadHelper.GetRating(rotation.GetType());

            var rate = "??";

            if (ratings.Count > 0)
            {
                rate = ratings.Sum(i => Math.Min(Math.Max((byte)1, i.Value), (byte)10) / (double)ratings.Count).ToString("F1");
            }

            float width;
            using (var font = ImRaii.PushFont(ImGuiHelper.GetFont(FontSize.Third)))
            {
                width = ImGui.CalcTextSize("YOUR RATING").X;
            }

            var wholeWidth = ImGui.GetWindowWidth();
            ImGuiHelper.DrawItemMiddle(() =>
            {
                DrawRate("RS RATING", rate, ratings.Count, ImGuiColors.DalamudYellow, "Rotation Solver All Rating");

            }, wholeWidth, width);

            if (Player.Available)
            {
                rate = ratings.TryGetValue(Player.Object.EncryptString(), out var str) ? str.ToString("F1") : "??";

                ImGui.NewLine();
                ImGuiHelper.DrawItemMiddle(() =>
                {
                    DrawRate("YOUR RATING", rate, 0, ImGuiColors.TankBlue, "Rotation Solver Your Rating");
                }, wholeWidth, width);
            }

            using (var popup = ImRaii.Popup("Rotation Solver All Rating"))
            {
                if (popup.Success)
                {
                    var count = (float)ratings.Count;
                    foreach (var item in ratings.GroupBy(i => i.Value).OrderByDescending(g => g.Key))
                    {
                        using (var font = ImRaii.PushFont(ImGuiHelper.GetFont(FontSize.Fifth, GameFontFamily.MiedingerMid)))
                        {
                            ImGui.Text(item.Key.ToString());
                        }

                        ImGui.SameLine();

                        var c = item.Count();
                        var r = c / count;

                        using (var font = ImRaii.PushFont(ImGuiHelper.GetFont(FontSize.Sixth, GameFontFamily.MiedingerMid)))
                        {
                            ImGui.ProgressBar(r, new(400, 20), $"{r:P1}({c:N0})");
                        }
                    }
                }
            }

            using (var popup = ImRaii.Popup("Rotation Solver Your Rating"))
            {
                if (popup.Success)
                {
                    var time = _nextChangeTime - DateTime.Now;

                    if(time > TimeSpan.Zero)
                    {
                        using var font = ImRaii.PushFont(ImGuiHelper.GetFont(FontSize.Fifth, GameFontFamily.Axis));
                        ImGui.TextColored(ImGuiColors.DalamudRed, string.Format(UiString.Rotation_Rate.Local(), (int)time.TotalSeconds));
                    }
                    else
                    {
                        using var font = ImRaii.PushFont(ImGuiHelper.GetFont(FontSize.Forth, GameFontFamily.MiedingerMid));
                        for (byte i = 1; i < 11; i++)
                        {
                            if (ImGui.Button($"{i}##My Rating Value"))
                            {
                                GithubRecourcesHelper.ModifyYourRate(rotation.GetType(), i);
                                DownloadHelper.ModifyMyRate(i);
                                _nextChangeTime = DateTime.Now + TimeSpan.FromMinutes(1);
                                ImGui.CloseCurrentPopup();
                            }
                            ImGui.SameLine();
                        }
                    }
                }
            }

            ImGui.Separator();

            ImGui.TextWrapped(UiString.ConfigWindow_Rotation_Rating_Description.Local());

            if (DrawRating((float)rotation.AverageCountOfLastUsing, rotation.MaxCountOfLastUsing, 10))
            {
                ImguiTooltips.ShowTooltip(UiString.ConfigWindow_Rotation_Rating_CountOfLastUsing.Local());
            }
            if (DrawRating((float)rotation.AverageCountOfCombatTimeUsing, rotation.MaxCountOfCombatTimeUsing, 20))
            {
                ImguiTooltips.ShowTooltip(UiString.ConfigWindow_Rotation_Rating_CountOfCombatTimeUsing.Local());
            }

            static bool DrawRating(float value1, int value2, float max)
            {
                var ratio1 = value1 / max;
                var ratio2 = value2 / max;
                var count = RatingColors.Length;

                var start = ImGui.GetCursorPos() + ImGui.GetWindowPos();

                var spacing = ImGui.GetStyle().ItemSpacing;
                ImGui.GetStyle().ItemSpacing = Vector2.Zero;

                var size = Vector2.Zero;
                using (var font = ImRaii.PushFont(DrawingExtensions.GetFont(16)))
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

                var result = ImGuiHelperRS.IsInRect(start, new Vector2(ImGui.GetWindowSize().X, size.Y));
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
        }

        private static float _groupWidth = 100;
        private static void DrawRotationInformation()
        {
            var rotation = DataCenter.RightNowRotation;
            if (rotation == null) return;

            var youtubeLink = rotation.GetType().GetCustomAttribute<YoutubeLinkAttribute>()?.ID;

            var wholeWidth = ImGui.GetWindowWidth();
            if (!string.IsNullOrEmpty(youtubeLink))
            {
                ImGui.NewLine();
                if (ImageLoader.GetTexture("https://www.gstatic.com/youtube/img/branding/youtubelogo/svg/youtubelogo.svg", out var icon) && ImGuiHelper.TextureButton(icon, wholeWidth, 250 * Scale, "Youtube Link"))
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

                    if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(repository) && !string.IsNullOrEmpty(link.Path))
                    {
                        DrawGitHubBadge(userName, repository, link.Path, $"https://github.com/{userName}/{repository}/blob/{link.Path}", center: true);
                    }
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

        private static readonly uint[] RatingColors =
        [
            ImGui.ColorConvertFloat4ToU32(ImGuiColors.TankBlue),
            ImGui.ColorConvertFloat4ToU32(ImGuiColors.HealerGreen),
            ImGui.ColorConvertFloat4ToU32(ImGuiColors.DalamudYellow),
            ImGui.ColorConvertFloat4ToU32(ImGuiColors.DalamudOrange),
            ImGui.ColorConvertFloat4ToU32(ImGuiColors.DPSRed),
        ];

        internal static void DrawLinkDescription(LinkDescription link, float wholeWidth, bool drawQuestion)
        {
            var hasTexture = ImageLoader.GetTexture(link.Url, out var texture);

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
    }

    [Description("Actions")]
    public class ActionsItem : ConfigWindowItemRS
    {
        public override uint Icon => 4;

        public override string Description => UiString.Item_Actions.Local();

        public override unsafe void Draw(ConfigWindow window)
        {
            ImGui.TextWrapped(UiString.ConfigWindow_Actions_Description.Local());

            using var table = ImRaii.Table("Rotation Solver Actions", 2, ImGuiTableFlags.Resizable);

            if (!table) return;
            ImGui.TableSetupColumn("Action Column", ImGuiTableColumnFlags.WidthFixed, ImGui.GetWindowWidth() / 2);
            ImGui.TableNextColumn();

            if (_actionsList != null)
            {
                _actionsList.ClearCollapsingHeader();

                if (DataCenter.RightNowRotation != null && RotationUpdater.AllGroupedActions != null)
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
                                if (ImGuiHelper.NoPaddingNoColorImageButton(icon.ImGuiHandle, Vector2.One * size, item.Name + item.ID))
                                {
                                    _activeAction = item;
                                }
                                ImGuiHelper.DrawActionOverlay(cursor, size, _activeAction == item ? 1 : 0);

                                if (ImageLoader.GetTexture("ui/uld/readycheck_hr1.tex", out var texture))
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

            DrawConfigsOfAction();
            DrawActionDebug();

            ImGui.TextWrapped(UiString.ConfigWindow_Actions_ConditionDescription.Local());
            _sequencerList?.Draw();

            static void DrawConfigsOfAction()
            {
                if (_activeAction == null) return;

                var enable = _activeAction.IsEnabled;
                if (ImGui.Checkbox($"{_activeAction.Name}##{_activeAction.Name} Enabled", ref enable))
                {
                    _activeAction.IsEnabled = enable;
                }

                const string key = "Action Enable Popup";
                var cmd = ToCommandStr(OtherCommandType.ToggleActions, _activeAction.ToString()!);
                ImGuiHelper.DrawHotKeysPopup(key, cmd);
                ImGuiHelper.ExecuteHotKeysPopup(key, cmd, string.Empty, false);

                enable = _activeAction.IsInCD;
                if (ImGui.Checkbox($"{UiString.ConfigWindow_Actions_ShowOnCDWindow.Local()}##{_activeAction.Name}InCooldown", ref enable))
                {
                    _activeAction.IsInCD = enable;
                }

                if (_activeAction is IBaseAction a)
                {
                    DrawConfigsOfBaseAction(a);
                }

                ImGui.Separator();

                static void DrawConfigsOfBaseAction(IBaseAction a)
                {
                    var config = a.Config;

                    if (Service.Config.MistakeRatio > 0
                        && !a.Setting.IsFriendly
                        && a.Setting.TargetType != TargetType.Move)
                    {
                        var enable = config.IsInMistake;
                        if (ImGui.Checkbox($"{UiString.ConfigWindow_Actions_IsInMistake.Local()}##{a.Name}InMistake", ref enable))
                        {
                            config.IsInMistake = enable;
                        }
                    }

                    ImGui.Separator();

                    var ttk = config.TimeToKill;
                    ImGui.SetNextItemWidth(Scale * 150);
                    if (ImGui.DragFloat($"{UiString.ConfigWindow_Actions_TTK.Local()}##{a}",
                        ref ttk, 0.1f, 0, 120, $"{ttk:F2}{ConfigUnitType.Seconds.ToSymbol()}"))
                    {
                        config.TimeToKill = ttk;
                    }
                    ImguiTooltips.HoveredTooltip(ConfigUnitType.Seconds.Local());

                    var ttu = config.TimeToUntargetable;
                    ImGui.SetNextItemWidth(Scale * 150);
                    if (ImGui.DragFloat($"{UiString.ConfigWindow_Actions_TTU.Local()}##{a}",
                        ref ttu, 0.1f, 0, 120, $"{ttu:F2}{ConfigUnitType.Seconds.ToSymbol()}"))
                    {
                        config.TimeToUntargetable = ttu;
                    }
                    ImguiTooltips.HoveredTooltip(ConfigUnitType.Seconds.Local());

                    if (a.Setting.StatusProvide != null || a.Setting.TargetStatusProvide != null)
                    {
                        var shouldStatus = config.ShouldCheckStatus;
                        if (ImGui.Checkbox($"{UiString.ConfigWindow_Actions_CheckStatus.Local()}##{a}", ref shouldStatus))
                        {
                            config.ShouldCheckStatus = shouldStatus;
                        }

                        if (shouldStatus)
                        {
                            var statusGcdCount = (int)config.StatusGcdCount;
                            ImGui.SetNextItemWidth(Scale * 150);
                            if (ImGui.DragInt($"{UiString.ConfigWindow_Actions_GcdCount.Local()}##{a}",
                                ref statusGcdCount, 0.05f, 1, 10))
                            {
                                config.StatusGcdCount = (byte)statusGcdCount;
                            }
                        }
                    }

                    if (!a.TargetInfo.IsSingleTarget)
                    {
                        var aoeCount = (int)config.AoeCount;
                        ImGui.SetNextItemWidth(Scale * 150);
                        if (ImGui.DragInt($"{UiString.ConfigWindow_Actions_AoeCount.Local()}##{a}",
                            ref aoeCount, 0.05f, 1, 10))
                        {
                            config.AoeCount = (byte)aoeCount;
                        }
                    }

                    var ratio = config.AutoHealRatio;
                    ImGui.SetNextItemWidth(Scale * 150);
                    if (ImGui.DragFloat($"{UiString.ConfigWindow_Actions_HealRatio.Local()}##{a}",
                        ref ratio, 0.002f, 0, 1, $"{ratio * 100:F1}{ConfigUnitType.Percent.ToSymbol()}"))
                    {
                        config.AutoHealRatio = ratio;
                    }
                    ImguiTooltips.HoveredTooltip(ConfigUnitType.Percent.Local());

                }
            }

            static void DrawActionDebug()
            {
                if (!Service.Config.InDebug) return;

                if (_activeAction is IBaseAction action)
                {

                    try
                    {
                        ImGui.Text("ID: " + action.Info.ID.ToString());

#if DEBUG
                        ImGui.Text("Is Real GCD: " + action.Info.IsRealGCD.ToString());
                        ImGui.Text("Resources: " + FFXIVClientStructs.FFXIV.Client.Game.ActionManager.Instance()->CheckActionResources(FFXIVClientStructs.FFXIV.Client.Game.ActionType.Action, action.AdjustedID).ToString());
                        ImGui.Text("Status: " + FFXIVClientStructs.FFXIV.Client.Game.ActionManager.Instance()->GetActionStatus(FFXIVClientStructs.FFXIV.Client.Game.ActionType.Action, action.AdjustedID).ToString());
                        ImGui.Text("Cast Time: " + action.Info.CastTime.ToString());
                        ImGui.Text("MP: " + action.Info.MPNeed.ToString());
#endif
                        ImGui.Text("AnimationLock: " + action.Info.AnimationLockTime.ToString());
                        ImGui.Text("AttackType: " + action.Info.AttackType.ToString());
                        ImGui.Text("Aspect: " + action.Info.Aspect.ToString());
                        ImGui.Text("Has One:" + action.CD.HasOneCharge.ToString());
                        ImGui.Text("Recast One: " + action.CD.RecastTimeOneChargeRaw.ToString());
                        ImGui.Text("Recast Elapsed: " + action.CD.RecastTimeElapsedRaw.ToString());
                        ImGui.Text($"Charges: {action.CD.CurrentCharges} / {action.CD.MaxCharges}");

                        ImGui.Text($"Can Use: {action.CanUse(out _, skipClippingCheck: true)} ");
                        ImGui.Text($"Why Can't: {action.WhyCant.Local()} ");
                        ImGui.Text("IgnoreCastCheck:" + action.CanUse(out _, skipClippingCheck: true, skipCastingCheck: true).ToString());
                        ImGui.Text($"Why Can't: {action.WhyCant.Local()} ");
                        ImGui.Text("Target Name: " + action.Target.Target?.Name ?? string.Empty);
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

                        if (item is HpPotionItem healPotionItem)
                        {
                            ImGui.Text("MaxHP:" + healPotionItem.MaxHp.ToString());
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }

        private static IAction? _activeAction;

        private static readonly CollapsingHeaderGroup _actionsList = new()
        {
            HeaderSize = FontSize.Forth,
        };

        private static readonly CollapsingHeaderGroup _sequencerList = new(new()
        {
            { () => UiString.ConfigWindow_Actions_ForcedConditionSet.Local(), () =>
            {
                ImGui.TextWrapped(UiString.ConfigWindow_Actions_ForcedConditionSet_Description.Local());

                var rotation = DataCenter.RightNowRotation;
                var set = DataCenter.RightSet;

                if (set == null || _activeAction == null || rotation == null) return;
                set.GetCondition(_activeAction.ID)?.DrawMain(rotation);
            } },

            { () => UiString.ConfigWindow_Actions_DisabledConditionSet.Local(), () =>
            {
                ImGui.TextWrapped(UiString.ConfigWindow_Actions_DisabledConditionSet_Description.Local());

                var rotation = DataCenter.RightNowRotation;
                var set = DataCenter.RightSet;

                if (set == null || _activeAction == null || rotation == null) return;
                set.GetDisabledCondition(_activeAction.ID)?.DrawMain(rotation);
            } },
        })
        {
            HeaderSize = FontSize.Forth,
        };
    }

    [Description("Rotations")]
    public class RotationsItem : ConfigWindowItemRS
    {
        private CollapsingHeaderGroup? _rotationsHeader;

        public override uint Icon => 47;

        public override string Description => UiString.Item_Rotations.Local();

        public override void Draw(ConfigWindow window)
        {
            var width = ImGui.GetWindowWidth();

            var text = UiString.ConfigWindow_Rotations_Download.Local();
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

            _rotationsHeader ??= new(new()
            {
                {  () => UiString.ConfigWindow_Rotations_Settings.Local(), () => DrawRotationsSettings(window)},
                {  () => UiString.ConfigWindow_Rotations_Loaded.Local(), DrawRotationsLoaded},
                {  () => UiString.ConfigWindow_Rotations_GitHub.Local(), DrawRotationsGitHub},
                {  () => UiString.ConfigWindow_Rotations_Libraries.Local(), DrawRotationsLibraries},
            });

            _rotationsHeader?.Draw();
        }

        private static void DrawRotationsSettings(ConfigWindow window)
        {
            window.Collection.DrawItems((int)UiString.ConfigWindow_Rotations_Settings);
        }

        private static void DrawRotationsLoaded()
        {
            var assemblyGrps = RotationUpdater.CustomRotationsDict
                .SelectMany(d => d.Value)
                .SelectMany(g => g.Rotations)
                .GroupBy(r => r.Assembly);

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
                    foreach (var jobs in grp.GroupBy(r => r.GetCustomAttribute<JobsAttribute>()!.Jobs[0]).OrderBy(g => Svc.Data.GetExcelSheet<ClassJob>()!.GetRow((uint)g.Key)!.GetJobRole()))
                    {
                        var role = Svc.Data.GetExcelSheet<ClassJob>()!.GetRow((uint)jobs.Key)!.GetJobRole();
                        if (lastRole == role && lastRole != JobRole.None) ImGui.SameLine();
                        lastRole = role;

                        if (ImageLoader.GetTexture(IconSet.GetJobIcon(jobs.Key, IconType.Framed), out var texture, 62574))
                            ImGui.Image(texture.ImGuiHandle, Vector2.One * 30 * Scale);

                        ImguiTooltips.HoveredTooltip(string.Join('\n', jobs.Select(t => t.GetCustomAttribute<UIAttribute>()?.Name ?? t.Name)));
                    }

                    ImGui.TableNextColumn();

                    if (!string.IsNullOrEmpty(info.GitHubUserName) && !string.IsNullOrEmpty(info.GitHubRepository) && !string.IsNullOrEmpty(info.FilePath))
                    {
                        DrawGitHubBadge(info.GitHubUserName, info.GitHubRepository, info.FilePath);
                    }

                    if (!string.IsNullOrEmpty(info.DonateLink)
                        && ImageLoader.GetTexture("https://storage.ko-fi.com/cdn/brandasset/kofi_button_red.png", out var icon)
                        && ImGuiHelper.NoPaddingNoColorImageButton(icon.ImGuiHandle, new Vector2(1, (float)icon.Height / icon.Width) * MathF.Min(250, icon.Width) * Scale, info.FilePath ?? string.Empty))
                    {
                        Util.OpenLink(info.DonateLink);
                    }
                }
            }
        }

        private static void DrawRotationsGitHub()
        {
            if (!Service.Config.GitHubLibs.Any(s => string.IsNullOrEmpty(s) || s == "||"))
            {
                Service.Config.GitHubLibs = [.. Service.Config.GitHubLibs, "||"];
            }

            ImGui.Spacing();

            foreach (var gitHubLink in DownloadHelper.LinkLibraries ?? [])
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
            for (int i = 0; i < Service.Config.GitHubLibs.Length; i++)
            {
                var strs = Service.Config.GitHubLibs[i].Split('|');
                var userName = strs.FirstOrDefault() ?? string.Empty;
                var repository = strs.Length > 1 ? strs[1] : string.Empty;
                var fileName = strs.LastOrDefault() ?? string.Empty;

                DrawGitHubBadge(userName, repository, fileName, center: true);

                var changed = false;

                var width = ImGui.GetWindowWidth() - ImGuiEx.CalcIconSize(FontAwesomeIcon.Ban).X - ImGui.GetStyle().ItemSpacing.X * 3 - 10 * Scale;
                width /= 3;

                ImGui.SetNextItemWidth(width);
                changed |= ImGui.InputTextWithHint($"##GitHubLib{i}UserName", UiString.ConfigWindow_Rotations_UserName.Local(), ref userName, 1024);
                ImGui.SameLine();

                ImGui.SetNextItemWidth(width);
                changed |= ImGui.InputTextWithHint($"##GitHubLib{i}Repository", UiString.ConfigWindow_Rotations_Repository.Local(), ref repository, 1024);
                ImGui.SameLine();

                ImGui.SetNextItemWidth(width);
                changed |= ImGui.InputTextWithHint($"##GitHubLib{i}FileName", UiString.ConfigWindow_Rotations_FileName.Local(), ref fileName, 1024);
                ImGui.SameLine();

                if (changed)
                {
                    Service.Config.GitHubLibs[i] = $"{userName}|{repository}|{fileName}";
                }

                if (ImGuiEx.IconButton(FontAwesomeIcon.Ban, $"##Rotation Solver Remove GitHubLibs{i}"))
                {
                    removeIndex = i;
                }
            }
            if (removeIndex > -1)
            {
                var list = Service.Config.GitHubLibs.ToList();
                list.RemoveAt(removeIndex);
                Service.Config.GitHubLibs = [.. list];
            }
        }

        private static void DrawRotationsLibraries()
        {
            if (!Service.Config.OtherLibs.Any(string.IsNullOrEmpty))
            {
                Service.Config.OtherLibs = [.. Service.Config.OtherLibs, string.Empty];
            }

            ImGui.Spacing();

            var width = ImGui.GetWindowWidth() - ImGuiEx.CalcIconSize(FontAwesomeIcon.Ban).X - ImGui.GetStyle().ItemSpacing.X - 10 * Scale;

            int removeIndex = -1;
            for (int i = 0; i < Service.Config.OtherLibs.Length; i++)
            {
                ImGui.SetNextItemWidth(width);
                ImGui.InputTextWithHint($"##Rotation Solver OtherLib{i}", UiString.ConfigWindow_Rotations_Library.Local(), ref Service.Config.OtherLibs[i], 1024);
                ImGui.SameLine();

                if (ImGuiEx.IconButton(FontAwesomeIcon.Ban, $"##Rotation Solver Remove OtherLibs{i}"))
                {
                    removeIndex = i;
                }
            }
            if (removeIndex > -1)
            {
                var list = Service.Config.OtherLibs.ToList();
                list.RemoveAt(removeIndex);
                Service.Config.OtherLibs = [.. list];
            }
        }
    }

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

    [Description("Basic")]
    public class BasicItem : ConfigWindowItemRS
    {
        public override uint Icon => 14;
        public override string Description => UiString.Item_Basic.Local();
        private CollapsingHeaderGroup? _baseHeader;
        public override void Draw(ConfigWindow window)
        {
            _baseHeader ??= new(new()
            {
                { () => UiString.ConfigWindow_Basic_Timer.Local(), () => DrawBasicTimer(window) },
                { () => UiString.ConfigWindow_Basic_AutoSwitch.Local(), () => DrawBasicAutoSwitch(window) },
                { () => UiString.ConfigWindow_Basic_NamedConditions.Local(), DrawBasicNamedConditions },
                { () => UiString.ConfigWindow_Basic_Others.Local(), () => DrawBasicOthers(window) },
            });
            _baseHeader?.Draw();
        }

        private static readonly uint PING_COLOR = ImGui.ColorConvertFloat4ToU32(ImGuiColors.ParsedGreen);
        private static readonly uint LOCK_TIME_COLOR = ImGui.ColorConvertFloat4ToU32(ImGuiColors.ParsedBlue);
        private static readonly uint WEAPON_DELAY_COLOR = ImGui.ColorConvertFloat4ToU32(ImGuiColors.ParsedGold);
        private static readonly uint IDEAL_CLICK_TIME_COLOR = ImGui.ColorConvertFloat4ToU32(new Vector4(0.8f, 0f, 0f, 1f));
        private static readonly uint CLICK_TIME_COLOR = ImGui.ColorConvertFloat4ToU32(ImGuiColors.ParsedPink);
        private static readonly uint ADVANCE_TIME_COLOR = ImGui.ColorConvertFloat4ToU32(ImGuiColors.DalamudYellow);
        private static readonly uint ADVANCE_ABILITY_TIME_COLOR = ImGui.ColorConvertFloat4ToU32(ImGuiColors.ParsedOrange);
        const float gcdSize = 50, ogcdSize = 40, pingHeight = 12, spacingHeight = 8;

        private static void AddPingLockTime(ImDrawListPtr drawList, Vector2 lineStart, float sizePerTime, float ping, float animationLockTime, float advanceTime, uint color, float clickTime)
        {
            var size = new Vector2(ping * sizePerTime, pingHeight);
            drawList.AddRectFilled(lineStart, lineStart + size, ChangeAlpha(PING_COLOR));
            if (ImGuiHelperRS.IsInRect(lineStart, size))
            {
                ImguiTooltips.ShowTooltip(UiString.ConfigWindow_Basic_Ping.Local());
            }

            var rectStart = lineStart + new Vector2(ping * sizePerTime, 0);
            size = new Vector2(animationLockTime * sizePerTime, pingHeight);
            drawList.AddRectFilled(rectStart, rectStart + size, ChangeAlpha(LOCK_TIME_COLOR));
            if (ImGuiHelperRS.IsInRect(rectStart, size))
            {
                ImguiTooltips.ShowTooltip(UiString.ConfigWindow_Basic_AnimationLockTime.Local());
            }

            drawList.AddLine(lineStart - new Vector2(0, spacingHeight), lineStart + new Vector2(0, pingHeight * 2 + spacingHeight / 2), IDEAL_CLICK_TIME_COLOR, 1.5f);

            rectStart = lineStart + new Vector2(-advanceTime * sizePerTime, pingHeight);
            size = new Vector2(advanceTime * sizePerTime, pingHeight);
            drawList.AddRectFilled(rectStart, rectStart + size, ChangeAlpha(color));
            if (ImGuiHelperRS.IsInRect(rectStart, size))
            {
                ImguiTooltips.ShowTooltip(() =>
                {
                    ImGui.TextWrapped(UiString.ConfigWindow_Basic_ClickingDuration.Local());

                    ImGui.Separator();

                    ImGui.TextColored(ImGui.ColorConvertU32ToFloat4(IDEAL_CLICK_TIME_COLOR),
                        UiString.ConfigWindow_Basic_IdealClickingTime.Local());

                    ImGui.TextColored(ImGui.ColorConvertU32ToFloat4(CLICK_TIME_COLOR),
                        UiString.ConfigWindow_Basic_RealClickingTime.Local());
                });
            }

            float time = 0;
            while (time < advanceTime)
            {
                var start = lineStart + new Vector2((time - advanceTime) * sizePerTime, 0);
                drawList.AddLine(start + new Vector2(0, pingHeight), start + new Vector2(0, pingHeight * 2 + spacingHeight), CLICK_TIME_COLOR, 2.5f);

                time += clickTime;
            }
        }
        private static void DrawBasicTimer(ConfigWindow window)
        {
            var gcdTime = DataCenter.WeaponTotal;
            if (gcdTime == 0) gcdTime = 2.5f;
            var wholeWidth = ImGui.GetWindowWidth();
            var ping = DataCenter.Ping;

            ImGui.PushFont(DrawingExtensions.GetFont(14));
            ImGui.PushStyleColor(ImGuiCol.Text, ImGui.ColorConvertFloat4ToU32(ImGuiColors.DalamudYellow));
            var infoText = $"GCD: {gcdTime:F2}s Ping: {ping:F2}s";
            var infoSize = ImGui.CalcTextSize(infoText);

            ImGuiHelper.DrawItemMiddle(() =>
            {
                ImGui.Text(infoText);
            }, wholeWidth, infoSize.X);
            ImGui.PopStyleColor();
            ImGui.PopFont();

            var actionAhead = Service.Config.ActionAhead;
            var minAbilityAhead = Service.Config.MinLastAbilityAdvanced;
            var animationLockTime = DataCenter.MinAnimationLock;
            var weaponDelay = (Service.Config.WeaponDelay.X + Service.Config.WeaponDelay.Y) / 2;
            var clickingDelay = (Service.Config.ClickingDelay.X + Service.Config.ClickingDelay.Y) / 2;

            var drawList = ImGui.GetWindowDrawList();
            ImGui.Spacing();
            var startCursorPt = ImGui.GetCursorPos();
            var windowsPos = ImGui.GetWindowPos();

            var sizePerTime = (wholeWidth - gcdSize) / (gcdTime + weaponDelay + actionAhead);

            var lineStart = windowsPos + startCursorPt + new Vector2(sizePerTime * actionAhead, gcdSize + spacingHeight);
            ImGuiHelper.DrawActionOverlay(startCursorPt + new Vector2(sizePerTime * actionAhead, 0), gcdSize, 0);
            ImGuiHelper.DrawActionOverlay(startCursorPt + new Vector2(wholeWidth - gcdSize, 0), gcdSize, 0);

            AddPingLockTime(drawList, lineStart, sizePerTime, ping, animationLockTime, actionAhead, ADVANCE_TIME_COLOR, clickingDelay);
            var start = lineStart + new Vector2(gcdTime * sizePerTime, 0);
            var rectSize = new Vector2(weaponDelay * sizePerTime, pingHeight);
            drawList.AddRectFilled(start, start + rectSize, WEAPON_DELAY_COLOR);
            drawList.AddRect(start, start + rectSize, uint.MaxValue, 0, ImDrawFlags.Closed, 2);
            if (ImGuiHelperRS.IsInRect(start, rectSize))
            {
                ImguiTooltips.ShowTooltip(typeof(Configs).GetProperty(nameof(Configs.WeaponDelay))!.Local());
            }
            drawList.AddLine(lineStart + new Vector2((gcdTime + weaponDelay) * sizePerTime, -spacingHeight), lineStart + new Vector2((gcdTime + weaponDelay) * sizePerTime,
                pingHeight * 2 + spacingHeight), IDEAL_CLICK_TIME_COLOR, 2);

            ImGui.PushFont(DrawingExtensions.GetFont(20));
            const string gcdText = "GCD";
            var size = ImGui.CalcTextSize(gcdText);
            ImGui.SetCursorPos(startCursorPt + new Vector2(sizePerTime * actionAhead + (gcdSize - size.X) / 2, (gcdSize - size.Y) / 2));
            ImGui.Text(gcdText);
            ImGui.SetCursorPos(startCursorPt + new Vector2(wholeWidth - gcdSize + (gcdSize - size.X) / 2, (gcdSize - size.Y) / 2));
            ImGui.Text(gcdText);
            ImGui.PopFont();

            ImGui.PushFont(DrawingExtensions.GetFont(14));
            const string ogcdText = "Off-\nGCD";
            size = ImGui.CalcTextSize(ogcdText);
            ImGui.PopFont();

            var timeStep = ping + animationLockTime;
            var time = timeStep;
            while (time < gcdTime - timeStep)
            {
                var isLast = time + 2 * timeStep > gcdTime;
                if (isLast)
                {
                    time = gcdTime - timeStep;
                }

                ImGuiHelper.DrawActionOverlay(startCursorPt + new Vector2(sizePerTime * (actionAhead + time), 0), ogcdSize, 0);
                ImGui.SetCursorPos(startCursorPt + new Vector2(sizePerTime * (actionAhead + time) + (ogcdSize - size.X) / 2, (ogcdSize - size.Y) / 2));

                ImGui.PushFont(DrawingExtensions.GetFont(14));
                ImGui.Text(ogcdText);
                ImGui.PopFont();

                var ogcdStart = lineStart + new Vector2(time * sizePerTime, 0);
                AddPingLockTime(drawList, ogcdStart, sizePerTime, ping, animationLockTime,
                    isLast ? MathF.Max(minAbilityAhead, actionAhead) : actionAhead, isLast ? ADVANCE_ABILITY_TIME_COLOR : ADVANCE_TIME_COLOR, clickingDelay);

                time += timeStep;
            }

            ImGui.SetCursorPosY(startCursorPt.Y + gcdSize + pingHeight * 2 + 2 * spacingHeight + ImGui.GetStyle().ItemSpacing.Y);

            ImGui.Spacing();

            window.Collection.DrawItems((int)UiString.ConfigWindow_Basic_Timer);
        }

        private static readonly CollapsingHeaderGroup _autoSwitch = new(new()
        {
            {
                () => UiString.ConfigWindow_Basic_SwitchCancelConditionSet.Local(),
                () => DataCenter.RightSet.SwitchCancelConditionSet?.DrawMain(DataCenter.RightNowRotation)
            },
            {
                () => UiString.ConfigWindow_Basic_SwitchManualConditionSet.Local(),
                () => DataCenter.RightSet.SwitchManualConditionSet?.DrawMain(DataCenter.RightNowRotation)
            },
            {
               () =>  UiString.ConfigWindow_Basic_SwitchAutoConditionSet.Local(),
                () => DataCenter.RightSet.SwitchAutoConditionSet?.DrawMain(DataCenter.RightNowRotation)
            },
        })
        {
            HeaderSize = FontSize.Forth,
        };
        private static void DrawBasicAutoSwitch(ConfigWindow window)
        {
            window.Collection.DrawItems((int)UiString.ConfigWindow_Basic_AutoSwitch);
            _autoSwitch?.Draw();
        }

        private static readonly Dictionary<int, bool> _isOpen = [];
        private static void DrawBasicNamedConditions()
        {
            if (!DataCenter.RightSet.NamedConditions.Any(c => string.IsNullOrEmpty(c.Name)))
            {
                DataCenter.RightSet.NamedConditions = [.. DataCenter.RightSet.NamedConditions, (string.Empty, new ConditionSet())];
            }

            ImGui.Spacing();

            int removeIndex = -1;
            for (int i = 0; i < DataCenter.RightSet.NamedConditions.Length; i++)
            {
                var value = _isOpen.TryGetValue(i, out var open) && open;

                var toggle = value ? FontAwesomeIcon.ArrowUp : FontAwesomeIcon.ArrowDown;
                var width = ImGui.GetWindowWidth() - ImGuiEx.CalcIconSize(FontAwesomeIcon.Ban).X
                    - ImGuiEx.CalcIconSize(toggle).X - ImGui.GetStyle().ItemSpacing.X * 2 - 20 * Scale;

                ImGui.SetNextItemWidth(width);
                ImGui.InputTextWithHint($"##Rotation Solver Named Condition{i}", UiString.ConfigWindow_Condition_ConditionName.Local(),
                    ref DataCenter.RightSet.NamedConditions[i].Name, 1024);

                ImGui.SameLine();

                if (ImGuiEx.IconButton(toggle, $"##Rotation Solver Toggle Named Condition{i}"))
                {
                    _isOpen[i] = value = !value;
                }

                ImGui.SameLine();

                if (ImGuiEx.IconButton(FontAwesomeIcon.Ban, $"##Rotation Solver Remove Named Condition{i}"))
                {
                    removeIndex = i;
                }

                if (value && DataCenter.RightNowRotation != null)
                {
                    DataCenter.RightSet.NamedConditions[i].Condition?.DrawMain(DataCenter.RightNowRotation);
                }
            }
            if (removeIndex > -1)
            {
                var list = DataCenter.RightSet.NamedConditions.ToList();
                list.RemoveAt(removeIndex);
                DataCenter.RightSet.NamedConditions = [.. list];
            }
        }

        private static void DrawBasicOthers(ConfigWindow window)
        {
            window.Collection.DrawItems((int)UiString.ConfigWindow_Basic_Others);

            if (Service.Config.UseAdditionalConditions && !DownloadHelper.IsSupporter)
            {
                ImGui.TextColored(ImGuiColors.DalamudRed, UiString.CantUseConditionBoolean.Local());
            }
        }
    }

    [Description("UI")]
    public class UIItem : ConfigWindowItemRS
    {
        private CollapsingHeaderGroup? _UIHeader;

        public override uint Icon => 42;
        public override string Description => UiString.Item_UI.Local();

        public override void Draw(ConfigWindow window)
        {
            _UIHeader ??= window.Collection.GetGroups<UiString>([
                    UiString.ConfigWindow_UI_Information,
                    UiString.ConfigWindow_UI_Overlay,
                    UiString.ConfigWindow_UI_Windows,
                ]);
            _UIHeader?.Draw();
        }
    }

    [Description("Auto")]
    public class AutoItem : ConfigWindowItemRS
    {
        private CollapsingHeaderGroup? _autoHeader;
        public override uint Icon => 29;
        public override string Description => UiString.Item_Auto.Local();

        public override void Draw(ConfigWindow window)
        {
            ImGui.TextWrapped(UiString.ConfigWindow_Auto_Description.Local());
            _autoHeader ??= new(new()
            {
                {  () => UiString.ConfigWindow_Auto_ActionUsage.Local(), () =>
                    {
                        ImGui.TextWrapped(UiString.ConfigWindow_Auto_ActionUsage_Description
                            .Local());
                        ImGui.Separator();

                        window.Collection.DrawItems((int)UiString.ConfigWindow_Auto_ActionUsage);
                    }
                },
                {  () => UiString.ConfigWindow_Auto_ActionCondition.Local(), () => DrawAutoActionCondition(window) },
                {  () => UiString.ConfigWindow_Auto_StateCondition.Local(), () => _autoState?.Draw() },
            });

            _autoHeader?.Draw();
        }
     
        private static readonly CollapsingHeaderGroup _autoState = new(new()
        {
            {
                () => UiString.ConfigWindow_Auto_HealAreaConditionSet.Local(),
                () => DataCenter.RightSet.HealAreaConditionSet?.DrawMain(DataCenter.RightNowRotation)
            },

            {
                () => UiString.ConfigWindow_Auto_HealSingleConditionSet.Local(),
                () => DataCenter.RightSet.HealSingleConditionSet?.DrawMain(DataCenter.RightNowRotation)
            },

            {
                () => UiString.ConfigWindow_Auto_DefenseAreaConditionSet.Local(),
                () => DataCenter.RightSet.DefenseAreaConditionSet?.DrawMain(DataCenter.RightNowRotation)
            },

            {
                () => UiString.ConfigWindow_Auto_DefenseSingleConditionSet.Local(),
                () => DataCenter.RightSet.DefenseSingleConditionSet?.DrawMain(DataCenter.RightNowRotation)
            },

            {
                () =>  UiString.ConfigWindow_Auto_DispelStancePositionalConditionSet.Local(),
                () => DataCenter.RightSet.DispelStancePositionalConditionSet?.DrawMain(DataCenter.RightNowRotation)
            },

            {
                () =>  UiString.ConfigWindow_Auto_RaiseShirkConditionSet.Local(),
                () => DataCenter.RightSet.RaiseShirkConditionSet?.DrawMain(DataCenter.RightNowRotation)
            },

            {
                () => UiString.ConfigWindow_Auto_MoveForwardConditionSet.Local(),
                () => DataCenter.RightSet.MoveForwardConditionSet?.DrawMain(DataCenter.RightNowRotation)
            },

            {
                () => UiString.ConfigWindow_Auto_MoveBackConditionSet.Local(),
                () => DataCenter.RightSet.MoveBackConditionSet?.DrawMain(DataCenter.RightNowRotation)
            },

            {
                () => UiString.ConfigWindow_Auto_AntiKnockbackConditionSet.Local(),
                () => DataCenter.RightSet.AntiKnockbackConditionSet?.DrawMain(DataCenter.RightNowRotation)
            },

            {
                () => UiString.ConfigWindow_Auto_SpeedConditionSet.Local(),
                () => DataCenter.RightSet.SpeedConditionSet?.DrawMain(DataCenter.RightNowRotation)
            },

            {
                () => UiString.ConfigWindow_Auto_LimitBreakConditionSet.Local(),
                () => DataCenter.RightSet.LimitBreakConditionSet?.DrawMain(DataCenter.RightNowRotation)
            },
        })
        {
            HeaderSize = FontSize.Forth,
        };

        private static void DrawAutoActionCondition(ConfigWindow window)
        {
            ImGui.TextWrapped(UiString.ConfigWindow_Auto_ActionCondition_Description.Local());
            ImGui.Separator();

            window.Collection.DrawItems((int)UiString.ConfigWindow_Auto_ActionCondition);
        }
    }

    [Description("Target")]
    public class TargetItem : ConfigWindowItemRS
    {
        private CollapsingHeaderGroup? _targetHeader;
        public override uint Icon => 16;
        public override string Description => UiString.Item_Target.Local();

        public override void Draw(ConfigWindow window)
        {
            _targetHeader ??= new(new()
            {
                {  () =>UiString.ConfigWindow_Target_Config.Local(), () => DrawTargetConfig(window) },
                {  () =>UiString.ConfigWindow_List_Hostile.Local(), DrawTargetHostile },
            });
            _targetHeader?.Draw();
        }

        private static void DrawTargetConfig(ConfigWindow window)
        {
            window.Collection.DrawItems((int)UiString.ConfigWindow_Target_Config);
        }

        private static void DrawTargetHostile()
        {
            if (ImGuiEx.IconButton(FontAwesomeIcon.Plus, "Add Hostile"))
            {
                Service.Config.TargetingTypes.Add(TargetingType.Big);
            }
            ImGui.SameLine();
            ImGui.TextWrapped(UiString.ConfigWindow_Param_HostileDesc.Local());

            for (int i = 0; i < Service.Config.TargetingTypes.Count; i++)
            {
                var targetType = Service.Config.TargetingTypes[i];

                void Delete()
                {
                    Service.Config.TargetingTypes.RemoveAt(i);
                };

                void Up()
                {
                    Service.Config.TargetingTypes.RemoveAt(i);
                    Service.Config.TargetingTypes.Insert(Math.Max(0, i - 1), targetType);
                };
                void Down()
                {
                    Service.Config.TargetingTypes.RemoveAt(i);
                    Service.Config.TargetingTypes.Insert(Math.Min(Service.Config.TargetingTypes.Count - 1, i + 1), targetType);
                }

                var key = $"Targeting Type Pop Up: {i}";

                ImGuiHelper.DrawHotKeysPopup(key, string.Empty,
                    (UiString.ConfigWindow_List_Remove.Local(), Delete, ["Delete"]),
                    (UiString.ConfigWindow_Actions_MoveUp.Local(), Up, ["↑"]),
                    (UiString.ConfigWindow_Actions_MoveDown.Local(), Down, ["↓"]));

                var names = Enum.GetNames(typeof(TargetingType));
                var targingType = (int)Service.Config.TargetingTypes[i];
                var text = UiString.ConfigWindow_Param_HostileCondition.Local();
                ImGui.SetNextItemWidth(ImGui.CalcTextSize(text).X + 30 * Scale);
                if (ImGui.Combo(text + "##HostileCondition" + i.ToString(), ref targingType, names, names.Length))
                {
                    Service.Config.TargetingTypes[i] = (TargetingType)targingType;
                }

                ImGuiHelper.ExecuteHotKeysPopup(key, string.Empty, string.Empty, true,
                    (Delete, [VirtualKey.DELETE]),
                    (Up, [VirtualKey.UP]),
                    (Down, [VirtualKey.DOWN]));
            }
        }
    }

    [Description("List")]
    public class ListItem : ConfigWindowItemRS
    {
        private CollapsingHeaderGroup? _idsHeader;

        public override uint Icon => 21;
        public override string Description => UiString.Item_List.Local();

        public override void Draw(ConfigWindow window)
        {
            ImGui.TextWrapped(UiString.ConfigWindow_List_Description.Local());

            _idsHeader ??= new(new()
            {
                { () => UiString.ConfigWindow_List_Statuses.Local(), DrawListStatuses},
                { () => Service.Config.UseDefenseAbility ? UiString.ConfigWindow_List_Actions.Local() : string.Empty,
                    () => DrawListActions(window)},
                {() =>  UiString.ConfigWindow_List_Territories.Local(), DrawListTerritories},
            });
            _idsHeader?.Draw();
        }

        #region List

        private static void DrawListStatuses()
        {
            ImGui.SetNextItemWidth(ImGui.GetWindowWidth());
            ImGui.InputTextWithHint("##Searching the action", UiString.ConfigWindow_List_StatusNameOrId.Local(), ref _statusSearching, 128);

            using var table = ImRaii.Table("Rotation Solver List Statuses", 4, ImGuiTableFlags.BordersInner | ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingStretchSame);
            if (table)
            {
                ImGui.TableSetupScrollFreeze(0, 1);
                ImGui.TableNextRow(ImGuiTableRowFlags.Headers);

                ImGui.TableNextColumn();
                ImGui.TableHeader(UiString.ConfigWindow_List_Invincibility.Local());

                ImGui.TableNextColumn();
                ImGui.TableHeader(UiString.ConfigWindow_List_Priority.Local());

                ImGui.TableNextColumn();
                ImGui.TableHeader(UiString.ConfigWindow_List_DangerousStatus.Local());

                ImGui.TableNextColumn();
                ImGui.TableHeader(UiString.ConfigWindow_List_NoCastingStatus.Local());

                ImGui.TableNextRow();

                ImGui.TableNextColumn();

                ImGui.TextWrapped(UiString.ConfigWindow_List_InvincibilityDesc.Local());
                DrawStatusList(nameof(OtherConfiguration.InvincibleStatus), OtherConfiguration.InvincibleStatus, AllStatus);

                ImGui.TableNextColumn();

                ImGui.TextWrapped(UiString.ConfigWindow_List_PriorityDesc.Local());
                DrawStatusList(nameof(OtherConfiguration.PriorityStatus), OtherConfiguration.PriorityStatus, AllStatus);

                ImGui.TableNextColumn();

                ImGui.TextWrapped(UiString.ConfigWindow_List_DangerousStatusDesc.Local());
                DrawStatusList(nameof(OtherConfiguration.DangerousStatus), OtherConfiguration.DangerousStatus, AllDispelStatus);

                ImGui.TableNextColumn();

                ImGui.TextWrapped(UiString.ConfigWindow_List_NoCastingStatusDesc.Local());
                DrawStatusList(nameof(OtherConfiguration.NoCastingStatus), OtherConfiguration.NoCastingStatus, BadStatus);

            }
        }

        private static void FromClipBoardButton(HashSet<uint> items)
        {
            if (ImGui.Button(UiString.ConfigWindow_Actions_Copy.Local()))
            {
                try
                {
                    ImGui.SetClipboardText(JsonConvert.SerializeObject(items));
                }
                catch (Exception ex)
                {
                    Svc.Log.Warning(ex, "Failed to copy the values to the clipboard.");
                }
            }

            ImGui.SameLine();

            if (ImGui.Button(UiString.ActionSequencer_FromClipboard.Local()))
            {
                try
                {
                    foreach (var aId in JsonConvert.DeserializeObject<uint[]>(ImGui.GetClipboardText())!)
                    {
                        items.Add(aId);
                    }
                }
                catch (Exception ex)
                {
                    Svc.Log.Warning(ex, "Failed to copy the values from the clipboard.");
                }
                finally
                {
                    OtherConfiguration.Save();
                    ImGui.CloseCurrentPopup();
                }
            }
        }

        static string _statusSearching = string.Empty;
        private static void DrawStatusList(string name, HashSet<uint> statuses, Status[] allStatus)
        {
            using var id = ImRaii.PushId(name);
            FromClipBoardButton(statuses);

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

            if (ImageLoader.GetTexture(16220, out var text))
            {
                if (index++ % count != 0)
                {
                    ImGui.SameLine();
                }
                if (ImGuiHelper.NoPaddingNoColorImageButton(text.ImGuiHandle, new Vector2(24, 32) * Scale, name))
                {
                    if (!ImGui.IsPopupOpen(popupId)) ImGui.OpenPopup(popupId);
                }
                ImguiTooltips.HoveredTooltip(UiString.ConfigWindow_List_AddStatus.Local());
            }

            foreach (var status in statuses.Select(a => Service.GetSheet<Status>().GetRow(a))
                .Where(a => a != null)
                .OrderByDescending(s => Searchable.Similarity(s!.Name + " " + s.RowId.ToString(), _statusSearching)))
            {
                void Delete() => removeId = status.RowId;

                var key = "Status" + status!.RowId.ToString();

                ImGuiHelper.DrawHotKeysPopup(key, string.Empty, (UiString.ConfigWindow_List_Remove.Local(), Delete, ["Delete"]));

                if (ImageLoader.GetTexture(status.Icon, out var texture, notLoadId))
                {
                    if (index++ % count != 0)
                    {
                        ImGui.SameLine();
                    }
                    ImGuiHelper.NoPaddingNoColorImageButton(texture.ImGuiHandle, new Vector2(24, 32) * Scale, "Status" + status.RowId.ToString());

                    ImGuiHelper.ExecuteHotKeysPopup(key, string.Empty, $"{status.Name} ({status.RowId})", false,
                        (Delete, [VirtualKey.DELETE]));
                }
            }

            if (removeId != 0)
            {
                statuses.Remove(removeId);
                OtherConfiguration.Save();
            }
        }

        private static void DrawListActions(ConfigWindow window)
        {
            ImGui.SetNextItemWidth(ImGui.GetWindowWidth());
            ImGui.InputTextWithHint("##Searching the action", UiString.ConfigWindow_List_ActionNameOrId.Local(), ref _actionSearching, 128);

            using var table = ImRaii.Table("Rotation Solver List Actions", 3, ImGuiTableFlags.BordersInner | ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingStretchSame);
            if (table)
            {
                ImGui.TableSetupScrollFreeze(0, 1);
                ImGui.TableNextRow(ImGuiTableRowFlags.Headers);

                ImGui.TableNextColumn();
                ImGui.TableHeader(UiString.ConfigWindow_List_HostileCastingTank.Local());

                ImGui.TableNextColumn();
                ImGui.TableHeader(UiString.ConfigWindow_List_HostileCastingArea.Local());

                ImGui.TableNextColumn();
                ImGui.TableHeader(UiString.ConfigWindow_List_HostileCastingKnockback.Local());

                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.TextWrapped(UiString.ConfigWindow_List_HostileCastingTankDesc.Local());
                DrawActionsList(nameof(OtherConfiguration.HostileCastingTank), OtherConfiguration.HostileCastingTank);

                ImGui.TableNextColumn();
                window.Collection.DrawItems((int)UiString.ConfigWindow_List_HostileCastingArea);
                ImGui.TextWrapped(UiString.ConfigWindow_List_HostileCastingAreaDesc.Local());
                DrawActionsList(nameof(OtherConfiguration.HostileCastingArea), OtherConfiguration.HostileCastingArea);

                ImGui.TableNextColumn();
                window.Collection.DrawItems((int)UiString.ConfigWindow_List_HostileCastingKnockback);
                ImGui.TextWrapped(UiString.ConfigWindow_List_HostileCastingKnockbackDesc.Local());
                DrawActionsList(nameof(OtherConfiguration.HostileCastingKnockback), OtherConfiguration.HostileCastingKnockback);
            }
        }

        private static string _actionSearching = string.Empty;
        private static void DrawActionsList(string name, HashSet<uint> actions)
        {
            using var id = ImRaii.PushId(name);

            uint removeId = 0;

            var popupId = "Rotation Solver Action Popup" + name;

            if (ImGui.Button(UiString.ConfigWindow_List_AddAction.Local() + "##" + name))
            {
                if (!ImGui.IsPopupOpen(popupId)) ImGui.OpenPopup(popupId);
            }

            ImGui.SameLine();
            FromClipBoardButton(actions);

            ImGui.Spacing();

            foreach (var action in actions.Select(a => Service.GetSheet<GAction>().GetRow(a))
                .Where(a => a != null)
                .OrderByDescending(s => Searchable.Similarity(s!.Name + " " + s.RowId.ToString(), _actionSearching)))
            {
                void Reset() => removeId = action.RowId;

                var key = "Action" + action!.RowId.ToString();

                ImGuiHelper.DrawHotKeysPopup(key, string.Empty, (UiString.ConfigWindow_List_Remove.Local(), Reset, ["Delete"]));

                ImGui.Selectable($"{action.Name} ({action.RowId})");

                ImGuiHelper.ExecuteHotKeysPopup(key, string.Empty, string.Empty, false, (Reset, [VirtualKey.DELETE]));
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
                ImGui.InputTextWithHint("##Searching the action pop up", UiString.ConfigWindow_List_ActionNameOrId.Local(), ref _actionSearching, 128);

                ImGui.Spacing();

                using var child = ImRaii.Child("Rotation Solver Add action", new Vector2(-1, 400 * Scale));
                if (child)
                {
                    foreach (var action in AllActions.OrderByDescending(s => Searchable.Similarity(s.Name + " " + s.RowId.ToString(), _actionSearching)))
                    {
                        var selected = ImGui.Selectable($"{action.Name} ({action.RowId})");
                        if (ImGui.IsItemHovered())
                        {
                            ImguiTooltips.ShowTooltip($"{action.Name} ({action.RowId})");
                            if (selected)
                            {
                                actions.Add(action.RowId);
                                OtherConfiguration.Save();
                                ImGui.CloseCurrentPopup();
                            }
                        }
                    }
                }
            }
        }


        private static void DrawListTerritories()
        {
            if (Svc.ClientState == null) return;

            var territoryId = Svc.ClientState.TerritoryType;

            DrawTerritoryHeader();
            DrawContentFinder(DataCenter.ContentFinder);

            using var table = ImRaii.Table("Rotation Solver List Territories", 3, ImGuiTableFlags.BordersInner | ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingStretchSame);
            if (table)
            {
                ImGui.TableSetupScrollFreeze(0, 1);
                ImGui.TableNextRow(ImGuiTableRowFlags.Headers);

                ImGui.TableNextColumn();
                ImGui.TableHeader(UiString.ConfigWindow_List_NoHostile.Local());

                ImGui.TableNextColumn();
                ImGui.TableHeader(UiString.ConfigWindow_List_NoProvoke.Local());

                ImGui.TableNextColumn();
                ImGui.TableHeader(UiString.ConfigWindow_List_BeneficialPositions.Local());

                ImGui.TableNextRow();

                ImGui.TableNextColumn();

                ImGui.TextWrapped(UiString.ConfigWindow_List_NoHostileDesc.Local());

                var width = ImGui.GetColumnWidth() - ImGuiEx.CalcIconSize(FontAwesomeIcon.Ban).X - ImGui.GetStyle().ItemSpacing.X - 10 * Scale;

                if (!OtherConfiguration.NoHostileNames.TryGetValue(territoryId, out var libs))
                {
                    OtherConfiguration.NoHostileNames[territoryId] = libs = [];
                }

                //Add one.
                if (!libs.Any(string.IsNullOrEmpty))
                {
                    OtherConfiguration.NoHostileNames[territoryId] = [.. libs, string.Empty];
                }

                int removeIndex = -1;
                for (int i = 0; i < libs.Length; i++)
                {
                    ImGui.SetNextItemWidth(width);
                    if (ImGui.InputTextWithHint($"##Rotation Solver Territory Target Name {i}", UiString.ConfigWindow_List_NoHostilesName.Local(), ref libs[i], 1024))
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
                    OtherConfiguration.NoHostileNames[territoryId] = [.. list];
                    OtherConfiguration.SaveNoHostileNames();
                }
                ImGui.TableNextColumn();

                ImGui.TextWrapped(UiString.ConfigWindow_List_NoProvokeDesc.Local());

                width = ImGui.GetColumnWidth() - ImGuiEx.CalcIconSize(FontAwesomeIcon.Ban).X - ImGui.GetStyle().ItemSpacing.X - 10 * Scale;

                if (!OtherConfiguration.NoProvokeNames.TryGetValue(territoryId, out libs))
                {
                    OtherConfiguration.NoProvokeNames[territoryId] = libs = [];
                }
                //Add one.
                if (!libs.Any(string.IsNullOrEmpty))
                {
                    OtherConfiguration.NoProvokeNames[territoryId] = [.. libs, string.Empty];
                }
                removeIndex = -1;
                for (int i = 0; i < libs.Length; i++)
                {
                    ImGui.SetNextItemWidth(width);
                    if (ImGui.InputTextWithHint($"##Rotation Solver Territory Provoke Name {i}", UiString.ConfigWindow_List_NoProvokeName.Local(), ref libs[i], 1024))
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
                    OtherConfiguration.NoProvokeNames[territoryId] = [.. list];
                    OtherConfiguration.SaveNoProvokeNames();
                }

                ImGui.TableNextColumn();

                if (!OtherConfiguration.BeneficialPositions.TryGetValue(territoryId, out var pts))
                {
                    OtherConfiguration.BeneficialPositions[territoryId] = pts = [];
                }

                if (ImGui.Button(UiString.ConfigWindow_List_AddPosition.Local()) && Player.Available) unsafe
                    {
                        var point = Player.Object.Position;
                        int* unknown = stackalloc int[] { 0x4000, 0, 0x4000, 0 };

                        RaycastHit hit = default;

                        OtherConfiguration.BeneficialPositions[territoryId]
                        =
                        [
                            .. pts,
                        FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance()->BGCollisionModule
                            ->RaycastEx(&hit, point + Vector3.UnitY * 5, -Vector3.UnitY, 20, 1, unknown) ? hit.Point : point,
                        ];
                        OtherConfiguration.SaveBeneficialPositions();
                    }

                HoveredPosition = Vector3.Zero;
                removeIndex = -1;
                for (int i = 0; i < pts.Length; i++)
                {
                    void Reset() => removeIndex = i;

                    var key = "Beneficial Positions" + i.ToString();

                    ImGuiHelper.DrawHotKeysPopup(key, string.Empty, (UiString.ConfigWindow_List_Remove.Local(), Reset, ["Delete"]));

                    ImGui.Selectable(pts[i].ToString());

                    if (ImGui.IsItemHovered())
                    {
                        HoveredPosition = pts[i];
                    }

                    ImGuiHelper.ExecuteHotKeysPopup(key, string.Empty, string.Empty, false, (Reset, [VirtualKey.DELETE]));
                }
                if (removeIndex > -1)
                {
                    var list = pts.ToList();
                    list.RemoveAt(removeIndex);
                    OtherConfiguration.BeneficialPositions[territoryId] = [.. list];
                    OtherConfiguration.SaveBeneficialPositions();
                }
            }
        }
        #endregion

    }

    [Description("Timeline")]
    public class TimelineItem : ConfigWindowItemRS
    {
        internal static uint _territoryId = 0;
        private static string _territorySearch = string.Empty;
        internal static TerritoryConfig? _territoryConfig;

        public override uint Icon => 73;
        public override string Description => UiString.Item_Timeline.Local();

        public override void Draw(ConfigWindow window)
        {
            static string GetName(TerritoryType? territory)
            {
                var str = territory?.ContentFinderCondition?.Value?.Name?.RawString;
                if (str == null || string.IsNullOrEmpty(str)) return "Unnamed Duty";
                return str;
            }

            var territory = Svc.Data.GetExcelSheet<TerritoryType>();
            if (territory == null) return;

            var territories = RaidTimeUpdater.PathForRaids.Keys.OrderByDescending(i => i).Select(territory.GetRow).ToArray();

            var rightTerritory = territory?.GetRow(_territoryId);
            var name = GetName(rightTerritory);

            var imFont = DrawingExtensions.GetFont(21);
            float width = 0;
            using (var font = ImRaii.PushFont(imFont))
            {
                width = ImGui.CalcTextSize(name).X + ImGui.GetStyle().ItemSpacing.X * 2;
            }

            ImGuiHelper.DrawItemMiddle(() =>
            {
                ImGuiHelperRS.SearchCombo("##Choice the specific dungeon", name, ref _territorySearch, territories, GetName, t =>
                {
                    _territoryId = t?.RowId ?? 0;
                }, UiString.ConfigWindow_Condition_DutyName.Local(), imFont, ImGuiColors.DalamudYellow);
            }, ImGui.GetWindowWidth(), width);

            DrawContentFinder(rightTerritory?.ContentFinderCondition.Value);

            _territoryConfig = OtherConfiguration.GetTerritoryConfigById(_territoryId);

            ImGui.Separator();

            if (ImGui.Button(UiString.ConfigWindow_Actions_Copy.Local()))
            {
                var str = JsonConvert.SerializeObject(_territoryConfig, Formatting.Indented);
                ImGui.SetClipboardText(str);
            }

            ImGui.SameLine();

            if (ImGui.Button(UiString.ActionSequencer_FromClipboard.Local()))
            {
                var str = ImGui.GetClipboardText();
                try
                {
                    OtherConfiguration.SetTerritoryConfigById(_territoryId, str, true);
                }
                catch (Exception ex)
                {
                    Svc.Log.Warning(ex, "Failed to load the condition.");
                }
            }

            if (_territoryConfig != null)
            {
                TimelineDrawer.DrawTimeline(_territoryId, _territoryConfig);
            }
        }
    }

    [Description("Trigger")]
    public class TriggerItem : ConfigWindowItemRS
    {
        internal static TerritoryConfig? _territoryConfig;

        public override uint Icon => 24;
        public override string Description => UiString.Item_Trigger.Local();

        public static TriggerData TriggerData { get; set; } = default;
        public static bool IsJob { get; set; }

        public override void Draw(ConfigWindow window)
        {
            DrawTerritoryHeader();
            DrawContentFinder(DataCenter.ContentFinder);

            _territoryConfig = OtherConfiguration.GetTerritoryConfigById(Svc.ClientState.TerritoryType);

            ImGui.Separator();

            if (ImGui.Button(UiString.ConfigWindow_Actions_Copy.Local()))
            {
                var str = JsonConvert.SerializeObject(_territoryConfig, Formatting.Indented);
                ImGui.SetClipboardText(str);
            }

            ImGui.SameLine();

            if (ImGui.Button(UiString.ActionSequencer_FromClipboard.Local()))
            {
                var str = ImGui.GetClipboardText();
                try
                {
                    OtherConfiguration.SetTerritoryConfigById(Svc.ClientState.TerritoryType, str, true);
                }
                catch (Exception ex)
                {
                    Svc.Log.Warning(ex, "Failed to load the condition.");
                }
            }

            ImGui.SameLine();

            var isJob = IsJob;
            if (ImGui.Button(UiString.ConfigWindow_Trigger_IsJob.Local()))
            {
                IsJob = isJob;
            }

            using var table = ImRaii.Table("Trigger Table", 3, 
                ImGuiTableFlags.BordersInner | ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingStretchProp | ImGuiTableFlags.ScrollY);

            if (!table) return;

            ImGui.TableSetupScrollFreeze(0, 1);
            ImGui.TableNextRow(ImGuiTableRowFlags.Headers);

            ImGui.TableNextColumn();
            ImGui.TableHeader(UiString.ConfigWindow_Trigger_Log.Local());

            ImGui.TableNextColumn();
            ImGui.TableHeader(UiString.ConfigWindow_Trigger_TriggerData.Local());

            ImGui.TableNextColumn();
            ImGui.TableHeader(IsJob ? UiString.ConfigWindow_Timeline_JobActions.Local()
                : UiString.ConfigWindow_Timeline_Actions.Local());

            ImGui.TableNextRow();

            ImGui.TableNextColumn();

            DrawRecord();

            ImGui.TableNextColumn();

            DrawTriggerData();

            ImGui.TableNextColumn();

            DrawTrigger();
        }

        private static void DrawRecord()
        {
            int index = 0;
            foreach ((var time, var data) in Recorder.Data)
            {
                ImGui.SameLine();
                ImGui.Text(time.ToString("HH:mm:ss.fff") + "|");
                ImGui.SameLine();

                if (ImGui.Button($"{data}##{index++}"))
                {
                    AddTriggerData(data.ToTriggerData(), false);
                }
                if (ImGui.IsItemHovered())
                {
                    if (ImGui.IsMouseClicked(ImGuiMouseButton.Right))
                    {
                        AddTriggerData(data.ToTriggerData(), true);
                    }
                    ImGuiHelper.ShowTooltip(UiString.ConfigWindow_Trigger_AddTrggerDataDesc.Local());
                }
            }

            static void AddTriggerData(TriggerData data, bool isJob)
            {
                if (_territoryConfig == null) return;

                var dict = isJob ? _territoryConfig.JobConfig.Trigger : _territoryConfig.Config.Trigger;

                TriggerData = data;
                IsJob = isJob;

                if (dict.ContainsKey(data)) return;

                dict[data] = [];
            }
        }

        private static void DrawTriggerData()
        {
            if (_territoryConfig == null) return;

            var dict = IsJob ? _territoryConfig.JobConfig.Trigger : _territoryConfig.Config.Trigger;

            int index = 0;
            foreach (var key in dict.Keys)
            {
                if (ImGui.Selectable($"{key}##{index++}", key == TriggerData))
                {
                    TriggerData = key;
                }
            }
        }

        private static void DrawTrigger()
        {
            if (_territoryConfig == null) return;
            var dict = IsJob ? _territoryConfig.JobConfig.Trigger : _territoryConfig.Config.Trigger;

            if (!dict.TryGetValue(TriggerData, out var data)) return;

            DrawItems(data, IsJob);

            static void DrawItems(List<BaseTriggerItem> triggerItems, bool isJob)
            {
                AddButton();
                for (int i = 0; i < triggerItems.Count; i++)
                {
                    if (i != 0)
                    {
                        ImGui.Separator();
                    }
                    var triggerItem = triggerItems[i];

                    void Delete()
                    {
                        triggerItems.RemoveAt(i);
                    };

                    void Up()
                    {
                        triggerItems.RemoveAt(i);
                        triggerItems.Insert(Math.Max(0, i - 1), triggerItem);
                    };

                    void Down()
                    {
                        triggerItems.RemoveAt(i);
                        triggerItems.Insert(Math.Min(triggerItems.Count, i + 1), triggerItem);
                    }

                    void Execute()
                    {
                        Task.Run(async () =>
                        {
                            triggerItem.TerritoryAction.Enable();
                            await Task.Delay(3000);
                            triggerItem.TerritoryAction.Disable();
                        });
                    }

                    var key = $"TimelineItem Pop Up: {triggerItem.GetHashCode()}";

                    ImGuiHelper.DrawHotKeysPopup(key, string.Empty,
                        (UiString.ConfigWindow_List_Remove.Local(), Delete, ["Delete"]),
                        (UiString.ConfigWindow_Actions_MoveUp.Local(), Up, ["↑"]),
                        (UiString.ConfigWindow_Actions_MoveDown.Local(), Down, ["↓"]),
                        (UiString.TimelineExecute.Local(), Execute, ["→"]));

                    ConditionDrawer.DrawCondition(true);

                    ImGuiHelper.ExecuteHotKeysPopup(key, string.Empty, string.Empty, true,
                        (Delete, [VirtualKey.DELETE]),
                        (Up, [VirtualKey.UP]),
                        (Down, [VirtualKey.DOWN]),
                        (Execute, [VirtualKey.RIGHT]));

                    ImGui.SameLine();
                    using var grp = ImRaii.Group();

                    TerritoryActionDrawer.DrawTerritoryAction(triggerItem.TerritoryAction, []);
                }

                void AddButton()
                {
                    if (ImGuiEx.IconButton(FontAwesomeIcon.Plus, "AddTriggerButton" + isJob))
                    {
                        ImGui.OpenPopup("PopupTriggerButton" + isJob);
                    }
                    ImguiTooltips.HoveredTooltip(UiString.AddTimelineButton.Local());

                    using var popUp = ImRaii.Popup("PopupTriggerButton" + isJob);
                    if (!popUp) return;

                    if (isJob)
                    {
                        AddOneCondition<ActionTriggerItem>();
                    }
                    AddOneCondition<StateTriggerItem>();
                    AddOneCondition<DrawingTriggerItem>();
                    AddOneCondition<MacroTriggerItem>();
                    AddOneCondition<MoveTriggerItem>();
                    AddOneCondition<PathfindTriggerItem>();

                    void AddOneCondition<T>() where T : BaseTriggerItem
                    {
                        if (ImGui.Selectable(typeof(T).Local()))
                        {
                            triggerItems.Add(Activator.CreateInstance<T>());
                            ImGui.CloseCurrentPopup();
                        }
                    }
                }
            }
        }

    }

    [Description("Extra")]
    public class ExtraItem : ConfigWindowItemRS
    {
        private CollapsingHeaderGroup? _extraHeader;
        public override uint Icon => 51;
        public override string Description => UiString.Item_Extra.Local();

        public override void Draw(ConfigWindow window)
        {
            _extraHeader ??= new(new()
            {
                {   () =>UiString.ConfigWindow_EventItem.Local(), DrawEventTab },
                {
                    () =>UiString.ConfigWindow_Extra_Others.Local(),
                    () => window.Collection.DrawItems((int)UiString.ConfigWindow_Extra_Others)
                },
            });
            ImGui.TextWrapped(UiString.ConfigWindow_Extra_Description.Local());
            _extraHeader?.Draw();
        }

        private static void DrawEventTab()
        {
            if (ImGui.Button(UiString.ConfigWindow_Events_AddEvent.Local()))
            {
                Service.Config.Events.Add(new ActionEventInfo());
            }
            ImGui.SameLine();

            ImGui.TextWrapped(UiString.ConfigWindow_Events_Description.Local());

            ImGui.Text(UiString.ConfigWindow_Events_DutyStart.Local());
            ImGui.SameLine();
            Service.Config.DutyStart.DisplayMacro();

            ImGui.Text(UiString.ConfigWindow_Events_DutyEnd.Local());
            ImGui.SameLine();
            Service.Config.DutyEnd.DisplayMacro();

            ImGui.Separator();

            ActionEventInfo? remove = null;
            foreach (var eve in Service.Config.Events)
            {
                eve.DisplayEvent();

                ImGui.SameLine();

                if (ImGui.Button($"{UiString.ConfigWindow_Events_RemoveEvent.Local()}##RemoveEvent{eve.GetHashCode()}"))
                {
                    remove = eve;
                }
                ImGui.Separator();
            }
            if (remove != null)
            {
                Service.Config.Events.Remove(remove);
            }
        }
    }

    [Description("Debug")]
    public class DebugItem : ConfigWindowItemRS
    {
        public override uint Icon => 5;
        public override void Draw(ConfigWindow window)
        {
            if (Player.Available)
            {
                ImGui.Text("Hash: ");
                ImGui.SameLine();
                var hash = Player.Object.EncryptString();
                if (ImGui.Button(hash))
                {
                    ImGui.SetClipboardText(hash);
                    Notify.Success(UiString.CopiedYourHash.Local());
                }
            }

            window.Collection.DrawItems(-1);

            if (!Player.Available || !Service.Config.InDebug) return;

            _debugHeader?.Draw();
        }

        private static readonly CollapsingHeaderGroup _debugHeader = new(new()
    {
        {() => DataCenter.RightNowRotation != null ? "Rotation" : string.Empty, DrawDebugRotationStatus},
        {() =>"Status", DrawStatus },
        {() =>"Party", DrawParty },
        {() =>"Target Data", DrawTargetData },
        {() =>"Next Action", DrawNextAction },
        {() =>"Last Action", DrawLastAction },
        {() =>"Others", DrawOthers },
        {() =>"Effect",  () =>
            {
                ImGui.Text(Watcher.ShowStrSelf);
                ImGui.Separator();
                ImGui.Text(DataCenter.Role.ToString());
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
            ImGui.Text("AnimationLock: " + DataCenter.AnimationLocktime.ToString());

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
            ImGui.Text("Party: " + DataCenter.PartyMembers.Length.ToString());
            ImGui.Text("Alliance: " + DataCenter.AllianceMembers.Length.ToString());

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
                ImGui.Text("Rank: " + b.GetObjectNPC()?.Rank.ToString() ?? string.Empty);
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
            ImGui.Text("Hostile: " + DataCenter.AllHostileTargets.Length.ToString());
            foreach (var item in DataCenter.AllHostileTargets)
            {
                ImGui.Text(item.Name.ToString());
            }
        }

        private static void DrawNextAction()
        {
            ImGui.Text(DataCenter.RightNowRotation?.GetType().GetCustomAttribute<RotationAttribute>()!.Name);
            ImGui.Text(DataCenter.SpecialType.ToString());

            ImGui.Text(ActionUpdater.NextAction?.Name ?? "null");
            ImGui.Text("Ability Remain: " + DataCenter.AbilityRemain.ToString());
            ImGui.Text("Action Remain: " + DataCenter.AnimationLocktime.ToString());
            ImGui.Text("Weapon Remain: " + DataCenter.WeaponRemain.ToString());
        }

        private static void DrawLastAction()
        {
            DrawAction(DataCenter.LastAction, nameof(DataCenter.LastAction));
            DrawAction(DataCenter.LastAbility, nameof(DataCenter.LastAbility));
            DrawAction(DataCenter.LastGCD, nameof(DataCenter.LastGCD));
            DrawAction(DataCenter.LastComboAction, nameof(DataCenter.LastComboAction));
        }

        private static unsafe void DrawOthers()
        {
            ImGui.Text("Combat Time: " + (DataCenter.CombatTimeRaw).ToString());
            ImGui.Text("Limit Break: " + CustomRotation.LimitBreakLevel.ToString());
        }

        private static void DrawAction(ActionID id, string type)
        {
            ImGui.Text($"{type}: {id}");
        }
    }

    [Description("ChangeLog")]
    public class ChangeLogItem : ConfigWindowItemRS
    {
        public override uint Icon => 80;

        public override string Link => $"https://github.com/{XIVConfigUIMain.UserName}/{XIVConfigUIMain.RepoName}/blob/main/CHANGELOG.md";
    }

    private static void DrawGitHubBadge(string userName, string repository, string id = "", string link = "", bool center = false)
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

    private static string ToCommandStr(OtherCommandType type, string str, string extra = "")
    {
        var result = Service.COMMAND + " " + type.ToString() + " " + str;
        if (!string.IsNullOrEmpty(extra)) result += " " + extra;
        return result;
    }

    private static void DrawTerritoryHeader()
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

    private static void DrawContentFinder(ContentFinderCondition? content)
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

    private static uint ChangeAlpha(uint color)
    {
        var c = ImGui.ColorConvertU32ToFloat4(color);
        c.W = 0.55f;
        return ImGui.ColorConvertFloat4ToU32(c);
    }
}