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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XIVPainter;

namespace RotationSolver.UI
{
    internal class ChangelogWindow : Window
    {
        public ChangelogWindow() : base("Rotation Solver Reborn Changelog", BaseFlags)
        {
            Size = new Vector2(600, 400);
            SizeCondition = ImGuiCond.FirstUseEver;
            if (_lastSeenChangelog != _assemblyVersion)
            {
                IsOpen = true;
                PopulateChangelogs();
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
                        if (releases?.Count > 0)
                        {
                            foreach (var release in releases)
                            {
                                if (release.Prerelease) continue;
                                return release.TagName;
                            }
                        }
                        return "4.0.0.0";
                    }
                    else
                    {
                        Svc.Log.Error($"Failed to get releases: {response.StatusCode}");
                        return "4.0.0.0";
                    }
                }
            }
            catch (Exception ex)
            {
                Svc.Log.Error(ex, "Failed to get releases");
                return "4.0.0.0";
            }
        }

        private void RenderMarkdown(string markdownText)
        {
            string[] lines = markdownText.Split('\n');

            foreach (string line in lines)
            {
                // Check for headings
                if (line.StartsWith("## "))
                {
                    ImGui.PushFont(DrawingExtensions.GetFont(ImGui.GetFontSize() + 6));
                    RenderStyledText(line.Substring(3));
                    ImGui.PopFont();
                }
                else if (line.TrimStart().StartsWith("- "))
                {
                    ImGui.Bullet();
                    RenderStyledText(line.Substring(2).Trim());
                }
                else
                {
                    RenderStyledText(line);
                }
            }
        }

        private void RenderStyledText(string text)
        {
            // Regex for URLs and bold syntax
            Regex urlRegex = new Regex(@"\bhttps?://\S+\b", RegexOptions.Compiled);
            Regex boldRegex = new Regex(@"\*\*(.*?)\*\*");
            Regex italicRegex = new Regex(@"\*(.*?)\*");

            int lastPos = 0;
            while (lastPos < text.Length)
            {
                Match urlMatch = urlRegex.Match(text, lastPos);
                Match boldMatch = boldRegex.Match(text, lastPos);
                Match italicMatch = italicRegex.Match(text, lastPos);

                // Find the nearest match
                Match nearestMatch = null;
                int nearestPos = int.MaxValue;

                if (urlMatch.Success && urlMatch.Index < nearestPos)
                {
                    nearestMatch = urlMatch;
                    nearestPos = urlMatch.Index;
                }

                if (boldMatch.Success && boldMatch.Index < nearestPos)
                {
                    nearestMatch = boldMatch;
                    nearestPos = boldMatch.Index;
                }

                if (italicMatch.Success && italicMatch.Index < nearestPos)
                {
                    nearestMatch = italicMatch;
                    nearestPos = italicMatch.Index;
                }

                // Render text up to the match
                if (nearestMatch != null)
                {
                    ImGui.TextUnformatted(text.Substring(lastPos, nearestMatch.Index - lastPos));
                    ImGui.SameLine(0, 0);
                }

                // Handle the specific markdown
                if (nearestMatch == urlMatch)
                {
                    string url = urlMatch.Value;

                    if (ImGui.Button($"Open Link"))
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = url, UseShellExecute = true });
                    }

                    lastPos = urlMatch.Index + urlMatch.Length;
                }
                else if (nearestMatch == boldMatch)
                {
                    ImGui.PushFont(DrawingExtensions.GetFont(ImGui.GetFontSize() + 3));
                    ImGui.Text(boldMatch.Groups[1].Value);
                    ImGui.PopFont();
                    ImGui.SameLine(0, 0);

                    lastPos = boldMatch.Index + boldMatch.Length;
                }
                else if (nearestMatch == italicMatch)
                {
                    // Assuming italicFont is defined and loaded appropriately
                    ImGui.PushFont(DrawingExtensions.GetFont(ImGui.GetFontSize()));
                    ImGui.Text(italicMatch.Groups[1].Value);
                    ImGui.PopFont();
                    ImGui.SameLine(0, 0);

                    lastPos = italicMatch.Index + italicMatch.Length;
                }
                else
                {
                    // No more matches, render the rest of the text
                    ImGui.TextUnformatted(text.Substring(lastPos));
                    break;
                }
            }
        }

        bool _first = true;

        public override void Draw()
        {
            // Centered title
            string title = $"Welcome to Rotation Solver Reborn!";
            ImGui.PushFont(DrawingExtensions.GetFont(ImGui.GetFontSize() + 10));  // Set the font first to calculate correct size
            float windowWidth = ImGui.GetWindowWidth();
            float textWidth = ImGui.CalcTextSize(title).X;  // Calculate text width with the correct font
            ImGui.SetCursorPosX((windowWidth - textWidth) * 0.5f);  // Correctly center the text
            ImGui.TextColored(ImGuiColors.ParsedGold, title);  // Render the text
            ImGui.PopFont();  // Reset to the previous font

            string version = $"Version {_assemblyVersion}";
            ImGui.PushFont(DrawingExtensions.GetFont(ImGui.GetFontSize() + 3));
            float versionWidth = ImGui.CalcTextSize(version).X;
            ImGui.SetCursorPosX((windowWidth - versionWidth) * 0.5f);
            ImGui.TextColored(ImGuiColors.TankBlue, version);
            ImGui.PopFont();

            string message = $"Here's what's new since the last time you were here:";
            ImGui.PushFont(DrawingExtensions.GetFont(ImGui.GetFontSize() + 1));
            float messageWidth = ImGui.CalcTextSize(message).X;
            ImGui.SetCursorPosX((windowWidth - messageWidth) * 0.5f);
            ImGui.Text(message);
            ImGui.PopFont();

            ImGui.Separator();  // Separator for aesthetic or logical separation

            // Track the first item
            bool first = true;

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
            var changeLog = _changeLog;
            if (changeLog == null || changeLog.Commits == null || changeLog.Commits.Count == 0)
            {
                ImGui.Text("No changelog available.");
                return;
            }

            var commits = changeLog.Commits.OrderByDescending(c => c.CommitData.CommitAuthor.Date).Where(c => !c.CommitData.Message.Contains("Merge pull request"));
            List<string> authors = GetAuthorsFromChangeLogs(commits);
            ImGui.PushFont(DrawingExtensions.GetFont(ImGui.GetFontSize() + 1));
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
