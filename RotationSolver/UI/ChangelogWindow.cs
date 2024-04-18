using Dalamud;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Style;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
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
            IsOpen = true;
            PopulateChangelogs();
        }

        private const ImGuiWindowFlags BaseFlags = ImGuiWindowFlags.NoCollapse
                                    | ImGuiWindowFlags.NoSavedSettings;

        private string _assemblyVersion = typeof(RotationConfigWindow).Assembly.GetName().Version?.ToString() ?? "?.?.?";

        private string _lastSeenChangelog = Service.Config.LastSeenChangelog;

        private Dictionary<string, string> _changeLogs = new Dictionary<string, string>();

        private void PopulateChangelogs()
        {
            _changeLogs.Clear();
            Task.Run(GetChangelogs);
        }

        private async Task GetChangelogs()
        {
            List<GithubRelease.Release> releases = new();
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "RotationSolver");
                    client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
                    var response = await client.GetAsync($"https://api.github.com/repos/{Service.USERNAME}/{Service.REPO}/releases");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        releases = JsonConvert.DeserializeObject<List<GithubRelease.Release>>(content);
                    }
                    else
                    {
                        Svc.Log.Error($"Failed to get changelog: {response.StatusCode}");
                    }
                }
                if (releases != null && releases.Count > 0)
                {
                    foreach (var release in releases)
                    {
                        _changeLogs.Add(release.TagName, release.Body);
                    }
                    this.IsOpen = true;
                }
                else
                {
                    Svc.Log.Error("Failed to get changelog: No releases found");
                }
            }
            catch (Exception ex)
            {
                Svc.Log.Error(ex, "Failed to get changelog");
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
            string title = "Rotation Solver Reborn was updated!";
            ImGui.PushFont(DrawingExtensions.GetFont(ImGui.GetFontSize() + 10));  // Set the font first to calculate correct size
            float windowWidth = ImGui.GetWindowWidth();
            float textWidth = ImGui.CalcTextSize(title).X;  // Calculate text width with the correct font
            ImGui.SetCursorPosX((windowWidth - textWidth) * 0.5f);  // Correctly center the text
            ImGui.TextColored(ImGuiColors.ParsedGold, title);  // Render the text
            ImGui.PopFont();  // Reset to the previous font
            ImGui.Separator();  // Separator for aesthetic or logical separation

            // Ensure the dictionary is sorted by version number in descending order
            var sortedLogs = _changeLogs.OrderByDescending(x => x.Key).ToList().Take(15);

            // Track the first item
            bool first = true;

            foreach (KeyValuePair<string, string> log in sortedLogs)
            {
                if (first && _first)
                {
                    ImGui.SetNextItemOpen(true);
                    first = false;
                    _first = false;
                }

                if (ImGui.CollapsingHeader($"Version {log.Key}"))
                {
                    RenderMarkdown(log.Value);
                }
            }
            ImGui.Separator();
            ImGui.Text($"Older changelogs are available on GitHub");
            if (ImGui.Button("Open GitHub"))
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = $"https://github.com/{Service.USERNAME}/{Service.REPO}", UseShellExecute = true });
            }
        }

        public override void OnClose()
        {
            Service.Config.LastSeenChangelog = _assemblyVersion;
            Service.Config.Save();
            base.OnClose();
        }
        public override bool DrawConditions()
        {
            return Svc.ClientState.IsLoggedIn && _lastSeenChangelog != _assemblyVersion && _changeLogs.Count > 0;
        }
    }
}
