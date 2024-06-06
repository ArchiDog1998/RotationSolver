using Dalamud.Interface.Colors;
using Dalamud.Interface.GameFonts;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using ECommons.GameHelpers;
using ECommons.ImGuiMethods;
using RotationSolver.Helpers;
using System.Diagnostics;
using XIVConfigUI;
using XIVConfigUI.Attributes;
using XIVDrawer;

namespace RotationSolver.UI.ConfigWindows;
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

        var warning = type.Assembly.GetWarning();
        if (warning == UiString.None)
        {
            using var color = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudYellow);
            ImGui.TextWrapped(string.Format(rotation.WhyNotValid, warning.Local()));
        }

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
        using (var font = ImRaii.PushFont(ImGuiHelper.GetFont(FontSize.Fourth, GameFontFamily.MiedingerMid)))
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

        var ratings = DownloadHelper.GetRating(rotation.GetType(), out var rate);

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

                var pairs = ratings.GroupBy(i => i.Value).ToDictionary(g => g.Key, g => g.Count());

                float? rateWidth = null; 

                for (byte i = 0; i <= 10; i++)
                {
                    if (!pairs.TryGetValue(i, out var rateCount)) rateCount = 0;
                    var r = rateCount / count;

                    using (var font = ImRaii.PushFont(ImGuiHelper.GetFont(FontSize.Fifth, GameFontFamily.MiedingerMid)))
                    {
                        ImGui.Text(i.ToString());
                        rateWidth ??= ImGui.CalcTextSize("10").X;
                    }

                    if (rateWidth.HasValue)
                    {
                        ImGui.SameLine(rateWidth.Value);
                    }
                    else
                    {
                        ImGui.SameLine();
                    }

                    using (var font = ImRaii.PushFont(ImGuiHelper.GetFont(FontSize.Sixth, GameFontFamily.MiedingerMid)))
                    {
                        ImGui.ProgressBar(r, new(400, 20), $"{r:P1}({rateCount:N0})");
                    }
                }
            }
        }

        using (var popup = ImRaii.Popup("Rotation Solver Your Rating"))
        {
            if (popup.Success)
            {
                var time = _nextChangeTime - DateTime.Now;

                if (time > TimeSpan.Zero)
                {
                    using var font = ImRaii.PushFont(ImGuiHelper.GetFont(FontSize.Fifth, GameFontFamily.Axis));
                    ImGui.TextColored(ImGuiColors.DalamudRed, string.Format(UiString.Rotation_Rate.Local(), (int)time.TotalSeconds));
                }
                else
                {
                    using var font = ImRaii.PushFont(ImGuiHelper.GetFont(FontSize.Fourth, GameFontFamily.MiedingerMid));
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
            ImGuiHelper.ShowTooltip(UiString.ConfigWindow_Rotation_Rating_CountOfLastUsing.Local());
        }
        if (DrawRating((float)rotation.AverageCountOfCombatTimeUsing, rotation.MaxCountOfCombatTimeUsing, 20))
        {
            ImGuiHelper.ShowTooltip(UiString.ConfigWindow_Rotation_Rating_CountOfCombatTimeUsing.Local());
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
            var wholeWidth = ImGui.GetWindowSize().X - size.X - (2 * radius);
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

                ImGui.GetWindowDrawList().AddRectFilled(start + shift, start + stepThis - shift, ImGuiHelperRS.ChangeAlpha(RatingColors[i]), radius,
                   isStart ? ImDrawFlags.RoundCornersLeft : isLast ? ImDrawFlags.RoundCornersRight : ImDrawFlags.RoundCornersNone);
                start += new Vector2(stepThis.X, 0);
            }

            ImGui.GetWindowDrawList().AddRect(veryStart + shift, start + new Vector2(0, size.Y) - shift, ImGui.ColorConvertFloat4ToU32(ImGuiColors.DalamudWhite2), radius);

            var linePt = veryStart + shift + new Vector2(radius + (wholeWidth * ratio1), 0);
            ImGui.GetWindowDrawList().AddLine(linePt, linePt + new Vector2(0, step.Y - (2 * shift.Y)), uint.MaxValue, 3);

            linePt = veryStart + shift + new Vector2(radius + (wholeWidth * ratio2), 0);
            ImGui.GetWindowDrawList().AddLine(linePt, linePt + new Vector2(0, step.Y - (2 * shift.Y)), uint.MaxValue, 3);

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
                    ImGuiHelperRS.DrawGitHubBadge(userName, repository, link.Path, $"https://github.com/{userName}/{repository}/blob/{link.Path}", center: true);
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