﻿using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using System.ComponentModel;
using System.Text.RegularExpressions;
using XIVConfigUI;

namespace RotationSolver.UI.ConfigWindows;

[Description("ChangeLog")]
public partial class ChangeLogItem : ConfigWindowItemRS
{
    private static string OpenLink = $"https://github.com/{XIVConfigUIMain.UserName}/{XIVConfigUIMain.RepoName}/blob/main/CHANGELOG.md",
        DownloadLind = $"https://raw.githubusercontent.com/{XIVConfigUIMain.UserName}/{XIVConfigUIMain.RepoName}/main/CHANGELOG.md";

    private enum LineType : byte
    {
        None,
        Header,
        Version,
        Category,
        Item,
    }

    public override uint Icon => 80;

    public override void Draw(ConfigWindow window)
    {
        var changeLog = GetChangeLog();
        if (string.IsNullOrEmpty(changeLog)) return;

        var lines = changeLog.Split('\n')
            .Where(s => !string.IsNullOrEmpty(s));

        foreach (var s in lines)
        {
            var line = s;
            var type = GetLineType(ref line);
            if (type == LineType.None) continue;

            var fontSize = type switch
            {
                LineType.Header => FontSize.First,
                LineType.Version => FontSize.Second,
                LineType.Category => FontSize.Third,
                _ => FontSize.Fourth,
            };

            using var font = ImRaii.PushFont(ImGuiHelper.GetFont(fontSize));

            if (type == LineType.Header)
            {
                if (ImGui.Button(line))
                {
                    Util.OpenLink(OpenLink);
                }
            }
            else
            {
                DrawLine(line);
            }
        }
    }

    private static void DrawLine(string text)
    {
        var regex = ButtonRegex();

        Match matched = regex.Match(text);

        while (matched.Success)
        {
            var buttonStr = matched.Value;
            ImGui.Text(text[..matched.Index]);
            text = text[(matched.Index + matched.Length)..];
            ImGui.SameLine();
            DrawButton(buttonStr);
            ImGui.SameLine();
        }

        ImGui.NewLine();
    }

    private static void DrawButton(string buttonStr)
    {
        var name = ButtonName().Match(buttonStr).Value[1..^1];
        var link = ButtonLink().Match(buttonStr).Value[1..^1];

        if (ImGui.Button(name + "##" + link))
        {
            Util.OpenLink(link);
        }
    }

    private static LineType GetLineType(ref string line)
    {
        if (line.StartsWith("# "))
        {
            line = line[2..];
            return LineType.Header;
        }
        else if (line.StartsWith("## "))
        {
            line = line[3..];
            return LineType.Version;
        }
        else if (line.StartsWith("### "))
        {
            line = line[4..];
            return LineType.Category;
        }
        else if (line.StartsWith("* "))
        {
            line = line[2..];
            return LineType.Category;
        }
        else
        {
            return LineType.None;
        }
    }

    private static string ChangeLog = string.Empty;
    private static bool IsDownload = false;
    private static string GetChangeLog()
    {
        if (!string.IsNullOrEmpty(ChangeLog)) return ChangeLog;
        if (IsDownload) return string.Empty;
        IsDownload = true;

        Task.Run(async () => 
        {
            using var client = new HttpClient();
            ChangeLog = await client.GetStringAsync(DownloadLind);
        });

        return string.Empty;
    }

    [GeneratedRegex("\\[.+?\\]")]
    private static partial Regex ButtonName();

    [GeneratedRegex("\\(.+?\\)")]
    private static partial Regex ButtonLink();

    [GeneratedRegex("\\[.+?\\]\\(.+?\\)")]
    private static partial Regex ButtonRegex();
}