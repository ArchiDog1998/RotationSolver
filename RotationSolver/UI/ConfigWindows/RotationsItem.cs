using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Updaters;
using System.ComponentModel;
using System.Diagnostics;
using XIVConfigUI;
using XIVConfigUI.Attributes;

namespace RotationSolver.UI.ConfigWindows;
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

                    ImGuiHelper.HoveredTooltip(string.Join('\n', jobs.Select(t => t.GetCustomAttribute<UIAttribute>()?.Name ?? t.Name)));
                }

                ImGui.TableNextColumn();

                if (!string.IsNullOrEmpty(info.GitHubUserName) && !string.IsNullOrEmpty(info.GitHubRepository) && !string.IsNullOrEmpty(info.FilePath))
                {
                    ImGuiHelperRS.DrawGitHubBadge(info.GitHubUserName, info.GitHubRepository, info.FilePath);
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

            ImGuiHelperRS.DrawGitHubBadge(userName, repository, fileName, center: true);
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

            ImGuiHelperRS.DrawGitHubBadge(userName, repository, fileName, center: true);

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
