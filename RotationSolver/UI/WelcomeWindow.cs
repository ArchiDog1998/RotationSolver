using Dalamud;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Style;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ECommons;
using ECommons.DalamudServices;
using ECommons.Reflection;
using RotationSolver.Data;
using RotationSolver.Localization;
using RotationSolver.Updaters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RotationSolver.UI
{
    internal class WelcomeWindow : Window
    {
        public WelcomeWindow() : base($"Welcome to Rotation Solver Reborn!", BaseFlags)
        {
            Size = new Vector2(650, 500);
            SizeCondition = ImGuiCond.FirstUseEver;
            if (_lastSeenChangelog != _assemblyVersion || !Service.Config.FirstTimeSetupDone)
            {
                PopulateChangelogs();
                IsOpen = true;
            }
        }

        private const ImGuiWindowFlags BaseFlags = ImGuiWindowFlags.NoCollapse
                                    | ImGuiWindowFlags.NoSavedSettings;
#if DEBUG
        private string _assemblyVersion = "4.0.5.5";
#else
        private string _assemblyVersion = typeof(RotationConfigWindow).Assembly.GetName().Version?.ToString() ?? "4.0.5.4";
#endif

        private string _lastSeenChangelog = Service.Config.LastSeenChangelog;

        private GitHubCommitComparison _changeLog = new();

        private void PopulateChangelogs()
        {
            Task.Run(GetGithubComparison);
        }

        private async Task GetGithubComparison()
        {
            var comparisonGoal = _lastSeenChangelog == "0.0.0.0" ? await GetNextMostRecentReleaseTag() : _lastSeenChangelog;
            string url = $"https://api.github.com/repos/{Service.USERNAME}/{Service.REPO}/compare/{comparisonGoal}...{_assemblyVersion}";
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "RotationSolver");
                    client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        _changeLog = JsonConvert.DeserializeObject<GitHubCommitComparison>(content);
                    }
                    else
                    {
                        Svc.Log.Error($"Failed to get comparison: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Svc.Log.Error(ex, "Failed to get comparison");
            }
        }

        private async Task<string> GetNextMostRecentReleaseTag()
        {
            var url = $"https://api.github.com/repos/{Service.USERNAME}/{Service.REPO}/releases";
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "RotationSolver");
                    client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var releases = JsonConvert.DeserializeObject<List<GithubRelease.Release>>(content);
                        var foundLatest = false;
                        if (releases?.Count > 0)
                        {
                            foreach (var release in releases)
                            {
                                if (release.Prerelease) continue;
                                if (!foundLatest)
                                {
                                    foundLatest = true;
                                    continue;
                                }
                                return release.TagName;
                            }
                        }
                        return "4.1.0.0";
                    }
                    else
                    {
                        Svc.Log.Error($"Failed to get releases: {response.StatusCode}");
                        return "4.1.0.0";
                    }
                }
            }
            catch (Exception ex)
            {
                Svc.Log.Error(ex, "Failed to get releases");
                return "4.1.0.0";
            }
        }

        public override void Draw()
        {
            var windowWidth = ImGui.GetWindowWidth();
            // Centered title
            var text = UiString.WelcomeWindow_Header.Local();
            ImGui.PushFont(FontManager.GetFont(ImGui.GetFontSize() + 10));
            var textSize = ImGui.CalcTextSize(text).X;
            ImGuiHelper.DrawItemMiddle(() =>
            {
                ImGui.TextColored(ImGuiColors.ParsedGold, text);
            }, windowWidth, textSize);
            ImGui.PopFont();

            text = $"Version {_assemblyVersion}";
            ImGui.PushFont(FontManager.GetFont(ImGui.GetFontSize() + 3));
            textSize = ImGui.CalcTextSize(text).X;
            ImGuiHelper.DrawItemMiddle(() =>
            {
                ImGui.TextColored(ImGuiColors.TankBlue, text);
            }, windowWidth, textSize);
            ImGui.PopFont();

            text = Service.Config.FirstTimeSetupDone ? UiString.WelcomeWindow_WelcomeBack.Local() : UiString.WelcomeWindow_Welcome.Local();
            ImGui.PushFont(FontManager.GetFont(ImGui.GetFontSize() + 1));
            textSize = ImGui.CalcTextSize(text).X;
            ImGuiHelper.DrawItemMiddle(() =>
            {
                ImGui.TextColored(ImGuiColors.ParsedBlue, text);
            }, windowWidth, textSize);
            ImGui.PopFont();


            ImGui.Separator();  // Separator for aesthetic or logical separation

            if (!Service.Config.FirstTimeSetupDone)
            {
                text = UiString.WelcomeWindow_FirstTime.Local();
                ImGui.PushFont(FontManager.GetFont(ImGui.GetFontSize() + 3));
                textSize = ImGui.CalcTextSize(text).X;
                ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudOrange);
                ImGui.TextWrapped(text);
                ImGui.PopStyleColor();
                ImGui.PopFont();

                text = UiString.WelcomeWindow_FirstTime3.Local();
                ImGui.Text(text);
                var autoUpdate = Service.Config.AutoLoadRotations.Value;
                if (ImGui.Checkbox(UiString.WelcomeWindow_LoadAtStartup.Local(), ref autoUpdate))
                {
                    Service.Config.AutoLoadRotations.Value = autoUpdate;
                    Service.Config.Save();
                }
                var autoReload = Service.Config.AutoReloadRotations.Value;
                if (ImGui.Checkbox(UiString.WelcomeWindow_AutoReload.Local(), ref autoReload))
                {
                    Service.Config.AutoReloadRotations.Value = autoReload;
                    Service.Config.Save();
                }

                text = UiString.WelcomeWindow_FirstTime2.Local();
                ImGui.PushFont(FontManager.GetFont(ImGui.GetFontSize() + 2));
                textSize = ImGui.CalcTextSize(text).X;
                ImGuiHelper.DrawItemMiddle(() =>
                {
                    ImGui.TextColored(ImGuiColors.DalamudOrange, text);
                }, windowWidth, textSize);
                ImGui.PopFont();

                text = UiString.WelcomeWindow_SaveAndInstall.Local();
                textSize = ImGui.CalcTextSize(text).X;
                ImGuiHelper.DrawItemMiddle(async () =>
                {
                    if (ImGui.Button(text))
                    {
                        Service.Config.FirstTimeSetupDone = true;
                        Service.Config.Save();
                        await Task.Run(async () =>
                        {
                            await RotationUpdater.GetAllCustomRotationsAsync(DownloadOption.Download | DownloadOption.Local | DownloadOption.ShowList);
                            Service.Config.FirstTimeSetupDone = true;
                            Service.Config.Save();
                        });
                    }
                }, windowWidth, textSize);
                ImGui.Separator();
            }

            DrawChangeLog();

            ImGui.Separator();
            ImGui.Text($"Older changelogs are available on GitHub");
            if (ImGui.Button("Open GitHub"))
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = $"https://github.com/{Service.USERNAME}/{Service.REPO}", UseShellExecute = true });
            }
        }

        private void DrawChangeLog()
        {
            var text = UiString.WelcomeWindow_Changelog.Local();
            var textSize = ImGui.CalcTextSize(text).X;
            ImGuiHelper.DrawItemMiddle(() =>
            {
                ImGui.TextColored(ImGuiColors.HealerGreen, text);
            }, ImGui.GetWindowWidth(), textSize);
            var changeLog = _changeLog;
            if (changeLog == null || changeLog.Commits == null || changeLog.Commits.Count == 0)
            {
                ImGui.Text("No changelog available.");
                return;
            }

            var commits = changeLog.Commits.OrderByDescending(c => c.CommitData.CommitAuthor.Date).Where(c => !c.CommitData.Message.Contains("Merge pull request"));
            List<string> authors = GetAuthorsFromChangeLogs(commits);
            ImGui.PushFont(FontManager.GetFont(ImGui.GetFontSize() + 1));
            ImGui.Text($"You've missed {commits.Count()} changes from {authors.Count()} contributer{(authors.Count() > 1 ? "s" : "")}!");
            ImGui.PopFont();
            foreach (var commit in commits)
            {

                ImGui.Text($"[{commit.CommitData.CommitAuthor.Date:yyyy-MM-dd}]");

                ImGui.Indent();
                ImGui.Text($"- {commit.CommitData.Message}");

                ImGui.Text($"By: @{commit.CommitData.CommitAuthor.Name}");
                ImGui.Unindent();
            }

            ImGui.NewLine();
            ImGui.Text("Contributors:");
            foreach (var author in authors)
            {
                if (ImGui.Button(author))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = $"https://github.com/{author}", UseShellExecute = true });
                }
            }
            //Build file stats
            var additions = changeLog.Files.Sum(f => f.Additions);
            var deletions = changeLog.Files.Sum(f => f.Deletions);
            var files = changeLog.Files.Count;
            if (ImGui.CollapsingHeader("Fun stats for nerds"))
            {
                ImGui.Text($"Total commits: {changeLog.TotalCommits}");
                ImGui.Text($"Total files changed: {files}");
                ImGui.Text($"Total additions: {additions}");
                ImGui.Text($"Total deletions: {deletions}");
            }
        }

        private List<string> GetAuthorsFromChangeLogs(IEnumerable<Commit> commits)
        {
            var authors = new List<string>();
            foreach (var commit in commits)
            {
                if (!authors.Contains(commit.CommitData.CommitAuthor.Name))
                {
                    authors.Add(commit.CommitData.CommitAuthor.Name);
                }
            }
            return authors;
        }

        public override void OnClose()
        {
            Service.Config.LastSeenChangelog = _assemblyVersion;
            Service.Config.FirstTimeSetupDone = true;
            Service.Config.Save();
            IsOpen = false;
            base.OnClose();
        }
        public override bool DrawConditions()
        {
            return Svc.ClientState.IsLoggedIn;
        }
    }
}
