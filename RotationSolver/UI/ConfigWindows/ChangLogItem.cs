using System.ComponentModel;
using XIVConfigUI;

namespace RotationSolver.UI.ConfigWindows;

[Description("ChangeLog")]
public class ChangeLogItem : ConfigWindowItemRS
{
    public override uint Icon => 80;

    public override string Link => $"https://github.com/{XIVConfigUIMain.UserName}/{XIVConfigUIMain.RepoName}/blob/main/CHANGELOG.md";
}